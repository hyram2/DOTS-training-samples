﻿using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace GameAI
{
    public struct ShopComponent : IComponentData
    {
        public int2 Position;
    };
}