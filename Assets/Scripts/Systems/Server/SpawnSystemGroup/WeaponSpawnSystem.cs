using Component;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

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
                var weaponMountDistance = playerComponent.ValueRO.InGameAttributes.weaponMountDistance;
                var weaponPerRad = 2 * Mathf.PI / maxWeaponCount;

                var weaponIndex = 0;
                var weaponBuffer = _weaponBuffer[entity];
                foreach (var element in weaponBuffer) {
                    var weapon = ecb.Instantiate(element.WeaponPrefab);
                    ecb.AddComponent(weapon, new Parent {Value = entity});
                    ecb.AddComponent(weapon, FactionComponent.Player);
                    //计算武器的位置

                    var newLocalTrans = state.EntityManager.GetComponentData<LocalTransform>(element.WeaponPrefab);
                    newLocalTrans.Position = new float3(
                        Mathf.Cos(weaponPerRad * weaponIndex) * weaponMountDistance,
                        Mathf.Sin(weaponPerRad * weaponIndex) * weaponMountDistance,
                        0
                    );

                    ecb.SetComponent(weapon, newLocalTrans);
                    weaponIndex++;
                }

                state.EntityManager.SetComponentEnabled<WeaponNeedRefresh>(entity, false);
            }
        }
    }
}