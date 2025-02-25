using System;
using Common;
using Sirenix.OdinInspector;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    
    /// <summary>
    /// 怪物的基础组件
    /// </summary>
    [Serializable]
    public struct MonsterComponent : IComponentData {
        [HideInInspector] public float3 targetPlayerPos;
        [HideInInspector] public float3 targetPlayerDirNormalized;
        [HideInInspector] public float targetDistanceSq;
    }


    /// <summary>
    /// 跟随组件
    /// </summary>
    [Serializable]
    public struct ChaseComponent : IComponentData, IEnableableComponent {
        public float speed;
    }


    
    /// <summary>
    /// 冲锋组件
    /// </summary>
    [Serializable]
    public struct ChargeComponent : IComponentData, IEnableableComponent {
        public float speed;
        public float chargeRange;
        public float chargeTotalTime;
        public CoolDownData coolDownData;
        [HideInInspector] public float3 direction;
    }

    /// <summary>
    /// 发射弹幕组件
    /// </summary>
    [Serializable]
    public struct ShooterComponent : IComponentData, IEnableableComponent {
        public ushort count;
        public float triggerRange;
        public ProjectileData projectileData;
        public DmgSrcComponent dmgSrcComponent;
        public HealthComponent projectileHealth;
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