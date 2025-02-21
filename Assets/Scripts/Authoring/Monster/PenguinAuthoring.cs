using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Monster {
    internal class PenguinAuthoring : MonoBehaviour {
        [SerializeField] private MonsterComponent monsterData;
        [SerializeField] private ChaseComponent chaseData;

        private class Baker : Baker<PenguinAuthoring> {
            public override void Bake(PenguinAuthoring authoring) {
                authoring.monsterData.Reset();
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
            }
        }
    }
}