using Component;
using Data;
using Unity.Entities;
using UnityEngine;


namespace Authoring {
    public class WeaponAuthoring : MonoBehaviour {
        [SerializeField] WeaponComponent weaponComponent;
        [SerializeField] GameObject projectilePrefab;

        class Baker : Baker<WeaponAuthoring> {
            public override void Bake(WeaponAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                authoring.weaponComponent.Prefabs = new WeaponPrefab {
                    Projectile = GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic)
                };
                AddComponent(entity, authoring.weaponComponent);
            }
        }
    }
}