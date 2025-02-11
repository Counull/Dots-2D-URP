using System.ComponentModel;
using Unity.Entities;
using UnityEngine;

public class GameRoomAuthoring : MonoBehaviour {
    [SerializeField] private uint MaxPlayers = 4;
    
}

 public struct GameRoom : IComponentData {
   public uint maxPlayers;
}