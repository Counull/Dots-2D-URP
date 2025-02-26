using Component;
using Unity.Burst;
using Unity.Entities;

namespace Systems.Server {
    /// <summary>
    /// 死亡系统，摧毁掉HealthComponent中标记为死亡的实体
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderLast = true)]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct DeathSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<HealthComponent>();
        }

       
        public void OnUpdate(ref SystemState state) {
            var ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                .CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (healthComponent, entity) in
                     SystemAPI.Query<RefRO<HealthComponent>>().WithEntityAccess()) {
                if (healthComponent.ValueRO.IsDead) {
                    ecb.DestroyEntity(entity);
                }
            }
        }
    }
}