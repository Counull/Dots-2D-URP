using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring {
    public class GameRoomAuthoring : MonoBehaviour {
       [SerializeField] private uint maxPlayers = 4;
    }

    public struct GameRoom : IComponentData {
        public uint MaxPlayers;
    }
}