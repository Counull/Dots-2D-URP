
#ifndef GPU_STRUCTURES_HLSL_
#define GPU_STRUCTURES_HLSL_

/////////////////////////////////////////////////////////////////////////////////

//  struct RigDefinition
ByteAddressBuffer rigDefinitions;
//  struct RigBone
ByteAddressBuffer rigBones;
//  struct AnimationClip
ByteAddressBuffer animationClips;
//  struct HumanRotationData
ByteAddressBuffer humanRotationDataBuffer;
int animatedBonesCount;

/////////////////////////////////////////////////////////////////////////////////

#include "GPUStructures/BoneTransform.hlsl"
#include "GPUStructures/AnimationClip.hlsl"
#include "GPUStructures/Track.hlsl"
#include "GPUStructures/SkinnedMeshBone.hlsl"
#include "GPUStructures/RigDefinition.hlsl"
#include "GPUStructures/RigBone.hlsl"
#include "GPUStructures/HumanRotationData.hlsl"
#include "GPUStructures/AnimationToProcess.hlsl"
#include "GPUStructures/AvatarMask.hlsl"
#include "GPUStructures/PerfectHashTable.hlsl"

/////////////////////////////////////////////////////////////////////////////////

struct AnimationJob
{
    int rigDefinitionIndex;
    int animatedBoneIndexOffset;
    int2 animationsToProcessRange;
};

/////////////////////////////////////////////////////////////////////////////////

struct SkinnedMeshWorkload
{
    int skinMatrixBaseOutIndex;
    int boneRemapTableIndex;
    int skinMatricesCount;
    int rootBoneIndex;
    int animatedBoneIndexOffset;
    float4x4 skinnedRootBoneToEntityTransform;
};

/////////////////////////////////////////////////////////////////////////////////

struct AnimatedBoneWorkload
{
    int boneIndexInRig;
    int animationJobIndex;
};

/////////////////////////////////////////////////////////////////////////////////

StructuredBuffer<AnimatedBoneWorkload> animatedBoneWorkload;
StructuredBuffer<AnimationJob> animationJobs;
StructuredBuffer<AnimationToProcess> animationsToProcess;

#endif
