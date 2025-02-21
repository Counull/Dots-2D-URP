using System;
using Common;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Component {
    [Serializable]
    public struct ProjectileData : IComponentData {
        public float speed;
        public float lifeTime;
        [HideInInspector] public double spawnTime;
        [HideInInspector] public float3 startPosition;
        [HideInInspector] public float3 direction;
   
    }

    public struct ProjectileShootingEvent : IBufferElementData {
        public ProjectileData ProjectileData;
        public DmgSrcComponent DmgSrcComponent;
        public Entity ProjectilePrefab;
    }
}