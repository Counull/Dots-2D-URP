using System;
using Component;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private PlayerAttributes attributes;

        [SerializeField] private GameObject playerVisualizationPrefab;


        class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                AddComponentObject(entity, new PlayerVisualizationComponent() {
                    PlayerVisualizationPrefab = authoring.playerVisualizationPrefab
                });
                AddComponent(entity,
                    new PlayerComponent() {BaseAttributes = authoring.attributes, InGameAttributes = authoring.attributes});
                AddComponent<PlayerInput>(entity);
            }
        }
    }


    
    
 
}