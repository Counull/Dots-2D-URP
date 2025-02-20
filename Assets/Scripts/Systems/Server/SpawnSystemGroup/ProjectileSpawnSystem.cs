using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server {
    public partial struct ProjectileSpawnSystem : ISystem {
        BufferLookup<ProjectileShootingEvent> _projectileShootingEventBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileShootingEvent>();
            _projectileShootingEventBuffer = state.GetBufferLookup<ProjectileShootingEvent>();
        }

        public void OnUpdate(ref SystemState state) {
            _projectileShootingEventBuffer.Update(ref state);
            var query = state.GetEntityQuery(ComponentType.ReadOnly<ProjectileShootingEvent>());
            using var entities = query.ToEntityArray(Allocator.Temp);
            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var entity in entities) {
                var buffer = _projectileShootingEventBuffer[entity];
                foreach (var projectileEvent in buffer) {
                    // 处理每个ProjectileShootingEvent
                    var projectileEntity = ecb.Instantiate(projectileEvent.ProjectilePrefab);

                    ecb.SetComponent(projectileEntity,
                        new LocalTransform() {
                            Scale = 1f,
                            Position = projectileEvent.ProjectileData.startPosition +
                                       projectileEvent.ProjectileData.direction,
                            Rotation =
                                quaternion.LookRotationSafe(new float3(0, 0, 1),
                                    projectileEvent.ProjectileData.direction),
                        });
                }

                buffer.Clear();
            }

            ecb.Playback(state.EntityManager);
        }
    }
}