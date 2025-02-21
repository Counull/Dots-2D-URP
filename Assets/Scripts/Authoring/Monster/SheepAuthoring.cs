using Component;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authoring.Monster {
 

    internal class MonsterAuthoring : MonsterAuthoringBase {
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;

        private class SheepAuthoringBaker : Baker<MonsterAuthoring> {
            public override void Bake(MonsterAuthoring authoring) {
            
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.healthComponent.Reset();
                AddComponent(entity, authoring.healthComponent);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
                this.AddComponentDisabled(entity, authoring.chargeData);
            }
        }
    }
}