using Unity.Entities;

namespace Utils {
    public static class BakerExtensions {
        public static void AddComponentDisabled<T>(this IBaker baker, Entity entity, T component) where
            T : unmanaged, IComponentData, IEnableableComponent {
            baker.AddComponent(entity, component);
            baker.SetComponentEnabled<T>(entity, false);
        }
    }
}