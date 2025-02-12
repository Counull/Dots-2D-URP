using Authoring;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Client {
    [UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
    public partial struct PlayerMovementSystem : ISystem {
        [BurstCompile]
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<Player>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            foreach (var (playerTransform, input, player) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInput>, RefRO<Player>>()
                         .WithAll<Simulate, GhostOwnerIsLocal>()) {
                if (input.ValueRO is {Horizontal: 0, Vertical: 0}) {
                    continue;
                }

                var move = new float3(input.ValueRO.Horizontal, input.ValueRO.Vertical, 0) * SystemAPI.Time.DeltaTime *
                           player.ValueRO.Speed;
                playerTransform.ValueRW.Position += move;
            }
        }
    }
}