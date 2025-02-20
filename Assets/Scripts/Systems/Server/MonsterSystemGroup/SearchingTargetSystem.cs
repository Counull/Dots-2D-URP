using Component;
using Systems.Server.RoundSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.MonsterBehavior {
    /// <summary>
    /// TODO 画饼之RVO避障
    /// 怪物更新最近的玩家
    /// </summary>
    [UpdateInGroup(typeof(MonsterBehaviorGroup))]
    public partial struct SearchingTargetSystem : ISystem {
        EntityQuery _monsterQuery;
        EntityQuery _targetQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            _monsterQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, MonsterComponent>()
                .Build();
            _targetQuery = SystemAPI.QueryBuilder().WithAll<PlayerComponent, LocalTransform>().Build();
            state.RequireForUpdate(_monsterQuery);
            state.RequireForUpdate(_targetQuery);
            state.RequireForUpdate<RoundData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingletonRW<RoundData>();
            if (roundData.ValueRO.Phase != RoundPhase.Combat) return;

            var targetTransforms =
                _targetQuery.ToComponentDataListAsync<LocalTransform>(state.WorldUnmanaged.UpdateAllocator.ToAllocator,
                    out var targetHandle);

            var searchingTargetJob = new SearchingTargetJob {
                TargetTransforms = targetTransforms,
            };
            state.Dependency.Complete();
            state.Dependency = searchingTargetJob.ScheduleParallel(targetHandle);
            state.Dependency.Complete();
        }
    }

    /// <summary>
    /// 跟踪最近的玩家的行为
    /// </summary>
    [BurstCompile]
    public partial struct SearchingTargetJob : IJobEntity {
        [ReadOnly] public NativeList<LocalTransform> TargetTransforms;


        private void Execute(MonsterAspect monsterAspect) {
            var monsterTransform = monsterAspect.Transform.ValueRO;

            // 寻找最近的目标 随便写的临时代码 之后会用RVO或者碰撞检测
            // 说不定用碰撞检测会有BVH之类的优化性能会更好
            var nearestTarget = TargetTransforms[0];
            var nearestDistanceSq = math.distancesq(monsterTransform.Position, nearestTarget.Position);
            for (int i = 1; i < TargetTransforms.Length; i++) {
                var target = TargetTransforms[i];
                var newDistanceSq = math.distancesq(monsterTransform.Position, target.Position);
                if (!(newDistanceSq < nearestDistanceSq)) continue;
                nearestTarget = target;
                nearestDistanceSq = newDistanceSq;
            }

            monsterAspect.Monster.ValueRW.targetPlayerPos = nearestTarget.Position;
            monsterAspect.Monster.ValueRW.targetDistanceSq = nearestDistanceSq;
            ;
        }
    }


    public readonly partial struct MonsterAspect : IAspect {
        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRW<MonsterComponent> Monster;
    }
}