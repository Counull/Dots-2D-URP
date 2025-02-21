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
            var roundData = SystemAPI.GetSingletonRW<RoundData>();
            if (roundData.ValueRO.Phase != RoundPhase.Combat) return;
            var deltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (monsterAspect, chase) in
                     SystemAPI.Query<MonsterAspectWithHealthRW, RefRO<ChaseComponent>>()
                    ) {
                if (monsterAspect.HealthComponent.ValueRO.IsDead) continue;

                //移动怪物位置
                monsterAspect.LocalTransform.ValueRW.Position +=
                    math.normalize(monsterAspect.Monster.ValueRO.targetPlayerPos -
                                   monsterAspect.LocalTransform.ValueRO.Position) *
                    deltaTime * chase.ValueRO.speed;
            }
        }
    }
}