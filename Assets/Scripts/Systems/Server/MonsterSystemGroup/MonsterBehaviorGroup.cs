using Systems.Server.RoundSystem;
using Unity.Entities;
using Unity.NetCode;

namespace Systems.Server.MonsterBehavior {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [UpdateAfter(typeof(RoundSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class MonsterBehaviorGroup : ComponentSystemGroup { }
}