using Component;
using Unity.Core;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems.Server {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateBefore(typeof(LocalToWorldSystem))]
    public partial struct ProjectileSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileData>();
        }

        public void OnUpdate(ref SystemState state) {
            var job = new ProjectileMovementJob {
                SystemTime = SystemAPI.Time,
                Ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                    .CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }

    public partial struct ProjectileMovementJob : IJobEntity {
        public TimeData SystemTime;
        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute(Entity entity, [EntityIndexInQuery] int entityInQueryIndex,
            ref LocalTransform localPosition,
            ref ProjectileData data) {
            if (SystemTime.ElapsedTime - data.spawnTime > data.lifeTime) {
                Ecb.DestroyEntity(entityInQueryIndex, entity);
                return;
            }

            localPosition.Position += data.direction * data.speed * SystemTime.DeltaTime;
        }
    }
}