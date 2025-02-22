using Common;
using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Server.HitSystemGroup {
    [UpdateInGroup(typeof(HitSystemGroup))]
    public partial struct HitSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<FactionComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
            var ss = SystemAPI.GetSingleton<SimulationSingleton>();
            sim.FinalJobHandle.Complete();
            var roundData = SystemAPI.GetSingleton<RoundData>();
            if (roundData.Phase != RoundPhase.Combat) return;

            var hitJob = new HitJob {
                ElapsedTime = SystemAPI.Time.ElapsedTime,
                HealthLookup = state.GetComponentLookup<HealthComponent>(),
                FactionLookup = state.GetComponentLookup<FactionComponent>(true),
                DmgSrcLookup = state.GetComponentLookup<DmgSrcComponent>(true),
                ProjectileLookup = state.GetComponentLookup<ProjectileData>(true),
            };
            state.Dependency = hitJob.Schedule(ss, state.Dependency);
        }
    }

    [BurstCompile]
    internal struct HitJob : ITriggerEventsJob {
        public double ElapsedTime;
        public ComponentLookup<HealthComponent> HealthLookup;
        [ReadOnly] public ComponentLookup<FactionComponent> FactionLookup;
        [ReadOnly] public ComponentLookup<DmgSrcComponent> DmgSrcLookup;
        [ReadOnly] public ComponentLookup<ProjectileData> ProjectileLookup;

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
            health.currentHealth -= dmgSrc.damage;
            health.hitCounter++;
            health.invincibilityEndTime = currentTime + health.invincibilityTimeBeenHit;
            HealthLookup[healthEntity] = health;


            //在这里计算子弹的命中次数属实啰嗦
            if (!ProjectileLookup.HasComponent(dmgSrcEntity)
                || !HealthLookup.TryGetComponent(dmgSrcEntity, out var projectileHealth)) return true;
            projectileHealth.hitCounter++;
            HealthLookup[dmgSrcEntity] = projectileHealth;
            return true;
        }
    }
}