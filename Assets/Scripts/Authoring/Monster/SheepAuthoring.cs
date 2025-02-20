using Component;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Authoring {
    class SheepAuthoring : MonoBehaviour {
        [SerializeField] private MonsterComponent monsterData;
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;

        class SheepAuthoringBaker : Baker<SheepAuthoring> {
            public override void Bake(SheepAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.monsterData.Reset();
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
                this.AddComponentDisabled(entity, authoring.chargeData);
            }
        }
    }
}