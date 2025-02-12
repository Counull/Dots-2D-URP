using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Systems.Client {
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    public partial struct PlayerInputSystem : ISystem {
        public class InputActions : IComponentData {
            public InputAction MoveAction;
        }

        public void OnCreate(ref SystemState state) {
            //将硬件输入转换为Component添加到SystemHandle上，
            //以便在Update中使用GetComponentObject<>（）函数取回
            state.EntityManager.AddComponentObject(state.SystemHandle,
                new InputActions() {MoveAction = InputSystem.actions.FindAction("Move")});

            state.RequireForUpdate<Authoring.PlayerInput>();
            state.RequireForUpdate<GhostOwnerIsLocal>();
        }


        public void OnUpdate(ref SystemState state) {
            //这模板用法总感觉是在写cpp
            foreach (var input in
                     SystemAPI.Query<RefRW<Authoring.PlayerInput>>()
                         .WithAll<GhostOwnerIsLocal>()
                    ) {
                input.ValueRW = default;

                var actions = state.EntityManager.GetComponentObject<InputActions>(state.SystemHandle);
                var move = actions.MoveAction.ReadValue<Vector2>();
                input.ValueRW.Horizontal = move.x;
                input.ValueRW.Vertical = move.y;
            }
        }
    }
}