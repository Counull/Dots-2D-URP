#ifndef MAKE_WORLD_SPACE_BONE_TRANSFORMS_HLSL_
#define MAKE_WORLD_SPACE_BONE_TRANSFORMS_HLSL_

/////////////////////////////////////////////////////////////////////////////////

RWStructuredBuffer<BoneTransform> outBoneTransforms;
StructuredBuffer<BoneTransform> boneLocalTransforms;

/////////////////////////////////////////////////////////////////////////////////

[numthreads(256, 1, 1)]
void MakeRigSpaceBoneTransforms(uint tid: SV_DispatchThreadID)
{
    if (tid >= (uint)animatedBonesCount)
        return;

    AnimatedBoneWorkload boneWorkload = animatedBoneWorkload[tid];
    AnimationJob animationJob = animationJobs[boneWorkload.animationJobIndex];
    RigDefinition rigDef = RigDefinition::ReadFromRawBuffer(rigDefinitions, animationJob.rigDefinitionIndex);
    RigBone rigBone = RigBone::ReadFromRawBuffer(rigBones, rigDef.rigBonesRange.x + boneWorkload.boneIndexInRig);

    int absoluteBoneIndex = animationJob.animatedBoneIndexOffset + boneWorkload.boneIndexInRig;
    BoneTransform bt = boneLocalTransforms[absoluteBoneIndex];
    int parentBoneIndex = rigBone.parentBoneIndex;
    while (parentBoneIndex > 0)
    {
        RigBone parentBoneData = RigBone::ReadFromRawBuffer(rigBones, rigDef.rigBonesRange.x + parentBoneIndex);
        int absoluteParentBoneIndex = animationJob.animatedBoneIndexOffset + parentBoneIndex;
        BoneTransform parentBoneTransform = boneLocalTransforms[absoluteParentBoneIndex];
        bt = BoneTransform::Multiply(parentBoneTransform, bt);
        parentBoneIndex = parentBoneData.parentBoneIndex;
    }
    outBoneTransforms[absoluteBoneIndex] = bt;
}

#endif // MAKE_WORLD_SPACE_BONE_TRANSFORMS_HLSL_
