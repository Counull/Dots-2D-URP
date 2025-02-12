using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private float moveSpeed = 5;


        class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);
                AddComponent<PlayerStatus>(entity);
                AddComponent<PlayerInput>(entity);

                var entityWithTransform = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                //TODO 这里添加这个组件或许会破坏与其他Player原型？
                AddComponent(entityWithTransform, new PlayerMovement() {Speed = authoring.moveSpeed});
            }
        }
    }


    public struct PlayerStatus : IComponentData {
        public bool IsDead;
    }

    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInput : IInputComponentData {
         public float Horizontal;
         public float Vertical;
    }

    public struct PlayerMovement : IComponentData {
        public float Speed;
    }
}