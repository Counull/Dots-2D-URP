using Component;
using Systems.Server.RoundSystem;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.MonsterBehavior {
    /// <summary>
    /// TODO 画饼之RVO避障
    /// </summary>
    public partial struct ChaseSystem : ISystem {
        EntityQuery _monsterQuery;
        EntityQuery _targetQuery;

        public void OnCreate(ref SystemState state) {
            _monsterQuery = SystemAPI.QueryBuilder().WithAll<LocalTransform, ChaseComponent, MonsterComponent>()
                .Build();
            _targetQuery = SystemAPI.QueryBuilder().WithAll<PlayerComponent, LocalTransform>().Build();
            state.RequireForUpdate(_monsterQuery);
            state.RequireForUpdate(_targetQuery);
            state.RequireForUpdate<RoundData>();
        }

        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingletonRW<RoundData>();
            if (roundData.ValueRO.Phase != RoundPhase.Combat) return;

            var targetTransforms =
                _targetQuery.ToComponentDataListAsync<LocalTransform>(state.World.UpdateAllocator.ToAllocator,
                    out var targetHandle);

            var chaseJob = new ChaseJob {
                TargetTransforms = targetTransforms,
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency.Complete();
            state.Dependency = chaseJob.ScheduleParallel(targetHandle);
            state.Dependency.Complete();
            
            
        }
    }


    public partial struct ChaseJob : IJobEntity {
        [ReadOnly] public NativeList<LocalTransform> TargetTransforms;
        public float DeltaTime;

        private void Execute(MonsterAspect monsterAspect) {
            var monsterTransform = monsterAspect.Transform.ValueRO;

            // 寻找最近的目标
            var nearestTarget = TargetTransforms[0];
            var nearestDistanceSq = math.distancesq(monsterTransform.Position, nearestTarget.Position);
            for (int i = 1; i < TargetTransforms.Length; i++) {
                var target = TargetTransforms[i];
                var newDistanceSq = math.distancesq(monsterTransform.Position, target.Position);
                if (!(newDistanceSq < nearestDistanceSq)) continue;
                nearestTarget = target;
                nearestDistanceSq = newDistanceSq;
            }

            // 更新怪物位置 在多线程环境下这里并不安全
            monsterAspect.Transform.ValueRW = new LocalTransform {
                Position = monsterTransform.Position +
                           math.normalize(nearestTarget.Position - monsterTransform.Position) * DeltaTime *
                           monsterAspect.Chase.ValueRO.Speed,
                Rotation = monsterTransform.Rotation,
                Scale = monsterTransform.Scale
            };
            
        }
    }

    public readonly partial struct MonsterAspect : IAspect {
        public readonly RefRW<LocalTransform> Transform;
        public readonly RefRO<ChaseComponent> Chase;
        public readonly RefRO<MonsterComponent> Monster;
    }
}