using Component;
using Unity.Entities;

namespace Systems {
    public partial struct WeaponSystem : ISystem {
        public void OnCreate(ref SystemState state) {
            state.RequireForUpdate<WeaponComponent>();
        }

        public void OnUpdate(ref SystemState state) {
            foreach (var weapon in SystemAPI.Query<RefRW<WeaponComponent>>()) { }
        }
    }
}