using Aspect;
using Component;
using Mono;
using Unity.Entities;
using Unity.NetCode;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Client {
    
    /// <summary>
    /// 用于刷新UI
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class MainUiSystem : SystemBase {
        private EntityQuery _localPlayerQuery;
        private UiManager _uiManager;

        protected override void OnCreate() {
            _localPlayerQuery = SystemAPI.QueryBuilder().WithAll<PlayerComponent, GhostOwnerIsLocal, HealthComponent>()
                .Build();
            RequireForUpdate(_localPlayerQuery);
            RequireForUpdate<RoundData>();
        }

        protected override void OnStartRunning() {
            _uiManager = Object.FindFirstObjectByType<UiManager>();
        }

        protected override void OnUpdate() {
            var roundData = SystemAPI.GetSingletonRW<RoundData>();
            if (roundData.ValueRO.RoundDefeated || roundData.ValueRO.CombatTimeOut ||
                roundData.ValueRO.CountingDownChangedPerSecond) {
                _uiManager.RefreshRoundData(roundData.ValueRW);
                roundData.ValueRW.UIRefresh();
            }

            foreach (var health in SystemAPI.Query<RefRW<HealthComponent>>()
                         .WithAll<PlayerComponent, GhostOwnerIsLocal>()) {
                if (health.ValueRO.HealthChanged) {
                    _uiManager.RefreshHp(health.ValueRO);
                    health.ValueRW.UIRefreshed();
                }
            }
        }
    }
}