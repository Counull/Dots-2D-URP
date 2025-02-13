using Unity.Entities;

namespace Systems {
    /// <summary>
    /// 这是创建这个System我第一个想到的事儿，
    /// 就是或许这个System是个管理类会更好，因为我完全可以面向对象写一个状态机去管理游戏状态变化
    /// 我已经预感到这个类会被实现得很烂
    /// </summary>
    public partial struct RoundSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.EntityManager.AddComponent<RoundData>(state.SystemHandle);
            state.RequireForUpdate<RoundData>();
        }

        public void OnUpdate(ref SystemState state) {
            var roundData = state.EntityManager.GetComponentData<RoundData>(state.SystemHandle);
            if (roundData.Phase == RoundPhase.Init) {
                if (roundData.NextPhase == default) ;
                {
                    roundData.NextPhase = RoundPhase.Combat;
                    InitGame(ref state);
                    return;
                }
                
            }
        }


        private void InitGame(ref SystemState state) { }


        public enum RoundPhase {
            Init, // 初始化阶段
            LevelUp, // 升级阶段
            Combat, // 战斗阶段
            Purchase, // 购买阶段
            Settlement, // 结算阶段
            //   PreparingNextPhase, // 等待其他系统完成
        }
    }


    public struct RoundData : IComponentData {
        public RoundSystem.RoundPhase Phase;
        public RoundSystem.RoundPhase NextPhase;
        public ushort Round;
        public ushort MaxRound;
        public ushort BattleCountingDown;
        public ushort PreparingSystemCount;
    }
}