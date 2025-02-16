
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using Hash128 = Unity.Entities.Hash128;

////////////////////////////////////////////////////////////////////////////////////////

namespace Rukhanka.Hybrid
{
[HelpURL("https://docs.rukhanka.com/getting_started#rig-definition")]
public class RigDefinitionAuthoring: MonoBehaviour
{
    public enum BoneEntityStrippingMode
    {
        None,
        Automatic,
        Manual
    }
    
    public enum RigConfigSource
    {
        FromAnimator,
        UserDefined
    }
    
    public enum AnimationEngine
    {
        CPU,
        GPU
    }

    public RigConfigSource rigConfigSource;
    public Avatar avatar;
    public bool applyRootMotion;
    public bool animationCulling;
    
    [Tooltip("<color=Cyan><b>None</b></color> - keep all skeleton bone entities.\n<color=Cyan><b>Automatic</b></color> - automatically strip unreferenced bone entities.\n<color=Cyan><b>Manual</b></color> - included and stripped bone entities will be taken from specified avatar mask. This mode will make 'flat' bone hierarchy.")]
    public BoneEntityStrippingMode boneEntityStrippingMode;
    public AvatarMask boneStrippingMask;
    public bool hasAnimationEvents;
    public bool hasAnimatorControllerEvents;
    public AnimationEngine animationEngine;
    
////////////////////////////////////////////////////////////////////////////////////////

    public Avatar GetAvatar()
    {
        var rv = avatar;
        if (rigConfigSource == RigConfigSource.FromAnimator)
        {
            var anm = GetComponent<Animator>();
            if (anm)
                rv = anm.avatar;
        }
        return rv;
    }
    
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public Hash128 CalculateRigHash()
	{
    #if UNITY_EDITOR
        var a = GetAvatar();
        var h = BakingUtils.GetAssetID((Object)a ?? gameObject);
		var rv = new Hash128(h.x, h.y, 0, 0);
        Assert.IsTrue(!math.all(rv.Value == default), $"Rig hash error '{name}'");
    #else
        var rv = new Hash128();
    #endif
		return rv;
	}
}
}
