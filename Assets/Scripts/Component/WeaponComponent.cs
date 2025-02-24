using System;
using Common;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


namespace Component {
    //本来打算将武器属性分成不同类型的Component，不过这好像有的过于面向对象而违背ECS的初衷
    [Serializable]
    public struct WeaponComponent : IComponentData {
        public float range;
        public CoolDownData coolDownData;
        public DmgSrcComponent dmgSrcComponent;
    }

    [Serializable]
    public struct WeaponProjectile : IComponentData {
        public HealthComponent projectileHealth;
        public ProjectileData projectileData;
        [HideInInspector] public Entity ProjectilePrefab;
    }


    public enum WeaponType {
        Melee,
        Ranged,
        Projectile
    }
}