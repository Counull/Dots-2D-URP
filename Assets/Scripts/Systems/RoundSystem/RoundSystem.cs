using Unity.Entities;

namespace Systems.RoundSystem {
    /// <summary>
    /// 这是创建这个System我第一个想到的事儿，
    /// 就是或许这个System是个管理类会更好，因为我完全可以面向对象写一个状态机去管理游戏状态变化
    /// 我已经预感到这个类会被实现得很烂
    /// </summary>
    public partial struct RoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            var roundData = new RoundData() { };
            state.EntityManager.AddComponentObject(state.SystemHandle,
                roundData);
            roundData.PhaseState = new RoundInitPhase(roundData);
            state.RequireForUpdate<RoundData>();
        }


        //临时阶段，用于测试
        public void OnUpdate(ref SystemState state) {
            var roundData = state.EntityManager.GetComponentObject<RoundData>(state.SystemHandle);
            if (roundData.PhaseState.ReadyForNextPhase()) {
                roundData.PhaseState = roundData.PhaseState.NextPhase;
            }
        }
    }


    public class RoundData : IComponentData {
        public RoundPhaseState PhaseState;
        public float CombatTimeCountingDown;
        public float MaxCombatTime;
        public ushort CombatRound;
        public ushort MaxCombatRound;
        public bool RoundFiled;
    }


    public enum RoundPhase {
        Init, // 初始化阶段
        LevelUp, // 升级阶段
        Combat, // 战斗阶段
        Purchase, // 购买阶段
        Settlement, // 结算阶段
    }
}