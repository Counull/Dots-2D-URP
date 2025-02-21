using Unity.Entities;
using Unity.NetCode;


namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
            [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class SpawnSystemGroup : ComponentSystemGroup { }
}