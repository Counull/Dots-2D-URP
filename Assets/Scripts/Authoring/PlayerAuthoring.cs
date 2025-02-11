using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAuthoring : MonoBehaviour {
    class Baker : Baker<PlayerAuthoring> {
        public override void Bake(PlayerAuthoring authoring) {
            var entity = GetEntity(authoring.gameObject, TransformUsageFlags.None);
            var entityWithTransform = GetEntity(authoring.gameObject, TransformUsageFlags.Dynamic);
            AddComponent<PlayerStatus>(entity);
            AddComponent<PlayerInput>(entity);
            //TODO 这里添加这个组件或许会破坏与其他Player原型？
            AddComponent<PlayerMovement>(entityWithTransform);
        }
    }
}


public struct PlayerStatus : IComponentData {
    public bool IsDead;
}

public struct PlayerInput : IComponentData {
    public float Horizontal;
    public float Vertical;
}

public struct PlayerMovement : IComponentData {
    public float Speed;
  
}