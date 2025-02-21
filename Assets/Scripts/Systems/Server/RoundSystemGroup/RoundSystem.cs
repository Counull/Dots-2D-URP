using Component;
using Unity.Entities;
using UnityEngine;

namespace Systems.Server.RoundSystemGroup {
    /// <summary>
    ///     这是创建这个System我第一个想到的事儿，
    ///     就是或许这个System是个管理类会更好，因为我完全可以面向对象写一个状态机去管理游戏状态变化
    ///     我已经预感到这个类会被实现得很烂
    /// </summary>
    [UpdateInGroup(typeof(RoundSystemGroup))]
    public partial struct RoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<RoundData>();
            var roundData = new RoundData();
            var initPhase = new RoundInitPhase();
            initPhase.PhaseEnter(ref roundData); //进入初始化阶段
            state.EntityManager.AddComponentObject(state.SystemHandle,
                new RoundPhaseData {PhaseState = initPhase});
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

}