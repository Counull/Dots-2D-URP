using Component;
using Systems.RoundSystem;
using Unity.Entities;

namespace Systems.Server {
    public partial struct CombatRoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
        }

        public void OnUpdate(ref SystemState state) {
            //  或许这里依靠RequireForUpdate会更好？
            var roundData = state.EntityManager.GetComponentObject<RoundData>(state.SystemHandle);
            //如果当前不是战斗阶段，就不进行任何操作
            if (!roundData.IsCombatPhase) return;

            roundData.CombatTimeCountingDown -= SystemAPI.Time.DeltaTime;
            if (roundData.CombatTimeOut) {
                return;
            }

            CheckRoundFailed(ref state);
        }


        private void CheckRoundFailed(ref SystemState state) {
            var roundData = state.EntityManager.GetComponentObject<RoundData>(state.SystemHandle);
            var defeated = true;
            foreach (var player in SystemAPI.Query<RefRO<PlayerComponent>>()) {
                defeated &= player.ValueRO.InGameAttributes.isDead;
            }

            roundData.RoundDefeated = defeated;
        }
    }
}