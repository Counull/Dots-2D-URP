using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Component {
    [Serializable]
    public struct ProjectileData : IComponentData {
        public float speed;
        public float dmg;
        public float lifeTime;
        [HideInInspector] public float3 startPosition;
        [HideInInspector] public float3 direction;
        [HideInInspector] public Entity Shooter;
    }

    public struct ProjectileShootingEvent : IBufferElementData {
        public ProjectileData ProjectileData;
        [HideInInspector] public Entity ProjectilePrefab;
    }
}