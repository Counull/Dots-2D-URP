using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct ProjectileSpawnSystem : ISystem {
        private BufferLookup<ProjectileShootingEvent> projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileShootingEvent>();
            projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        /// <summary>
        /// /迭代所有的ProjectileShootingEvent，生成对应的Projectile并为其添加组件
        /// </summary>
        /// <param name="state"></param>
        public void OnUpdate(ref SystemState state) {
            projectileShootingEventBuffer.Update(ref state);
            var query = state.GetEntityQuery(ComponentType.ReadOnly<ProjectileShootingEvent>());
            var ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                .CreateCommandBuffer(state.WorldUnmanaged);
            using var entities = query.ToEntityArray(Allocator.Temp);
            foreach (var entity in entities) {
                var buffer = projectileShootingEventBuffer[entity];
                foreach (var projectileEvent in buffer) {
                    // 处理每个ProjectileShootingEvent
                    var projectileEntity = ecb.Instantiate(projectileEvent.ProjectilePrefab);
                    ecb.AddComponent(projectileEntity, projectileEvent.ProjectileData);
                    ecb.AddComponent(projectileEntity, projectileEvent.DmgSrcComponent);
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