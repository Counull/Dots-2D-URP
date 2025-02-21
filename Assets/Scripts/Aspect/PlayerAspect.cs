using Component;
using Unity.Entities;
using Unity.Transforms;

namespace Aspect {
    public readonly partial struct PlayerBaseAspectRW : IAspect {
        public readonly RefRW<HealthComponent> HealthComponent;
        public readonly RefRW<LocalTransform> LocalTransform;
        public readonly RefRW<PlayerComponent> PlayerComponent;
    }
}