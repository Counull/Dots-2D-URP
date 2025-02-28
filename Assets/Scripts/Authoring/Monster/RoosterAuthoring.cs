using Common;
using Component;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authoring.Monster {
    [DisallowMultipleComponent]
    internal class RoosterAuthoring : MonsterAuthoringBase {
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;
        [SerializeField] private ShooterComponent shooterData;
        [SerializeField] private GameObject projectilePrefab;

        private class RoosterAuthoringBaker : Baker<RoosterAuthoring> {
            public override void Bake(RoosterAuthoring authoring) {
                authoring.InitComponentData();
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.AddBaseComponent(this, entity);
                AddComponent(entity, authoring.chaseData);
                AddShooterComponent(this, entity, authoring.shooterData, authoring.projectilePrefab);
                this.AddComponentDisabled(entity, authoring.chargeData);
            }
        }
    }
}