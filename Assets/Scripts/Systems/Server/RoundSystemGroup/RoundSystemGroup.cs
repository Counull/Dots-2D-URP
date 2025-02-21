using Unity.Entities;

namespace Systems.Server.RoundSystemGroup {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RoundSystemGroup : ComponentSystemGroup { }
}