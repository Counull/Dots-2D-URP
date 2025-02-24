using Unity.Entities;
using Unity.NetCode;

namespace Systems.Server.WeaponSystemGroup {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class WeaponSystemGroup : ComponentSystemGroup { }
}