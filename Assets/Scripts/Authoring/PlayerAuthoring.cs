using Common;
using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private PlayerAttributes attributes;
        [SerializeField] private HealthComponent healthComponent;
        [SerializeField] private GameObject playerVisualizationPrefab;


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
            }
        }
    }
}