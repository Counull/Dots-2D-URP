using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class SpawnerAuthoring : MonoBehaviour {
        [Header("Player")] [SerializeField] private GameObject playerPrefab;

        [FormerlySerializedAs("weaponPrefab")] [Header("Weapon")] [SerializeField]
        private GameObject[] weaponPrefabs;

        [FormerlySerializedAs("enemyPrefab")] [Header("Enemy")] [SerializeField]
        private GameObject[] enemyPrefabs;

        class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent(entity, new PlayerSpawner() {
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic),
                });


                var enemyBuffer = AddBuffer<EnemyPrefabElement>(entity);
                foreach (var enemyPrefab in authoring.enemyPrefabs) {
                    enemyBuffer.Add(new EnemyPrefabElement()
                        {EnemyPrefab = GetEntity(enemyPrefab, TransformUsageFlags.Dynamic)});
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
    }

    public struct WeaponPrefabElement : IBufferElementData {
        public Entity WeaponPrefab;
    }
}