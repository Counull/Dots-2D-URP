using Common;
using Component;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authoring.Monster {
    internal class SheepAuthoring : MonsterAuthoringBase {
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;

        private class SheepAuthoringBaker : Baker<SheepAuthoring> {
            public override void Bake(SheepAuthoring authoring) {
                authoring.InitComponentData();
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.AddBaseComponent(this, entity);
                AddComponent(entity, authoring.chaseData);
                this.AddComponentDisabled(entity, authoring.chargeData);
            }
        }
    }
}