using System;
using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring {
    [DisableAutoCreation]
    public class SpawnerAuthoring : MonoBehaviour {
        [Header("Player")] [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject[] weaponPrefabs;
        [SerializeField] private EnemySpawnerInput[] enemyInput;
        [SerializeField] private Vector2 spawnFiledLB;
        [SerializeField] private Vector2 spawnFiledRT;


        private class Baker : Baker<SpawnerAuthoring> {
            public override void Bake(SpawnerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);

                AddComponent(entity, new SpawnSettings {
                    SpawnFiledLB = authoring.spawnFiledLB,
                    SpawnFiledRT = authoring.spawnFiledRT
                });

                AddComponent(entity, new PlayerSpawner {
                    PlayerPrefab = GetEntity(authoring.playerPrefab, TransformUsageFlags.Dynamic)
                });


                var enemyBuffer = AddBuffer<EnemyPrefabElement>(entity);
                foreach (var input in authoring.enemyInput)
                    enemyBuffer.Add(new EnemyPrefabElement {
                        EnemyPrefab = GetEntity(input.enemyPrefab, TransformUsageFlags.Dynamic),
                        EnemyAttributes = input.enemyAttributes
                    });


                // Add and initialize the DynamicBuffer
                var buffer = AddBuffer<WeaponSlotElement>(entity);
                foreach (var prefab in authoring.weaponPrefabs)
                    buffer.Add(new WeaponSlotElement {
                        WeaponPrefab = GetEntity(prefab, TransformUsageFlags.Dynamic)
                    });
            }
        }
    }


    //用作输入
    [Serializable]
    public struct EnemySpawnerInput {
        public GameObject enemyPrefab;
        public EnemySpawnAttributes enemyAttributes;
    }
}