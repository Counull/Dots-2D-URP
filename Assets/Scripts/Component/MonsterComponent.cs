using Unity.Entities;

namespace Component {
    public struct MonsterComponent : IComponentData {
        public float Health;
        public float MaxHealth;
    }

    public struct ChaseComponent : IComponentData, IEnableableComponent {
        public float Speed;
    }

    public struct ChargeComponent : IComponentData, IEnableableComponent {
        public float Speed;
    }


    public struct ShootComponent : IComponentData, IEnableableComponent {
        public float ShootInterval;
        public float ShootRange;
    }
}