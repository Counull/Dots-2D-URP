using Common;
using Component;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring.Monster {
    public abstract class MonsterAuthoringBase : MonoBehaviour {
        [SerializeField] protected MonsterComponent monsterData;
        [SerializeField] protected HealthComponent healthComponent;
        [SerializeField] protected DmgSrcComponent collisionDmgSrc;

        protected void InitComponentData() {
            healthComponent.Reset();
        }


        public void AddBaseComponent(IBaker baker, Entity entity) {
            baker.AddComponent(entity, healthComponent);
            baker.AddComponent(entity, monsterData);
            baker.AddComponent(entity, collisionDmgSrc);
            baker.AddComponent(entity, new FactionComponent {Faction = Faction.Monster});
     
        }

        public static void AddShooterComponent(IBaker baker, Entity entity, ShooterComponent shooterData,
            GameObject projectilePrefab) {
            shooterData.ProjectilePrefab =
                baker.GetEntity(projectilePrefab, TransformUsageFlags.Dynamic);
            baker.AddComponent(entity, shooterData);
            baker.AddBuffer<ProjectileShootingEvent>(entity);
        }
    }
}