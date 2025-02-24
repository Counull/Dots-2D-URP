using Component;
using Unity.Entities;
using Unity.Transforms;

namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct WeaponSpawnSystem : ISystem {
        private BufferLookup<WeaponSlotElement> _weaponBuffer;

        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<WeaponNeedRefresh>();
            _weaponBuffer = state.GetBufferLookup<WeaponSlotElement>();
        }

        public void OnUpdate(ref SystemState state) {
            _weaponBuffer.Update(ref state);
            var ecb = SystemAPI.GetSingletonRW<BeginSimulationEntityCommandBufferSystem.Singleton>().ValueRW
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (playerComponent, entity) in SystemAPI.Query<RefRO<PlayerComponent>>()
                         .WithAll<WeaponNeedRefresh>()
                         .WithEntityAccess()) {
                var maxWeaponCount = playerComponent.ValueRO.InGameAttributes.maxWeaponCount;
                var weaponIndex = 0;
                var weaponBuffer = _weaponBuffer[entity];
                foreach (var element in weaponBuffer) {
                    var weapon = ecb.Instantiate(element.WeaponPrefab);
                    ecb.AddComponent(weapon, new Parent {Value = entity});
                    weaponIndex++;
                }

                state.EntityManager.SetComponentEnabled<WeaponNeedRefresh>(entity, false);
            }
        }
    }
}