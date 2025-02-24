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
        public float lifeTime; //TODO: 应该用MaxDistance来代替 写的时候脑抽了
        [HideInInspector] public double spawnTime;
        [HideInInspector] public float3 startPosition;
        [HideInInspector] public float3 direction;
    }


    public struct ProjectileShootingEvent : IBufferElementData {
        public ProjectileData ProjectileData;
        public DmgSrcComponent DmgSrcComponent;

        [Tooltip("HealthComponent is used to count the number of hits for future destruction.")]
        public HealthComponent HealthComponent;

        public FactionComponent FactionComponent;
        public Entity ProjectilePrefab;
    }
}