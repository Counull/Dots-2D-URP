using System;
using Common;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    [Serializable]
    public struct DmgSrcComponent : IComponentData {
        public float damage;
        [HideInInspector] public Faction ownerFaction;
    }

    [Serializable]
    public struct HealthComponent : IComponentData {
        public float health;
        public float maxHealth;
        public bool IsDead => health <= 0;

        public void Reset() {
            health = maxHealth;
        }
    }
}