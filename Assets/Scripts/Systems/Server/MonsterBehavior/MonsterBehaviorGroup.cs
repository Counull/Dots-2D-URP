using Unity.Entities;

namespace Systems.Server.MonsterBehavior {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class MonsterBehaviorGroup : ComponentSystemGroup { }
}