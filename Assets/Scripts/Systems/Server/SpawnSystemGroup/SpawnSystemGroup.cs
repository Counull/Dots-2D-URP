using Unity.Entities;
using Unity.NetCode;

namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup), OrderFirst = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class SpawnSystemGroup : ComponentSystemGroup { }
}