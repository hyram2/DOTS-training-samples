using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateParticleDistanceSystem))]
    public class UpdateParticleColorSystem : JobComponentSystem
    {
        EntityQuery m_ParticleSetup;
        protected override void OnCreate()
        {
            base.OnCreate();
            m_ParticleSetup = GetEntityQuery(
                ComponentType.ReadOnly<ParticleSetupComponent>()
            );
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var particleSetup = EntityManager.GetSharedComponentData<ParticleSetupComponent>(m_ParticleSetup.GetSingletonEntity());
            var surfaceColor = particleSetup.SurfaceColor;
            var exteriorColor = particleSetup.ExteriorColor;
            var interiorColor = particleSetup.InteriorColor;
            var colorStiffness = particleSetup.ColorStiffness;
            var exteriorColorDist = particleSetup.ExteriorColorDist;
            var interiorColorDist = particleSetup.InteriorColorDist;
            
            var data = new NativeArray<float>(1024, Allocator.TempJob);
            data.CopyFrom(AudioManagement.instance.data);
            
            var result = Entities.ForEach((ref RenderColorComponent color, in PositionInDistanceFieldComponent fieldPosition, in Entity en) =>
            {
                
//                Color otherColor = fieldPosition.Distance > 0 ? exteriorColor : interiorColor;
                float distance = fieldPosition.Distance > 0 ? exteriorColorDist : -interiorColorDist;
                float h,s,v;
                if (fieldPosition.Distance>0f)
                {
                    h = Mathf.Abs(data[en.Index % data.Length]);
                    s = 1; 
                    v =(data[en.Index % data.Length] + 1.1f * 10f) / 2;
                } else
                {
                    h = (data[en.Index % data.Length] + 1.1f * 10f) / 2;
                    s = 1;
                    v = (data[en.Index % data.Length] + 1.1f * 10f) / 2;
                }

                Color otherColor = Color.HSVToRGB(h,s,v);
                
                Color targetColor = Color.Lerp(surfaceColor, otherColor, fieldPosition.Distance / distance);
                color.Value = Color.Lerp(color.Value, targetColor, (1 - colorStiffness) / 60f);
            }).Schedule(inputDeps);
            result.Complete();
            data.Dispose();
            return result;
        }
    }
}
