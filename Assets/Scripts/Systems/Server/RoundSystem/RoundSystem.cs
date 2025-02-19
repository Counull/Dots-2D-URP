using Unity.Entities;
using UnityEngine;

namespace Systems.Server.RoundSystem {
    /// <summary>
    /// 这是创建这个System我第一个想到的事儿，
    /// 就是或许这个System是个管理类会更好，因为我完全可以面向对象写一个状态机去管理游戏状态变化
    /// 我已经预感到这个类会被实现得很烂
    /// </summary>
    public partial struct RoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
            var roundData = new RoundData() { };
            var initPhase = new RoundInitPhase();
            initPhase.PhaseEnter(ref roundData); //进入初始化阶段
            state.EntityManager.AddComponentObject(state.SystemHandle,
                new RoundPhaseData() {PhaseState = initPhase});
            state.EntityManager.AddComponentData(state.SystemHandle,
                roundData);
        }


        //临时阶段，用于测试
        public void OnUpdate(ref SystemState state) {
            var roundData = state.EntityManager.GetComponentDataRW<RoundData>(state.SystemHandle);
            var phaseStateData = state.EntityManager.GetComponentObject<RoundPhaseData>(state.SystemHandle);

            if (phaseStateData.PhaseState.ReadyForNextPhase(ref roundData.ValueRW)) {
                phaseStateData.PhaseState.PhaseExit(ref roundData.ValueRW);

                if (phaseStateData.PhaseState.Phase == RoundPhase.Settlement &&
                    phaseStateData.PhaseState.NextPhase == null) {
                    //游戏结束
                    Debug.Log("Game Over");
                    return;
                }

                //状态切换调度
                phaseStateData.PhaseState = phaseStateData.PhaseState.NextPhase;
                phaseStateData.PhaseState?.PhaseEnter(ref roundData.ValueRW);
            }
        }
    }


    // manged component真的是有诸多限制
    public struct RoundData : IComponentData {
        public float CombatTimeCountingDown;
        public ushort CombatRound;
        public float MaxCombatTime => 60f;
        public ushort MaxCombatRound => 20;
        public bool RoundDefeated;
        public RoundPhase Phase;
        public RoundPhase NextPhase;

        //其它系统可以锁定阶段转换，防止在某些情况下提前进入下一个阶段
        private ushort _lockedSystemCount;
        public bool CombatTimeOut => CombatTimeCountingDown <= 0;
        public bool AllSystemReady() => _lockedSystemCount == 0;

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
        Settlement, // 结算阶段
    }
}