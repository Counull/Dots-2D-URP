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

                AddComponentObject(entity, new PlayerVisualizationComponent() {
                    PlayerVisualizationPrefab = authoring.playerVisualizationPrefab
                });

                AddComponent(entity,new Player(){Speed = 5});
                AddComponent<PlayerInput>(entity);


 
            }
        }
    }


    public struct Player : IComponentData {
        public bool IsDead;
        public float Speed;
    }

    [GhostComponent()]
    public struct PlayerInput : IInputComponentData{
        [GhostField]  public float Horizontal;
        [GhostField] public float Vertical;
      
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