using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    public enum ProjectileTarget {
        Player,
        Monster
    }


    [Serializable]
    public struct ProjectileData : IComponentData {
        public float speed;
        public float dmg;
        public float lifeTime;
        [HideInInspector] public double spawnTime;
        [HideInInspector] public float3 startPosition;
        [HideInInspector] public float3 direction;
        [HideInInspector] public Entity Shooter;
        [HideInInspector] public ProjectileTarget target;
    }

    public struct ProjectileShootingEvent : IBufferElementData {
        public ProjectileData ProjectileData;
        [HideInInspector] public Entity ProjectilePrefab;
    }
}