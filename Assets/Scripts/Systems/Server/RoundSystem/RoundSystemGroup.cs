using Systems.Server.MonsterBehavior;
using Unity.Entities;

namespace Systems.Server.RoundSystem {
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial class RoundSystemGroup : ComponentSystemGroup { }
}