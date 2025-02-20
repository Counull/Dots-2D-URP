using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Component {
    //本来打算将武器属性分成不同类型的Component，不过这好像有的过于面向对象而违背ECS的初衷
    [Serializable]
    public struct WeaponComponent : IComponentData {
        public float baseDamage;
        public float range;
        public float fireInterval;
        public WeaponType weaponType;
        public float projectileSpeed;
        public WeaponPrefab Prefabs;
        [HideInInspector] public ulong NextFireTime { get; private set; }
    }


    public struct WeaponPrefab {
        public Entity Projectile;
    }



    public enum WeaponType {
        Melee,
        Ranged,
        Projectile
    }
}