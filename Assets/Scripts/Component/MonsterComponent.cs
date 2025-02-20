using System;
using Data;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    [Serializable]
    public struct MonsterComponent : IComponentData {
        public float maxHealth;
        [HideInInspector] public float health;
        [HideInInspector] public float3 targetPlayerPos;
        [HideInInspector] public float targetDistanceSq;
        public bool IsDead => health <= 0;

       public void Reset() {
            health = maxHealth;
        }
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
    public struct ShootComponent : IComponentData, IEnableableComponent {
        public float shootRange;
        public CoolDownData coolDownData;
    }
}