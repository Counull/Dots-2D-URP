using System;
using Common;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    [Serializable]
    public struct MonsterComponent : IComponentData {
        [HideInInspector] public float3 targetPlayerPos;
        [HideInInspector] public float3 targetPlayerDirNormalized;
        [HideInInspector] public float targetDistanceSq;
    }


    [Serializable]
    public struct ChaseComponent : IComponentData, IEnableableComponent {
        public float speed;
    }


    [Serializable]
    public struct ChargeComponent : IComponentData, IEnableableComponent {
        public float speed;
        public float chargeRange;
        public float chargeTotalTime;
        public CoolDownData coolDownData;
        [HideInInspector] public float3 direction;
    }

    [Serializable]
    public struct ShooterComponent : IComponentData, IEnableableComponent {
        public ushort count;

        public float triggerRange;
        public ProjectileData projectileData;
        public DmgSrcComponent dmgSrcComponent;
        public CoolDownData coolDownData;
        [HideInInspector] public float spreadAngleRad;
        [HideInInspector] public Entity ProjectilePrefab;

        [ShowInInspector]
        public float SpreadAngleDeg {
            get => math.degrees(spreadAngleRad);
            set => spreadAngleRad = math.radians(value);
        }
    }
}