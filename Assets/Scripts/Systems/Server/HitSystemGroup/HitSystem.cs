using System.Linq;
using Common;
using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Server.HitSystemGroup {
    /// <summary>
    /// 用于检测碰撞 暂时在这里处理伤害计算但最终伤害计算会被分发到各个其他系统中
    /// </summary>
    [UpdateInGroup(typeof(HitSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct HitSystem : ISystem {
        ComponentLookup<HealthComponent> _healthLookup;
        ComponentLookup<FactionComponent> _factionLookup;
        ComponentLookup<DmgSrcComponent> _dmgSrcLookup;
        ComponentLookup<ProjectileData> _projectileLookup;
        BufferLookup<HitedEntityElement> _hitedEntityBufferLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<FactionComponent>();

            _healthLookup = state.GetComponentLookup<HealthComponent>();
            _factionLookup = state.GetComponentLookup<FactionComponent>(true);
            _dmgSrcLookup = state.GetComponentLookup<DmgSrcComponent>(true);
            _projectileLookup = state.GetComponentLookup<ProjectileData>(true);
            _hitedEntityBufferLookup = state.GetBufferLookup<HitedEntityElement>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingleton<RoundData>();
            if (roundData.Phase != RoundPhase.Combat) return;


            //丑陋
            _healthLookup.Update(ref state);
            _factionLookup.Update(ref state);
            _dmgSrcLookup.Update(ref state);
            _projectileLookup.Update(ref state);

            var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
            var ss = SystemAPI.GetSingleton<SimulationSingleton>();
            sim.FinalJobHandle.Complete();

            _hitedEntityBufferLookup.Update(ref state);
            var hitJob = new HitJob {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                HealthLookup = _healthLookup,
                FactionLookup = _factionLookup,
                DmgSrcLookup = _dmgSrcLookup,
                ProjectileLookup = _projectileLookup,
                HitedEntityBufferLookup = _hitedEntityBufferLookup,
                Ecb = SystemAPI.GetSingletonRW<EndSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                    .CreateCommandBuffer(state.WorldUnmanaged)
            };
            state.Dependency = hitJob.Schedule(ss, state.Dependency);
        }
    }

    [BurstCompile]
    internal struct HitJob : ITriggerEventsJob {
        public double ElapsedTime;
        public EntityCommandBuffer Ecb;
        public ComponentLookup<HealthComponent> HealthLookup;
        [ReadOnly] public ComponentLookup<FactionComponent> FactionLookup;
        [ReadOnly] public ComponentLookup<DmgSrcComponent> DmgSrcLookup;
        [ReadOnly] public ComponentLookup<ProjectileData> ProjectileLookup;
        [ReadOnly] public BufferLookup<HitedEntityElement> HitedEntityBufferLookup;


        public void Execute(TriggerEvent triggerEvent) {
            if (!FactionLookup.TryGetComponent(triggerEvent.EntityA, out var factionA)
                || !FactionLookup.TryGetComponent(triggerEvent.EntityB, out var factionB)) {
                return;
            }

            if (factionA == factionB) return;

            //todo 如果未来有更复杂伤害计算将不在这里处理 此处预定改为dmgEvent然后分发到Buffer中
            BeenHit(triggerEvent.EntityA, triggerEvent.EntityB, ElapsedTime);
            BeenHit(triggerEvent.EntityB, triggerEvent.EntityA, ElapsedTime);
        }

        private bool BeenHit(Entity healthEntity, Entity dmgSrcEntity, double currentTime) {
            if (!HealthLookup.TryGetComponent(healthEntity, out var health) ||
                !DmgSrcLookup.TryGetComponent(dmgSrcEntity, out var dmgSrc)) return false;

            if (currentTime < health.invincibilityEndTime) return false;


            if (!CheckProjectileHited(healthEntity, dmgSrcEntity)) return false;

            health.CurrentHealth -= dmgSrc.damage;
            health.hitCounter++;
            health.invincibilityEndTime = currentTime + health.invincibilityTimeBeenHit;
            HealthLookup[healthEntity] = health;


            return false;
        }

        /// <summary>
        /// 检测是否为投射物且是否投射物是否命中过该物体
        /// 这里的这个函数的语义处理的不好，其实对投射物的伤害检测或是命中检测应该另写个投射物系统处理
        /// TODO 对于伤害计算和处理应当将伤害事件塞入buffer中，然后在具体的伤害系统中处理
        /// </summary>
        /// <param name="healthEntity"></param>
        /// <param name="dmgSrcEntity"></param>
        /// <returns>是否成功造成伤害需要后续伤害计算</returns>
        bool CheckProjectileHited(Entity healthEntity, Entity dmgSrcEntity) {
            if (!ProjectileLookup.HasComponent(dmgSrcEntity)
                || !HealthLookup.TryGetComponent(dmgSrcEntity, out var projectileHealth)
                || !HitedEntityBufferLookup.TryGetBuffer(healthEntity, out var hitedEntityBuffer))
                return true; //如果该物体不适投射物则不需要后续判断直接计算伤害

            foreach (var hitedEntityElement in hitedEntityBuffer) {
                if (hitedEntityElement.hitedEntity == healthEntity) {
                    return false; //如果投射物已经命中过该物体则返回不需要后续的伤害计算
                }
            }

            //投生物成功命中物体 则增加命中次数
            projectileHealth.hitCounter++;
            HealthLookup[dmgSrcEntity] = projectileHealth;
            Ecb.AppendToBuffer(dmgSrcEntity, new HitedEntityElement {hitedEntity = healthEntity});
            return true; //计算后续伤害
        }
    }
}