using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private float moveSpeed = 5;


        class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent<Player>(entity);
                AddComponent<PlayerInput>(entity);


                //TODO 这里添加这个组件或许会破坏与其他Player原型？
                AddComponent(entity, new PlayerMovement() {Speed = authoring.moveSpeed});
                AddComponentObject(entity, new PlayerManagedComponent() {
                    Transform = authoring.transform,
                    Animator = authoring.GetComponent<Animator>()
                });
            }
        }
    }


    public struct Player : IComponentData {
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


    public class PlayerManagedComponent : ICleanupComponentData {
        public Transform Transform;
        public Animator Animator;
    }
}