
using Unity.Entities;

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

namespace Rukhanka
{
public struct AvatarMaskBlob
{
#if RUKHANKA_DEBUG_INFO
	public BlobString name;
	public BlobArray<BlobString> includedBoneNames;
	public float bakingTime;
#endif
	public Hash128 hash;
	public BlobArray<uint> includedBoneMask;
	public uint humanBodyPartsAvatarMask;
	
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public static (int, uint) GetUintIndexAndMask(int boneIndex)
	{
		int uintIndex = boneIndex >> 5; // boneIndex / 32
		int byteOffsetInUint = boneIndex & 0x1f; // boneIndex % 32;
		uint boneMask = 1u << byteOffsetInUint;
		return (uintIndex, boneMask);
	}

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

	public bool IsBoneIncluded(int boneIndex)
	{
		var (uintIndex, mask) = GetUintIndexAndMask(boneIndex);
		var rv = (includedBoneMask[uintIndex] & mask) != 0;
		return rv;
	}
}
}
