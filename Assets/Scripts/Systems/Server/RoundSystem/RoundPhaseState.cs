namespace Systems.Server.RoundSystem {
    /// <summary>
    /// 阶段转换的状态机
    /// </summary>
    public abstract class RoundPhaseState {
        public abstract RoundPhase Phase { get; }
        public RoundPhaseState NextPhase { get; protected set; }

        public virtual void PhaseEnter(ref RoundData roundData) {
            roundData.Phase = Phase;
            if (NextPhase != null) {
                roundData.NextPhase = NextPhase.Phase;
            }
        }

        public virtual void PhaseExit(ref RoundData roundData) { }

        public virtual bool ReadyForNextPhase(ref RoundData roundData) {
            return roundData.AllSystemReady();
        }
    }

    /// <summary>
    /// 初始化阶段
    /// </summary>
    public class RoundInitPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Init;

        public RoundInitPhase() { }

        public override void PhaseEnter(ref RoundData roundData) {
            NextPhase = new RoundCombatPhase();
            roundData.CombatRound = 0;
            base.PhaseEnter(ref roundData);
        }
    }

    /// <summary>
    /// 战斗阶段
    /// </summary>
    public class RoundCombatPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Combat;

        public override void PhaseEnter(ref RoundData roundData) {
            roundData.CombatRound++;
            roundData.CombatTimeCountingDown = roundData.MaxCombatTime;
            roundData.RoundDefeated = false;
            if (roundData.CombatRound < roundData.MaxCombatRound) {
                NextPhase = new RoundLevelUpPhase();
            }
            else {
                //所有战斗回合结束进入结算阶段    
                NextPhase = new RoundSettlementPhase();
            }

            base.PhaseEnter(ref roundData);
        }


        public override bool ReadyForNextPhase(ref RoundData roundData) {
            //如果战斗提前失败则直接进入结算阶段
            if (roundData.RoundDefeated && NextPhase is not RoundSettlementPhase) {
                NextPhase = new RoundSettlementPhase();
                roundData.NextPhase = NextPhase.Phase;
            }

            //如果超时或战斗失败则进入下一阶段
            return base.ReadyForNextPhase(ref roundData) && (roundData.CombatTimeOut || roundData.RoundDefeated);
        }
    }

    /// <summary>
    /// 升级选择阶段
    /// </summary>
    public class RoundLevelUpPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.LevelUp;

        public RoundLevelUpPhase() { }

        public override void PhaseEnter(ref RoundData roundData) {
            NextPhase = new RoundPurchasePhase();
            base.PhaseEnter(ref roundData);
        }
    }


    /// <summary>
    /// 购买物品阶段
    /// </summary>
    public class RoundPurchasePhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Purchase;

        public RoundPurchasePhase() { }

        public override void PhaseEnter(ref RoundData roundData) {
            NextPhase = new RoundCombatPhase();
            base.PhaseEnter(ref roundData);
        }
    }


    /// <summary>
    /// 结算阶段
    /// </summary>
    public class RoundSettlementPhase : RoundPhaseState {
        public override RoundPhase Phase => RoundPhase.Settlement;

        public RoundSettlementPhase() { }
    }
}