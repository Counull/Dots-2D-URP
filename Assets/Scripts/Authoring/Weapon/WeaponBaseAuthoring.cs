using Component;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Weapon {
    public abstract class WeaponBaseAuthoring : MonoBehaviour {
        [SerializeField] protected WeaponComponent weaponComponent;
       
    }
}