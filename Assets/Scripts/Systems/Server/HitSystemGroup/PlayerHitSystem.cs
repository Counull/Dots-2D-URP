using Component;

using Systems.Server.RoundSystemGroup;
using Unity.Entities;
using Unity.Physics;

namespace Systems.Server.HitSystemGroup {
    [UpdateInGroup(typeof(HitSystemGroup))]
    public partial struct PlayerHitSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<SimulationSingleton>();
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<PlayerComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            var sim = SystemAPI.GetSingleton<SimulationSingleton>().AsSimulation();
            sim.FinalJobHandle.Complete();
            foreach (var triggerEvent in sim.TriggerEvents) { }
        }
    }
}