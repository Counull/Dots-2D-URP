using Component;
using Unity.Entities;
using Unity.VisualScripting;

namespace Systems.RoundSystem {
    public abstract class RoundPhaseState {
        private int _preparingSystemCount;


        protected readonly RoundData RoundData;
        public abstract RoundPhase Phase { get; }
        public RoundPhaseState NextPhase { get; protected set; }

        public RoundPhaseState(RoundData roundData) {
            RoundData = roundData;
        }


        public virtual void PhaseEnter() { }
        public virtual void PhaseExit() { }

        public virtual void SystemPreparing() {
            _preparingSystemCount++;
        }

        public virtual void SystemReady() {
            _preparingSystemCount--;
        }

        public virtual bool ReadyForNextPhase() {
            return _preparingSystemCount == 0;
        }
    }


    public class RoundInitPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Init;

        public RoundInitPhase(RoundData roundData) :
            base(roundData) {
            NextPhase = new RoundCombatPhase(roundData);
        }

        public override void PhaseEnter() {
            RoundData.CombatRound = 0;
        }
    }


    public class RoundCombatPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Combat;

        public RoundCombatPhase(RoundData roundData) :
            base(roundData) { }

        public override void PhaseEnter() {
            RoundData.CombatRound++;
            RoundData.CombatTimeCountingDown = RoundData.MaxCombatTime;
            RoundData.RoundDefeated = false;
            if (RoundData.CombatRound < RoundData.MaxCombatRound) {
                NextPhase = new RoundLevelUpPhase(RoundData);
            }
            else {
                //所有战斗回合结束进入结算阶段    
                NextPhase = new RoundSettlementPhase(RoundData);
            }
        }


        public override bool ReadyForNextPhase() {
            //如果战斗提前失败则直接进入结算阶段
            if (RoundData.RoundDefeated && NextPhase is not RoundSettlementPhase) {
                NextPhase = new RoundSettlementPhase(RoundData);
            }

            return base.ReadyForNextPhase() && (RoundData.CombatTimeOut || RoundData.RoundDefeated);
        }
    }

    public class RoundLevelUpPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.LevelUp;

        public RoundLevelUpPhase(RoundData roundData) :
            base(roundData) {
            NextPhase = new RoundPurchasePhase(roundData);
        }
    }


    public class RoundPurchasePhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Purchase;

        public RoundPurchasePhase(RoundData roundData) :
            base(roundData) {
            NextPhase = new RoundCombatPhase(roundData);
        }
    }


    public class RoundSettlementPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Settlement;

        public RoundSettlementPhase(RoundData roundData) :
            base(roundData) { }
    }
}