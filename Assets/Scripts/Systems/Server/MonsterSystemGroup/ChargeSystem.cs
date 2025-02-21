using Component;
using Systems.Server.RoundSystemGroup;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems.Server.MonsterSystemGroup {
    [UpdateInGroup(typeof(MonsterBehaviorGroup))]
    [UpdateAfter(typeof(SearchingTargetSystem))]
    public partial struct ChargeSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            var monsterQuery = SystemAPI.QueryBuilder()
                .WithAll<LocalTransform, ChargeComponent, MonsterComponent>()
                .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState).Build();
            state.RequireForUpdate(monsterQuery);
            state.RequireForUpdate<RoundData>();
        }


        /// <summary>
        ///     暂时是否使用异步只是基于可能激活此系统的怪物数量
        /// </summary>
        /// <param name="state"></param>
        public void OnUpdate(ref SystemState state) {
            foreach (var (localTransform, chargeComponent
                         , monsterComponent, entity)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRW<ChargeComponent>,
                             RefRO<MonsterComponent>>()
                         .WithOptions(EntityQueryOptions.IgnoreComponentEnabledState)
                         .WithEntityAccess()) {
                if (monsterComponent.ValueRO.IsDead) continue; //怪物死亡

                ref var chargeDataRW = ref chargeComponent.ValueRW;
                var charging = state.EntityManager.IsComponentEnabled<ChargeComponent>(entity);
                ref var cd = ref chargeDataRW.coolDownData;
                if (!charging) {
                    if (cd.TriggerTime == 0) {
                        //未触发过冷则初始化冷却时间
                        cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime);
                        continue;
                    }

                    if (!cd.IsCoolDownReady(SystemAPI.Time.ElapsedTime)) continue; //冷却时间未到
                    var rangeSq = chargeDataRW.chargeRange * chargeDataRW.chargeRange;
                    if (monsterComponent.ValueRO.targetDistanceSq > rangeSq) continue; //超出范围

                    //如果没有激活冲锋则初始化冲锋数据
                    cd.TriggerCoolDown(SystemAPI.Time.ElapsedTime); //触发冷却
                    var targetDir = monsterComponent.ValueRO.targetPlayerPos - localTransform.ValueRO.Position;
                    chargeDataRW.direction = math.normalize(targetDir); //这里的normalize其实是可以被优化的节省一个dot的计算量
                    EnableCharge(ref state, entity, true);
                }

                //冲锋状态移动
                localTransform.ValueRW.Position +=
                    chargeDataRW.direction * chargeDataRW.speed * SystemAPI.Time.DeltaTime;

                //如果未超过冲锋时间则不进行处理
                if (SystemAPI.Time.ElapsedTime - cd.TriggerTime < chargeDataRW.chargeTotalTime) continue;
                //超过冲锋时间则停止冲锋
                EnableCharge(ref state, entity, false);
            }
        }


        private void EnableCharge(ref SystemState state, Entity entity, bool enable) {
            state.EntityManager.SetComponentEnabled<ChargeComponent>(entity, enable);
            state.EntityManager.SetComponentEnabled<ChaseComponent>(entity, !enable);
        }
    }
}