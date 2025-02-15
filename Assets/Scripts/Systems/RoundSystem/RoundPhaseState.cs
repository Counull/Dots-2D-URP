using Unity.VisualScripting;

namespace Systems.RoundSystem {
    public abstract class RoundPhaseState {
        private int _preparingSystemCount;

        public RoundPhaseState(RoundData roundData) {
            RoundData = roundData;
        }

        protected RoundData RoundData;
        public abstract RoundPhase Phase { get; }


        public RoundPhaseState NextPhase { get; protected set; }

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
        public RoundInitPhase(RoundData roundData) :
            base(roundData) {
            roundData.CombatRound = 0;
            NextPhase = new RoundCombatPhase(roundData);
        }

        public override RoundPhase Phase => RoundPhase.Init;
    }


    public class RoundCombatPhase : RoundPhaseState {
        public RoundCombatPhase(RoundData roundData) :
            base(roundData) {
            roundData.CombatRound++;
            roundData.CombatTimeCountingDown = roundData.MaxCombatTime;
            roundData.RoundFiled = false;
            if (roundData.CombatRound <= roundData.MaxCombatRound) {
                NextPhase = new RoundLevelUpPhase(roundData);
            }
            else {
                //所有战斗回合结束进入结算阶段    
                NextPhase = new RoundSettlementPhase(roundData);
            }
        }

        public override bool ReadyForNextPhase() {
            //如果战斗提前失败则直接进入结算阶段
            if (RoundData.RoundFiled && NextPhase is not RoundSettlementPhase) {
                NextPhase = new RoundSettlementPhase(RoundData);
            }

            return base.ReadyForNextPhase() && (RoundData.CombatTimeCountingDown <= 0 || RoundData.RoundFiled);
        }

        public override RoundPhase Phase => RoundPhase.Combat;
    }

    public class RoundLevelUpPhase : RoundPhaseState {
        public RoundLevelUpPhase(RoundData roundData) :
            base(roundData) {
            NextPhase = new RoundPurchasePhase(roundData);
        }

        public override RoundPhase Phase => RoundPhase.LevelUp;
    }


    public class RoundPurchasePhase : RoundPhaseState {
        public RoundPurchasePhase(RoundData roundData) :
            base(roundData) { }

        public override RoundPhase Phase => RoundPhase.Purchase;
    }


    public class RoundSettlementPhase : RoundPhaseState {
        public RoundSettlementPhase(RoundData roundData) :
            base(roundData) { }

        public override RoundPhase Phase => RoundPhase.Settlement;
    }
}