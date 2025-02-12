using Unity.Entities;
using UnityEngine;

namespace Authoring {
    public class SpawnerAuthoring : MonoBehaviour {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;

        class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);
           
                AddComponent(entity, new PlayerSpawner() {
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                });
           
                AddComponent(entity,new EnemySpawner() {
                    EnemyPrefab = GetEntity(authoring.enemyPrefab,TransformUsageFlags.Dynamic)
                });
            }
        }
    }

    public struct PlayerSpawner : IComponentData {
        public Entity PlayerPrefab;
    }

    public struct EnemySpawner : IComponentData {
        public Entity EnemyPrefab;
    }
}