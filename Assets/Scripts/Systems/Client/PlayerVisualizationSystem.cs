using Aspect;
using Component;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Client {
    /// <summary>
    ///     需要与GameObject交互所以使用托管组件
    ///     在客户端的所有移动操作（包括网络同步）之后更新
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateAfter(typeof(GhostSimulationSystemGroup))]
    public partial class PlayerVisualizationSystem : SystemBase {
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");
        private static readonly int Direction = Animator.StringToHash("Direction");
        private CinemachineCamera _camera;

        protected override void OnCreate() {
            RequireForUpdate<PlayerComponent>();
            RequireForUpdate<PlayerVisualizationComponent>();
        }

        protected override void OnStartRunning() {
            _camera = Object.FindFirstObjectByType<CinemachineCamera>();
        }


        protected override void OnUpdate() {
            foreach (var (localTransform, playerInput, entity) in
                     SystemAPI.Query<RefRO<LocalTransform>, RefRO<PlayerInput>>()
                         .WithAll<GhostOwnerIsLocal, PlayerComponent>()
                         .WithEntityAccess()) {
                var managedComponent = EntityManager.GetComponentObject<PlayerVisualizationComponent>(entity);

                //如果没有可视化对象，就实例化一个
                if (!managedComponent.VisualizationInstance) {
                    managedComponent.CreatePlayerVisualizationInstance();

                    //如果是本地玩家，就让摄像机跟随
                    if (EntityManager.IsComponentEnabled<GhostOwnerIsLocal>(entity))
                        _camera.Follow = managedComponent.VisualizationInstance.transform;
                }

                //更新可视化对象的位置
                managedComponent.VisualizationInstance.transform.position = localTransform.ValueRO.Position;

                //更新可视化对象的动画
                UpdateAnimation(managedComponent.Animator,
                    new Vector2(playerInput.ValueRO.Horizontal, playerInput.ValueRO.Vertical));
            }
        }


        private void UpdateAnimation(Animator animator, Vector2 input) {
            //更新可视化对象的动画
            var isWalking = input.magnitude > 0;
            animator.SetBool(IsMoving, isWalking);
            if (!isWalking) return;
            if (input.x < 0)
                animator.SetInteger(Direction, 3);
            else if (input.x > 0)
                animator.SetInteger(Direction, 2);
            else if (input.y < 0)
                animator.SetInteger(Direction, 0);
            else if (input.y > 0) animator.SetInteger(Direction, 1);
        }
    }
}