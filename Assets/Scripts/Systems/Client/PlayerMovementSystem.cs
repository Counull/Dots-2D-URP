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
            state.RequireForUpdate<PlayerMovement>();
        }


        [BurstCompile]
        public void OnUpdate(ref SystemState state) {
            ;
            var playerMovement = SystemAPI.GetSingleton<PlayerMovement>();
            var speed = SystemAPI.Time.DeltaTime * playerMovement.Speed;

            foreach (var (playerTransform, input) in
                     SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerInput>>()
                         .WithAll<Simulate>()) {
                if (input.ValueRO is {Horizontal: 0, Vertical: 0}) {
                    continue;
                }
                var move = new float3(input.ValueRO.Horizontal, input.ValueRO.Vertical, 0) * speed;
                playerTransform.ValueRW.Position += move;
            }
        }
    }
}