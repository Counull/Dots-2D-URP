using Unity.Entities;
using Unity.Physics.Systems;

namespace Systems.Server.HitSystemGroup {
    [UpdateInGroup(typeof(AfterPhysicsSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class HitSystemGroup : ComponentSystemGroup { }
}