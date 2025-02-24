using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct ProjectileSpawnSystem : ISystem {
        private BufferLookup<ProjectileShootingEvent> _projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileShootingEvent>();
            _projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        /// <summary>
        /// /迭代所有的ProjectileShootingEvent，生成对应的Projectile并为其添加组件
        /// 说实话这里应当包含一个线程池以避免更多的Structural Changes 用ECS节省的性能都在这浪费了
        /// </summary>
        /// <param name="state"></param>
        public void OnUpdate(ref SystemState state) {
            var query = state.GetEntityQuery(ComponentType.ReadOnly<ProjectileShootingEvent>());
            var ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                .CreateCommandBuffer(state.WorldUnmanaged);
            using var entities = query.ToEntityArray(Allocator.Temp);
            state.Dependency.Complete();
            _projectileShootingEventBuffer.Update(ref state);
            foreach (var entity in entities) {
                var buffer = _projectileShootingEventBuffer[entity];

                foreach (var projectileEvent in buffer) {
                    // 处理每个ProjectileShootingEvent
                    var projectileEntity = ecb.Instantiate(projectileEvent.ProjectilePrefab);
                    ecb.AddComponent(projectileEntity, projectileEvent.ProjectileData);
                    ecb.AddComponent(projectileEntity, projectileEvent.DmgSrcComponent);
                    ecb.AddComponent(projectileEntity, projectileEvent.HealthComponent);
                    ecb.AddComponent(projectileEntity, projectileEvent.FactionComponent);
                    ecb.SetComponent(projectileEntity, new LocalTransform {
                        Scale = 1f,
                        Position = projectileEvent.ProjectileData.startPosition,
                        Rotation =
                            quaternion.LookRotationSafe(new float3(0, 0, 1),
                                projectileEvent.ProjectileData.direction)
                    });
                }

                buffer.Clear();
            }
        }
    }
}