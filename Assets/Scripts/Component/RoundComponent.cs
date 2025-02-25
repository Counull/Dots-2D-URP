using Systems.Server.RoundSystemGroup;
using Unity.Entities;

namespace Component {
    public struct RoundData : IComponentData {
        public ushort CombatRound;
        public float MaxCombatTime => 60f;
        public ushort MaxCombatRound => 20;
        public ushort PlayerCount;
        public ushort MinPlayerCount => 1;
        public ushort MaxPlayerCount => 4;
        public bool RoundDefeated;
        public RoundPhase Phase;
        public RoundPhase NextPhase;

        //其它系统可以锁定阶段转换，防止在某些情况下提前进入下一个阶段
        private ushort _lockedSystemCount;
        private float _combatTimeCountingDown;

        public float CombatTimeCountingDown {
            get => _combatTimeCountingDown;
            set {
                var lastSecond = (int) _combatTimeCountingDown;
                var currentSecond = (int) value;
                _combatTimeCountingDown = value;
                CountingDownChangedPerSecond |= (lastSecond != currentSecond);
                if (_combatTimeCountingDown < 0) {
                    _combatTimeCountingDown = 0;
                }
            }
        }

        public bool CountingDownChangedPerSecond { get; private set; }
        public bool CombatTimeOut => CombatTimeCountingDown <= 0;

        public bool AllSystemReady() {
            return _lockedSystemCount == 0;
        }

        public void UIRefresh() {
            CountingDownChangedPerSecond = false;
        }

        public void SystemLock() {
            _lockedSystemCount++;
        }

        public void SystemUnlock() {
            _lockedSystemCount--;
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