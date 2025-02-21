using Systems.Server.RoundSystemGroup;
using Unity.Entities;

namespace Component {
    public struct RoundData : IComponentData {
        public float CombatTimeCountingDown;
        public ushort CombatRound;
        public float MaxCombatTime => 60f;
        public ushort MaxCombatRound => 20;
        public bool RoundDefeated;
        public RoundPhase Phase;
        public RoundPhase NextPhase;

        //其它系统可以锁定阶段转换，防止在某些情况下提前进入下一个阶段
        private ushort lockedSystemCount;
        public bool CombatTimeOut => CombatTimeCountingDown <= 0;

        public bool AllSystemReady() {
            return lockedSystemCount == 0;
        }

        public void SystemLock() {
            lockedSystemCount++;
        }

        public void SystemUnlock() {
            lockedSystemCount--;
        }
    }

    public class RoundPhaseData : IComponentData {
        public RoundPhaseState PhaseState;
    }


    public enum RoundPhase {
        Init, // 初始化阶段
        LevelUp, // 升级阶段
        Combat, // 战斗阶段
        Purchase, // 购买阶段
        Settlement // 结算阶段
    }
}