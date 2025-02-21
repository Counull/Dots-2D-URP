using Component;
using UnityEngine;

namespace Authoring.Monster {
    public class MonsterAuthoringBase : MonoBehaviour {
        [SerializeField] protected MonsterComponent monsterData;
        [SerializeField] protected HealthComponent healthComponent;
    }
}