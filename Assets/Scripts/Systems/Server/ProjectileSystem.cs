using Component;
using Unity.Burst;
using Unity.Core;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;

namespace Systems.Server {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct ProjectileSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<ProjectileData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var job = new ProjectileMovementJob {
                SystemTime = SystemAPI.Time,
            };
            state.Dependency = job.Schedule(state.Dependency);
        }
    }

    /// <summary>
    /// TODO 此处应该改用IJobEntityBatch
    /// </summary>
    [BurstCompile]
    public partial struct ProjectileMovementJob : IJobEntity {
        public TimeData SystemTime;


        private void Execute(ref LocalTransform localPosition,
            ref ProjectileData data, ref HealthComponent healthComponent) {
            var distance = data.speed * SystemTime.DeltaTime;
            //直线移动
            localPosition.Position += data.direction * distance;
            //增加投射物的移动距离
            data.currentDistance += distance;
            if (!data.ShouldDestroy(SystemTime.ElapsedTime)) return;
            //如果超过生命周期，标记销毁投射物
            healthComponent.MarkDeath();
            return;
        }
    }
}