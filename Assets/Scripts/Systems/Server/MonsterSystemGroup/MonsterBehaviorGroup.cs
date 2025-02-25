using Unity.Entities;
using Unity.NetCode;

namespace Systems.Server.MonsterSystemGroup {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class MonsterBehaviorGroup : ComponentSystemGroup { }
}