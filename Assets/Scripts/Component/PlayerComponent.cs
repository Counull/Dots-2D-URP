using System;
using UnityEngine;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Component {
    public class GlobalPlayerAttributes {
        [Serializable]
        public struct ExperienceGrowthFormula {
            public float baseExperience; // A
            public float growthRate; // b

            static double CalculateExperience(int level, ExperienceGrowthFormula formula) {
                return formula.baseExperience * Math.Pow(formula.growthRate, level);
            }
        }
    }


    [Serializable]
    public struct PlayerAttributes : IComponentData {
        public float speed;
        public float health;
        public uint maxWeaponCount;
        public bool IsDead => health <= 0;
    }


    [GhostComponent]
    public struct PlayerComponent : IComponentData {
        public float Experience;
        public uint Level;
        public ulong NextLevelExperience;
        public PlayerAttributes InGameAttributes;
        public PlayerAttributes BaseAttributes;
    }


    [GhostComponent]
    public struct PlayerInput : IInputComponentData {
        [GhostField] public float Horizontal;
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