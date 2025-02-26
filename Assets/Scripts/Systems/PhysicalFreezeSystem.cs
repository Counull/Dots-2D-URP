using Component;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems {
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    [UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
    public partial struct PhysicalFreezeSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<PhysicalShouldFreeze>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            using var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (mass, entity) in
                     SystemAPI.Query<RefRW<PhysicsMass>>().WithAll<PhysicalShouldFreeze>()
                         .WithEntityAccess()) {
                mass.ValueRW.InverseInertia = float3.zero;
                ecb.RemoveComponent<PhysicalShouldFreeze>(entity);
            }

            ecb.Playback(state.EntityManager);
        }
    }
}