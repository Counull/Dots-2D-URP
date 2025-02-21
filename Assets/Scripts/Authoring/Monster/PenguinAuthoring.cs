using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Monster {
    internal class PenguinAuthoring : MonsterAuthoringBase {

        [SerializeField] private ChaseComponent chaseData;

        private class Baker : Baker<PenguinAuthoring> {
            public override void Bake(PenguinAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.healthComponent.Reset();
                AddComponent(entity, authoring.healthComponent);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
            }
        }
    }
}