using Unity.Entities;
using UnityEngine;

namespace Authoring {
    public class SpawnerAuthoring : MonoBehaviour {
        [Header("Player")] [SerializeField] private GameObject playerPrefab;

        [Header("Weapon")] [SerializeField] private GameObject[] weaponPrefab;

        [Header("Enemy")] [SerializeField] private GameObject enemyPrefab;

        class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent(entity, new PlayerSpawner() {
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                });

                AddComponent(entity, new EnemySpawner() {
                    EnemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic)
                });

                // Add and initialize the DynamicBuffer
                var buffer = AddBuffer<WeaponPrefabElement>(entity);
                foreach (var prefab in authoring.weaponPrefab) {
                    buffer.Add(new WeaponPrefabElement {
                        WeaponPrefab = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });
                }
               
            }
        }
    }

    public struct PlayerSpawner : IComponentData {
        public Entity PlayerPrefab;
    }

    public struct EnemySpawner : IComponentData {
        public Entity EnemyPrefab;
    }

    public struct WeaponPrefabElement : IBufferElementData {
        public Entity WeaponPrefab;
    }
}