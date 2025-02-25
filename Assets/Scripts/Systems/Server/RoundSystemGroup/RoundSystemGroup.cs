using Unity.Entities;

namespace Systems.Server.RoundSystemGroup {
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class RoundSystemGroup : ComponentSystemGroup { }
}