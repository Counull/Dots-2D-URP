using System;
using Unity.Entities;
using UnityEngine;

namespace Component {
    public struct PlayerSpawner : IComponentData {
        public Entity PlayerPrefab;
    }

    public struct EnemyPrefabElement : IBufferElementData {
        public Entity EnemyPrefab;
        public EnemySpawnAttributes EnemyAttributes;
    }


    [Serializable]
    public struct EnemySpawnAttributes : IComponentData {
        public float maxSpawnCount;
        public ushort groupSpawnRange;
        public float spawnInterval;
        [HideInInspector] public double nextSpawnTime;
    }

    public struct WeaponSlotElement : IBufferElementData {
        public Entity WeaponPrefab;
    }

    public struct SpawnSettings : IComponentData {
        public Vector2 SpawnFiledLB;
        public Vector2 SpawnFiledRT;
    }
}