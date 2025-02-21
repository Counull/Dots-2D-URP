using Component;

using Unity.Entities;

namespace Systems.Server.RoundSystemGroup {
    [UpdateInGroup(typeof(RoundSystemGroup))]
    [UpdateAfter(typeof(RoundSystem))]
    public partial struct CombatRoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
            state.RequireForUpdate<PlayerComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            var roundData = SystemAPI.GetSingletonRW<RoundData>();

            //如果当前不是战斗阶段，就不进行任何操作，这个判断可以优化成RequireForUpdate
            if (roundData.ValueRO.Phase != RoundPhase.Combat) return;
            //更新计时
            roundData.ValueRW.CombatTimeCountingDown -= SystemAPI.Time.DeltaTime;
            if (roundData.ValueRO.CombatTimeOut) return;

            CheckRoundFailed(ref state, ref roundData.ValueRW);
        }


        /// <summary>
        ///     所有玩家死亡则战斗回合失败
        /// </summary>
        /// <param name="state"></param>
        /// <param name="roundData"></param>
        private void CheckRoundFailed(ref SystemState state, ref RoundData roundData) {
            var defeated = true;
            foreach (var player in SystemAPI.Query<RefRO<PlayerComponent>>())
                defeated &= player.ValueRO.InGameAttributes.IsDead;

            roundData.RoundDefeated = defeated;
        }
    }
}