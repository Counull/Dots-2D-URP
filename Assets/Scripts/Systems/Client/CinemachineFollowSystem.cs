using System.Runtime.InteropServices;
using Authoring;
using Unity.Cinemachine;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using Unity.VisualScripting;
using UnityEngine;

namespace Systems.Client {
    /// <summary>
    /// 需要与GameObject交互所以使用托管组件
    /// </summary>
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class SetCinemachineFollowSystem : SystemBase {
        private CinemachineCamera virtualCamera;
        private Transform cameraTargetTransform;

        protected override void OnCreate() {
            RequireForUpdate<Player>();
            RequireForUpdate<GhostOwnerIsLocal>();
        }

        protected override void OnStartRunning() {
            cameraTargetTransform = GameObject.Find("CameraTarget").GetComponent<Transform>();
            virtualCamera = Object.FindFirstObjectByType<CinemachineCamera>();
        }


        protected override void OnUpdate() {
            foreach (var localTransform in
                     SystemAPI.Query<RefRO<LocalTransform>>().WithAll<Player, GhostOwnerIsLocal>()) {
                cameraTargetTransform.position = localTransform.ValueRO.Position;
            }
            
        }
    }

    public struct CameraFollowed : IComponentData { }
}