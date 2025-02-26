using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.SpawnSystemGroup {
    /// <summary>
    /// 根据场景中的projectileEvent生成对应的Projectile 
    /// </summary>
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ProjectileSpawnSystem : ISystem {
        private BufferLookup<ProjectileShootingEvent> _projectileShootingEventBuffer;
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileShootingEvent>();
            _projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
            _query = state.GetEntityQuery(ComponentType.ReadOnly<ProjectileShootingEvent>());
        }

        /// <summary>
        /// /迭代所有的ProjectileShootingEvent，生成对应的Projectile并为其添加组件
        /// 说实话这里应当包含一个线程池以避免更多的Structural Changes 用ECS节省的性能都在这浪费了
        /// </summary>
        /// <param name="state"></param>
        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            using var entities = _query.ToEntityArray(Allocator.Temp);
            _projectileShootingEventBuffer.Update(ref state);

            foreach (var entity in entities) {
                var buffer = _projectileShootingEventBuffer[entity];

                foreach (var projectileEvent in buffer) {
                    // 处理每个ProjectileShootingEvent
                    var projectileEntity = ecb.Instantiate(projectileEvent.ProjectilePrefab);
                    var prefabTransform =
                        state.EntityManager.GetComponentData<LocalTransform>(projectileEvent.ProjectilePrefab);
                    prefabTransform.Position = projectileEvent.ProjectileData.startPosition;
                    prefabTransform.Rotation = quaternion.LookRotationSafe(math.forward(),
                        projectileEvent.ProjectileData.direction);
                    ecb.AddComponent(projectileEntity, projectileEvent.ProjectileData);
                    ecb.AddComponent(projectileEntity, projectileEvent.DmgSrcComponent);
                    ecb.AddComponent(projectileEntity, projectileEvent.HealthComponent);
                    ecb.AddComponent(projectileEntity, projectileEvent.FactionComponent);
                    ecb.SetComponent(projectileEntity, prefabTransform);
                    ecb.AddBuffer<HitedEntityElement>(projectileEntity);
                }

                buffer.Clear();
            }
        }
    }
}