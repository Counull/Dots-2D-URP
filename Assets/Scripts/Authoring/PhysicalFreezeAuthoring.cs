using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring {
    [DisallowMultipleComponent]
    public class PhysicalFreezeAuthoring : MonoBehaviour {
        public class Baker : Baker<PhysicalFreezeAuthoring> {
            public override void Bake(PhysicalFreezeAuthoring authoring) {
                var entity = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
                AddComponent<PhysicalShouldFreeze>(entity);
            }
        }
    }
}