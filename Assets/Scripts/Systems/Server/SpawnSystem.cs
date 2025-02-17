using Authoring;
using Component;
using Systems.RoundSystem;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Systems.Server {
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct SpawnSystem : ISystem {
        private BufferLookup<EnemyPrefabElement> _enemyBufferLookup;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SpawnSettings>();
            state.RequireForUpdate<RoundData>();
            _enemyBufferLookup = state.GetBufferLookup<EnemyPrefabElement>();
        }


        /// <summary>
        /// 随机生成敌人，如果这里用ECB可能性能会有提升但是设置敌人的位置需要在物体完全被创建之后所以逻辑会变得复杂
        /// 当然我可以让敌人的prefab最外层不具有任何缩放旋转，但是这绝对不是最好的解决方案
        /// 也可以将生成随机位置的逻辑放到别的系统中
        /// </summary>
        /// <param name="state"></param>
        public void OnUpdate(ref SystemState state) {
            var spawner = SystemAPI.GetSingletonEntity<SpawnSettings>();
            var spawnerData = state.EntityManager.GetComponentData<SpawnSettings>(spawner);

            _enemyBufferLookup.Update(ref state);
            var enemyBuffer = _enemyBufferLookup[spawner];
            for (var i = 0; i < enemyBuffer.Length; i++) {
                var element = enemyBuffer[i];
                // 如果未到下次生成时间则什么都不做
                if (SystemAPI.Time.ElapsedTime < element.EnemyAttributes.nextSpawnTime) continue;

                var center = GenerateRandomRangeCenter(spawnerData, element.EnemyAttributes.groupSpawnRange);
                for (var j = 0; j < element.EnemyAttributes.maxSpawnCount; j++) {
                    var enemy = state.EntityManager.Instantiate(element.EnemyPrefab);
                    // 随机设置敌人的位置
                    var spawnPos = RandomInRange(center, element.EnemyAttributes.groupSpawnRange);
                    var currentTransform = state.EntityManager.GetComponentData<LocalTransform>(enemy);
                    currentTransform.Position = spawnPos;
                    state.EntityManager.SetComponentData(enemy, currentTransform);
                }


                // 设置敌人Spawn的冷却时间
                element.EnemyAttributes.nextSpawnTime =
                    SystemAPI.Time.ElapsedTime + element.EnemyAttributes.spawnInterval;
                enemyBuffer[i] = element;
            }
        }

        public float3 GenerateRandomRangeCenter(in SpawnSettings settings, float enemySpawnRange) {
            return new float3(
                Random.Range(settings.SpawnFiledLB.x + enemySpawnRange, settings.SpawnFiledRT.x - enemySpawnRange),
                Random.Range(settings.SpawnFiledLB.y + enemySpawnRange, settings.SpawnFiledRT.y - enemySpawnRange),
                0);
        }

        public float3 RandomInRange(float3 center, float range) {
            return new float3(
                Random.Range(center.x - range, center.x + range),
                Random.Range(center.y - range, center.y + range),
                0);
        }
    }
}