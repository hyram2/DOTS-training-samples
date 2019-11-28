using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(UpdateParticleVelocitySystem))]
    public class LocalToWorldSystem : JobComponentSystem
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
            var data = new NativeArray<float>(1024, Allocator.TempJob);
            data.CopyFrom(AudioManagement.instance.data);
            var modifier = AudioManagement.instance.modifier;
            var particleSetup = EntityManager.GetSharedComponentData<ParticleSetupComponent>(m_ParticleSetup.GetSingletonEntity());
            var speedStretch = particleSetup.SpeedStretch;
            var result = Entities.ForEach((ref LocalToWorldComponent localToWorld, in PositionComponent position, in VelocityComponent velocity,in Entity en) =>
            {
                float speed = math.length(velocity.Value);
                quaternion rotation = MathHelpers.LookRotationWithUp(velocity.Value / speed);
                float3 scale = new float3(.1f, .01f, math.max(.1f, speed * speedStretch));
                scale *= (data[en.Index % data.Length]*modifier * 12f);
                localToWorld.Value = float4x4.TRS(position.Value, rotation, scale);
            }).Schedule(inputDeps);
            result.Complete();
            data.Dispose();
            return result;
        }
    }
}
