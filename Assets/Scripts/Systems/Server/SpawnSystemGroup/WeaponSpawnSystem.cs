using Component;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Server.SpawnSystemGroup {
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct WeaponSpawnSystem : ISystem {
        private BufferLookup<WeaponSlotElement> _weaponBuffer;

        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<WeaponNeedRefresh>();
            _weaponBuffer = state.GetBufferLookup<WeaponSlotElement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            _weaponBuffer.Update(ref state);
            var ecb = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (playerComponent, playerEntity) in SystemAPI.Query<RefRO<PlayerComponent>>()
                         .WithAll<WeaponNeedRefresh>()
                         .WithEntityAccess()) {
                var maxWeaponCount = playerComponent.ValueRO.InGameAttributes.maxWeaponCount;
                var weaponMountDistance = playerComponent.ValueRO.InGameAttributes.weaponMountDistance;
                var weaponBuffer = _weaponBuffer[playerEntity];
                ;
                var weaponPerRad = 2 * Mathf.PI / maxWeaponCount;
                var weaponIndex = 0;
                foreach (var element in weaponBuffer) {
                    //计算武器的位置
                    var newLocalTrans = state.EntityManager.GetComponentData<LocalTransform>(element.WeaponPrefab);
                    newLocalTrans.Position = new float3(
                        Mathf.Cos(weaponPerRad * weaponIndex) * weaponMountDistance,
                        Mathf.Sin(weaponPerRad * weaponIndex) * weaponMountDistance,
                        0
                    );
                    var weapon = ecb.Instantiate(element.WeaponPrefab);
                    ecb.AddComponent<GhostChildEntity>(weapon);
                    ecb.AddComponent(weapon, new Parent {Value = playerEntity});
                    ecb.AddComponent(weapon, FactionComponent.Player);
                    ecb.SetComponent(weapon, newLocalTrans);
                    ecb.SetComponent(weapon, new WeaponMounted() {PlayerEntity = playerEntity});
                    ecb.AppendToBuffer(playerEntity, new GhostGroup() {Value = weapon});
                    ecb.AppendToBuffer(playerEntity, new LinkedEntityGroup() {Value = weapon});
                    weaponIndex++;
                }

                state.EntityManager.SetComponentEnabled<WeaponNeedRefresh>(playerEntity, false);
            }
        }
    }
}