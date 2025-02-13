using Component;
using Unity.Entities;
using UnityEngine;


namespace Authoring {
    public class WeaponAuthoring : MonoBehaviour {
        [SerializeField] WeaponComponent weaponComponent;
        [SerializeField] WeaponPrefabInput prefabs;

        class Baker : Baker<WeaponAuthoring> {
            public override void Bake(WeaponAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                authoring.weaponComponent.Prefabs = new WeaponPrefab {
                    Projectile = GetEntity(authoring.prefabs.projectile, TransformUsageFlags.Dynamic)
                };
                AddComponent(entity, authoring.weaponComponent);
            }
        }
    }
}