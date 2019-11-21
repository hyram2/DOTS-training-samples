﻿using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

[UpdateInGroup((typeof(InitializationSystemGroup)))]
[UpdateAfter(typeof(AntSpawningSystem))]
[UpdateBefore(typeof(AntPostInitializationSystem))]
public class AntInitializationSystem : JobComponentSystem
{
    EntityQuery m_MapSettingsQuery;
    
    protected override void OnCreate()
    {
        base.OnCreate();
        m_MapSettingsQuery = GetEntityQuery(ComponentType.ReadOnly<MapSettingsComponent>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        return new Job
        {
            Seed = 1 + (uint)Time.frameCount,
            MapSize = m_MapSettingsQuery.GetSingleton<MapSettingsComponent>().MapSize,
        }.Schedule(this, inputDeps);
    }

    [RequireComponentTag(typeof(UninitializedTagComponent))]
    struct Job : IJobForEachWithEntity<BrightnessComponent, FacingAngleComponent, PositionComponent, RandomSteeringComponent>
    {
        public uint Seed;
        public int MapSize;

        public void Execute(Entity entity, int index, ref BrightnessComponent brightness, ref FacingAngleComponent facingAngle, ref PositionComponent position, ref RandomSteeringComponent random)
        {
            var rng = new Random(((uint)index + 1) * Seed * 100151);
            facingAngle.Value = rng.NextFloat() * 2 * math.PI;
            brightness.Value = rng.NextFloat(0.75f, 1.25f);
            position.Value = .5f * MapSize + new float2(rng.NextFloat(-5, 5), rng.NextFloat(-5, 5));
            random.Rng = rng;
        }
    }
}