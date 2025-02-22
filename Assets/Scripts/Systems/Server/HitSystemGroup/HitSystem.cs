using Common;
using Component;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Server.HitSystemGroup {
    [UpdateInGroup(typeof(HitSystemGroup))]
    public partial struct HitSystem : ISystem {
        ComponentLookup<FactionComponent> factionLookup;
        ComponentLookup<HealthComponent> healthLookup;
        ComponentLookup<DmgSrcComponent> dmgSrcLookup;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<FactionComponent>();
            factionLookup = state.GetComponentLookup<FactionComponent>(true);
            healthLookup = state.GetComponentLookup<HealthComponent>();
            dmgSrcLookup = state.GetComponentLookup<DmgSrcComponent>(true);
        }

        public void OnUpdate(ref SystemState state) {
            var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
            sim.FinalJobHandle.Complete();
            factionLookup.Update(ref state);
            healthLookup.Update(ref state);
            dmgSrcLookup.Update(ref state);
            foreach (var triggerEvent in sim.TriggerEvents) {
                if (!factionLookup.TryGetComponent(triggerEvent.EntityA, out var factionA)
                    || !factionLookup.TryGetComponent(triggerEvent.EntityB, out var factionB)) {
                    continue;
                }

                if (factionA == factionB) continue;


                if (!healthLookup.TryGetComponent(triggerEvent.EntityA, out var healthA)
                    || !healthLookup.TryGetComponent(triggerEvent.EntityB, out var healthB)) {
                    continue;
                }


                if (dmgSrcLookup.TryGetComponent(triggerEvent.EntityA, out var dmgSrcA)) {
                    healthB.currentHealth -= dmgSrcA.damage;
                }

                if (dmgSrcLookup.TryGetComponent(triggerEvent.EntityB, out var dmgSrcB)) {
                    healthA.currentHealth -= dmgSrcB.damage;
                }

                healthA.hitCounter++;
                healthB.hitCounter++;
                healthLookup[triggerEvent.EntityA] = healthA;
                healthLookup[triggerEvent.EntityB] = healthB;
            }
        }
    }
}