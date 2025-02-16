using Unity.Entities;

namespace Systems.EnemyBehavior {
    public struct EnemyComponent : IComponentData {
        public float Health;
        public float MaxHealth;
    }

    public struct ChaseComponent : IComponentData {
        public float Speed;
    }

    public struct ChargeComponent : IComponentData {
        public float Speed;
    }


    public struct ShootComponent : IComponentData {
        public float ShootInterval;
        public float ShootRange;
    }
}