using Systems.Server.RoundSystem;
using Unity.Entities;

namespace Systems.Server.MonsterBehavior {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(RoundSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class MonsterBehaviorGroup : ComponentSystemGroup { }
}