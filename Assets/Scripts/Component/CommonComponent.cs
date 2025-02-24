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

        //todo 这里计数Hit是否有必要 目前只有投射物在用
        public ushort maxHit;
        public float invincibilityTimeBeenHit;
        [HideInInspector] public double invincibilityEndTime;
        [HideInInspector] public float currentHealth;
        [HideInInspector] public ushort hitCounter;

        //根基命中次数和生命值判断死亡
        public bool IsDead {
            get {
                if (maxHit > 0) {
                    return hitCounter >= maxHit;
                }

                return currentHealth <= 0;
            }
        }

        public void MarkDeath() {
            currentHealth = float.MinValue;
            hitCounter = ushort.MaxValue;
        }

        public void Reset() {
            currentHealth = maxHealth;
            hitCounter = 0;
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