using Aspect;
using Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.MonsterSystemGroup {
    /// <summary>
    ///     TODO 画饼之RVO避障
    ///     怪物更新最近的玩家
    /// </summary>
    [UpdateInGroup(typeof(MonsterBehaviorGroup), OrderLast = true)]
    [UpdateAfter(typeof(SearchingTargetSystem))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct ChaseSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            var monsterQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, MonsterComponent, ChaseComponent>()
                .Build();
            state.RequireForUpdate(monsterQuery);
            state.RequireForUpdate<RoundData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingleton<RoundData>();
            if (roundData.Phase != RoundPhase.Combat) return;
            var deltaTime = SystemAPI.Time.DeltaTime;
            var chaseMoveJob = new ChaseMoveJob {
                DeltaTime = deltaTime
            };
            state.Dependency = chaseMoveJob.Schedule(state.Dependency);
        }
    }


    [BurstCompile]
    public partial struct ChaseMoveJob : IJobEntity {
        public float DeltaTime;

        private void Execute(MonsterAspectWithHealthRW monsterAspect,
            ref ChaseComponent chase) {
            if (monsterAspect.HealthComponent.ValueRO.IsDead) return;
            var playerDir = monsterAspect.Monster.ValueRO.targetPlayerDirNormalized;
            monsterAspect.LocalTransform.ValueRW.Position += playerDir * DeltaTime * chase.speed;
            monsterAspect.LocalTransform.ValueRW.Rotation = quaternion.LookRotationSafe(math.forward(), playerDir);
        }
    }
}