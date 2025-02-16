using Systems.EnemyBehavior;
using Unity.Entities;
using UnityEngine;

namespace Authoring {
    class PenguinAuthoring : MonoBehaviour {
        [SerializeField] private float speed;

        class Baker : Baker<PenguinAuthoring> {
            public override void Bake(PenguinAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent(entity, new ChaseComponent() {Speed = authoring.speed});
            }
        }
    }
}