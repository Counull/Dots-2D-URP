using Common;
using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Monster {
    [DisallowMultipleComponent]
    internal class PenguinAuthoring : MonsterAuthoringBase {
        [SerializeField] private ChaseComponent chaseData;


        /// <summary>
        /// todo Baker 究竟能不能继承
        /// </summary>
        private class Baker : Baker<PenguinAuthoring> {
            public override void Bake(PenguinAuthoring authoring) {
                authoring.InitComponentData();
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.chaseData);
                authoring.AddBaseComponent(this, entity);
            }
        }
    }
}