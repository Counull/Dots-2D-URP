using Component;
using Systems.RoundSystem;
using Unity.Entities;
using UnityEngine;

namespace Systems.Server {
    public partial struct CombatRoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<PlayerComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingletonRW<RoundData>();

            //如果当前不是战斗阶段，就不进行任何操作
            if (roundData.ValueRO.Phase != RoundPhase.Combat) return;
            //更新计时
            roundData.ValueRW.CombatTimeCountingDown -= SystemAPI.Time.DeltaTime;
            if (roundData.ValueRO.CombatTimeOut) {
                return;
            }

            CheckRoundFailed(ref state, ref roundData.ValueRW);
        }


        private void CheckRoundFailed(ref SystemState state, ref RoundData roundData) {
            var defeated = true;
            foreach (var player in SystemAPI.Query<RefRO<PlayerComponent>>()) {
                defeated &= player.ValueRO.InGameAttributes.isDead;
            }

            roundData.RoundDefeated = defeated;
        }
    }
}