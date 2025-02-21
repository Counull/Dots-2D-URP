using Component;
using Unity.Entities;
using Unity.Transforms;

namespace Aspect {
    public readonly partial struct MonsterAspect : IAspect {
        public readonly RefRW<LocalTransform> LocalTransform;
        public readonly RefRW<MonsterComponent> Monster;
    }

    public readonly partial struct MonsterAspectWithHealthRW : IAspect {
        public readonly RefRW<HealthComponent> HealthComponent;
        public readonly RefRW<LocalTransform> LocalTransform;
        public readonly RefRW<MonsterComponent> Monster;
    }
}