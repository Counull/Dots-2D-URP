using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class SpawnerAuthoring : MonoBehaviour {
        [Header("Player")] [SerializeField] private GameObject playerPrefab;

         [SerializeField]
        private GameObject[] weaponPrefabs;

        [SerializeField] private EnemySpawnerInput[] enemyInput;

        class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent(entity, new PlayerSpawner() {
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                });


                var enemyBuffer = AddBuffer<EnemyPrefabElement>(entity);
                foreach (var input in authoring.enemyInput) {
                    enemyBuffer.Add(new EnemyPrefabElement() {
                        EnemyPrefab = GetEntity(input.enemyPrefab, TransformUsageFlags.Dynamic),
                        EnemyAttributes = input.enemyAttributes
                    });
                }


                // Add and initialize the DynamicBuffer
                var buffer = AddBuffer<WeaponPrefabElement>(entity);
                foreach (var prefab in authoring.weaponPrefabs) {
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

    public struct EnemyPrefabElement : IBufferElementData {
        public Entity EnemyPrefab;
        public EnemySpawnAttributes EnemyAttributes;
    }

    [System.Serializable]
    public struct EnemySpawnerInput {
        public GameObject enemyPrefab;
        public EnemySpawnAttributes enemyAttributes;
    }

    [System.Serializable]
    public struct EnemySpawnAttributes : IComponentData {
        public float maxSpawnCount;
        public ushort groupSpawnRange;
    }

    public struct WeaponPrefabElement : IBufferElementData {
        public Entity WeaponPrefab;
    }
}