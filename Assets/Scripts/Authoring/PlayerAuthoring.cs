using Common;
using Component;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private PlayerAttributes attributes;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private GameObject playerVisualizationPrefab;

        [SerializeField] private GameObject[] initialWeaponPrefab;

        private class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                authoring.healthComponent.Reset();

                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                AddComponentObject(entity, new PlayerVisualizationComponent {
                    PlayerVisualizationPrefab = authoring.playerVisualizationPrefab
                });
                AddComponent(entity,
                    new PlayerComponent
                        {BaseAttributes = authoring.attributes, InGameAttributes = authoring.attributes});
                AddComponent(entity, authoring.healthComponent);
                AddComponent<PlayerInput>(entity);
                AddComponent(entity, new FactionComponent {Faction = Faction.Player});
                AddComponent<WeaponNeedRefresh>(entity);
                
                var weaponPrefabBuffer = AddBuffer<WeaponSlotElement>(entity);
                foreach (var weaponPrefab in authoring.initialWeaponPrefab) {
                    weaponPrefabBuffer.Add(new WeaponSlotElement() {
                        WeaponPrefab =
                            GetEntity(weaponPrefab, TransformUsageFlags.Dynamic)
                    });
                }
            }
        }
    }
}