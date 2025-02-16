#ifndef MAKE_SKIN_MATRICES_HLSL_
#define MAKE_SKIN_MATRICES_HLSL_

/////////////////////////////////////////////////////////////////////////////////

RWStructuredBuffer<float3x4> outSkinMatrices;
StructuredBuffer<BoneTransform> rigSpaceBoneTransformsBuf;
StructuredBuffer<SkinnedMeshWorkload> skinMatrixWorkloadBuf;
ByteAddressBuffer skinnedMeshBoneData;
uint totalSkinnedMeshes;

/////////////////////////////////////////////////////////////////////////////////

[numthreads(128, 1, 1)]
void ComputeSkinMatrices(uint tid: SV_DispatchThreadID)
{
    if (tid >= totalSkinnedMeshes)
        return;

    SkinnedMeshWorkload smw = skinMatrixWorkloadBuf[tid];
    float4x4 w2l = smw.skinnedRootBoneToEntityTransform;

    for (int i = 0; i < smw.skinMatricesCount; ++i)
    {
        SkinnedMeshBone smb = SkinnedMeshBone::ReadFromRawBuffer(skinnedMeshBoneData, smw.boneRemapTableIndex + i);
        BoneTransform bt = rigSpaceBoneTransformsBuf[smb.boneRemapIndex + smw.animatedBoneIndexOffset];
        float4x4 skinMatrix = bt.ToFloat4x4();
        skinMatrix = mul(w2l, skinMatrix);
        float4x4 outSkinMatrix = mul(skinMatrix, smb.bindPose);
        outSkinMatrices[smw.skinMatrixBaseOutIndex + i] = (float3x4)outSkinMatrix;
    }
}

#endif //MAKE_SKIN_MATRICES_HLSL_
