using System;
using Common;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    
   
    //伤害来源组件，会在伤害检测时作用于HealthComponent
    [Serializable]
    public struct DmgSrcComponent : IComponentData {
        public float damage;
    }

    public struct DmgEventComponent : IBufferElementData {
        public DmgSrcComponent DmgSrc;
        public float3 HitPoint;
    }

    //TODO 或许该使用sharedComponent去优化内存
    [Serializable]
    [GhostComponent]
    public struct HealthComponent : IComponentData, IEnableableComponent {
        public float maxHealth;

        //todo 这里计数Hit是否有必要 目前只有投射物在用
        public ushort maxHit;
        public float invincibilityTimeBeenHit;//玩家被击中后的无敌时间，怪物一般设置为0
        [HideInInspector] public double invincibilityEndTime;
        [HideInInspector] public ushort hitCounter;
        private float _currentHealth;

        public bool HealthChanged { get; private set; }

        [GhostField]
        public float CurrentHealth {
            get => _currentHealth;
            set {
                _currentHealth = value;
                HealthChanged = true;
            }
        }

        public void UIRefreshed() {
            HealthChanged = false;
        }

        //根基命中次数和生命值判断死亡
        public bool IsDead {
            get {
                if (maxHit > 0) {
                    return hitCounter >= maxHit;
                }

                return CurrentHealth <= 0;
            }
        }

        public void MarkDeath() {
            CurrentHealth = float.MinValue;
            hitCounter = ushort.MaxValue;
        }

        public void Reset() {
            CurrentHealth = maxHealth;
            hitCounter = 0;
        }
    }


    //阵营Enum
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