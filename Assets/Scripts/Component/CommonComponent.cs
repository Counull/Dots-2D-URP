using System;
using Common;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    [Serializable]
    public struct DmgSrcComponent : IComponentData {
        public float damage;
    }

    [Serializable]
    public struct HealthComponent : IComponentData, IEnableableComponent {
        public float maxHealth;
        public ushort maxHit;
        public float invincibilityTimeBeenHit;
        [HideInInspector] public double invincibilityEndTime;
        [HideInInspector] public float currentHealth;
        [HideInInspector] public ushort hitCounter;

        public bool IsDead {
            get {
                if (maxHit > 0) {
                    return hitCounter >= maxHit;
                }
                return currentHealth <= 0;
            }
        }

        public void Reset() {
            currentHealth = maxHealth;
        }
    }


    public struct FactionComponent : IComponentData, IEquatable<FactionComponent> {
        public Faction Faction;

        public static FactionComponent Player => new FactionComponent {Faction = Faction.Player};
        public static FactionComponent Monster => new FactionComponent {Faction = Faction.Monster};


        public bool Equals(FactionComponent other) {
            return Faction == other.Faction;
        }

        public override bool Equals(object obj) {
            return obj is FactionComponent other && Equals(other);
        }

        public override int GetHashCode() {
            return (int) Faction;
        }

        public static bool operator ==(FactionComponent left, FactionComponent right) {
            return left.Equals(right);
        }

        public static bool operator !=(FactionComponent left, FactionComponent right) {
            return !(left == right);
        }
    }
}