using Component;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authoring.Monster {
    internal class SheepAuthoring : MonoBehaviour {
        [SerializeField] private MonsterComponent monsterData;
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;

        private class SheepAuthoringBaker : Baker<SheepAuthoring> {
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