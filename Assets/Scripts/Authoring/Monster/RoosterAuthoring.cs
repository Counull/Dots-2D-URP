using Component;
using Unity.Entities;
using UnityEngine;
using Utils;

namespace Authoring.Monster {
    internal class RoosterAuthoring : MonsterAuthoringBase {

        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;
        [SerializeField] private ShooterComponent shooterData;
        [SerializeField] private GameObject projectilePrefab;

        private class RoosterAuthoringBaker : Baker<RoosterAuthoring> {
            public override void Bake(RoosterAuthoring authoring) {
              
                authoring.shooterData.ProjectilePrefab =
                    GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic);
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                authoring.healthComponent.Reset();
                AddComponent(entity, authoring.healthComponent);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
                this.AddComponentDisabled(entity, authoring.chargeData);
                AddComponent(entity, authoring.shooterData);
                AddBuffer<ProjectileShootingEvent>(entity);
            }
        }
    }
}