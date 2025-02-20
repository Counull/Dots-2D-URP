using Component;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Authoring {
    class RoosterAuthoring : MonoBehaviour {
        [SerializeField] private MonsterComponent monsterData;
        [SerializeField] private ChaseComponent chaseData;
        [SerializeField] private ChargeComponent chargeData;

        [SerializeField] private ShooterComponent shooterData;

        [SerializeField] private GameObject projectilePrefab;

        class RoosterAuthoringBaker : Baker<RoosterAuthoring> {
            public override void Bake(RoosterAuthoring authoring) {
                authoring.monsterData.Reset();
                authoring.shooterData.ProjectilePrefab =
                    GetEntity(authoring.projectilePrefab, TransformUsageFlags.Dynamic);
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, authoring.monsterData);
                AddComponent(entity, authoring.chaseData);
                this.AddComponentDisabled(entity, authoring.chargeData);
                AddComponent(entity, authoring.shooterData);
                AddBuffer<ProjectileShootingEvent>(entity);
            }
        }
    }
}