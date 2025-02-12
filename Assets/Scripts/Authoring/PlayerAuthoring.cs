using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class PlayerAuthoring : MonoBehaviour {
        [SerializeField] private float moveSpeed = 5;
        [SerializeField] private GameObject playerVisualizationPrefab;

        class Baker : Baker<PlayerAuthoring> {
            public override void Bake(PlayerAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);

                AddComponentObject<PlayerVisualizationComponent>(entity, new PlayerVisualizationComponent() {
                    PlayerVisualizationPrefab = authoring.playerVisualizationPrefab
                });

                AddComponent<Player>(entity);
                AddComponent<PlayerInput>(entity);


                //TODO 这里添加这个组件或许会破坏与其他Player原型？
                AddComponent(entity, new PlayerMovement() {Speed = authoring.moveSpeed});
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


    public class PlayerVisualizationComponent : IComponentData {
        public GameObject PlayerVisualizationPrefab;
        public GameObject VisualizationInstance;
        public Animator Animator;

        /// <summary>
        /// 有点纠结这个函数的位置，感觉应该放在PlayerManagedComponent里但是这个类是个组件
        /// 《重构》里的典型坏代码
        /// 还是放在这了
        /// </summary>
        /// <param name="playerVisualizationComponent"></param>
        public void CreatePlayerVisualizationInstance() {
            //实例化可视化对象
            VisualizationInstance = Object.Instantiate(PlayerVisualizationPrefab);
            Animator = VisualizationInstance.GetComponent<Animator>();
        }
    }
}