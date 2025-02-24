using System;
using Common;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    [Serializable]
    public struct ProjectileData : IComponentData {
        public float speed;

        [Tooltip("ProjectileData should have either maxDistance or lifeTime set.")]
        public float lifeTime;

        [Tooltip("ProjectileData should have either maxDistance or lifeTime set.")]
        public float maxDistance;

        [HideInInspector] public float currentDistance;
        [HideInInspector] public double spawnTime;
        [HideInInspector] public float3 startPosition;
        [HideInInspector] public float3 direction;

        public bool ShouldDestroy(double currentTime) {
            if (maxDistance == 0 && lifeTime == 0)
                throw new Exception("ProjectileData should have either maxDistance or lifeTime set.");

            return (maxDistance != 0 && currentDistance >= maxDistance) ||
                   (lifeTime != 0 && currentTime - spawnTime >= lifeTime);
        }
    }


    /// <summary>
    /// ProjectileSpawnSystem 会查找场景内所有的ProjectileShootingEvent，然后生成对应的Projectile
    /// </summary>
    public struct ProjectileShootingEvent : IBufferElementData {
        public ProjectileData ProjectileData;
        public DmgSrcComponent DmgSrcComponent;

        [Tooltip("HealthComponent is used to count the number of hits for future destruction.")]
        public HealthComponent HealthComponent;

        public FactionComponent FactionComponent;
        public Entity ProjectilePrefab;
    }
}