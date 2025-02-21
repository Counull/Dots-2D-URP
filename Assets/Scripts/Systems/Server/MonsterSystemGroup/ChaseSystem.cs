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
            foreach (var (transform, chase, monsterComponent) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<ChaseComponent>, RefRO<MonsterComponent>>()
                    ) {
                if (monsterComponent.ValueRO.IsDead) continue;
                transform.ValueRW.Position +=
                    math.normalize(monsterComponent.ValueRO.targetPlayerPos - transform.ValueRO.Position) *
                    deltaTime *
                    chase.ValueRO.speed;
            }
        }
    }
}