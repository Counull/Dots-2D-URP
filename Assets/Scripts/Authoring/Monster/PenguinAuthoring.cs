using Component;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Authoring {
    class PenguinAuthoring : MonoBehaviour {
        [SerializeField] private MonsterComponent monsterData;
        [SerializeField] private ChaseComponent chaseData;

        class Baker : Baker<PenguinAuthoring> {
            public override void Bake(PenguinAuthoring authoring) {
                authoring.monsterData.Reset();
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
            }
        }
    }
}