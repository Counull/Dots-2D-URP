#ifndef BONE_TRANFORM_HLSL_
#define BONE_TRANFORM_HLSL_

/////////////////////////////////////////////////////////////////////////////////

#include "Quaternion.hlsl"

/////////////////////////////////////////////////////////////////////////////////

struct BoneTransform
{
    float3 pos;
    Quaternion rot;
    float3 scale;

    static const uint size = (3 + 4 + 3) * 4;

/////////////////////////////////////////////////////////////////////////////////

    static BoneTransform Identity()
    {
        BoneTransform rv;
        rv.pos = 0;
        rv.rot.value = float4(0, 0, 0, 1);
        rv.scale = 1;
        return rv;
    }

/////////////////////////////////////////////////////////////////////////////////

    static BoneTransform Multiply(BoneTransform parent, BoneTransform child)
    {
   		BoneTransform rv;
		rv.pos = Quaternion::Rotate(parent.rot, child.pos * parent.scale) + parent.pos;
		rv.rot = Quaternion::Multiply(parent.rot, child.rot);
		rv.scale = parent.scale * child.scale;
		return rv;
    }

/////////////////////////////////////////////////////////////////////////////////

    static BoneTransform Scale(BoneTransform v, float3 scale)
    {
        v.pos *= scale.x;
        v.rot.value *= scale.y;
        v.scale *= scale.z;
        return v;
    }

/////////////////////////////////////////////////////////////////////////////////

    float4x4 ToFloat4x4()
    {
        float3x3 rotMat = rot.ToRotationMatrix();
        float4 c0 = float4(rotMat._11_21_31 * scale, pos.x);
        float4 c1 = float4(rotMat._12_22_32 * scale, pos.y);
        float4 c2 = float4(rotMat._13_23_33 * scale, pos.z);
        float4 c3 = float4(0, 0, 0, 1);

        float4x4 rv = float4x4(c0, c1, c2, c3);
        return rv;
    }

/////////////////////////////////////////////////////////////////////////////////

    static BoneTransform Inverse(BoneTransform v)
    {
        BoneTransform rv;
        rv.rot = Quaternion::Inverse(v.rot);
		rv.scale = rcp(v.scale);
		rv.pos = Quaternion::Rotate(rv.rot, -v.pos * rv.scale);
        return rv;
    }

/////////////////////////////////////////////////////////////////////////////////

    static float3 Transform(BoneTransform bt, float3 v)
    {
        float3 rv = Quaternion::Rotate(bt.rot, v) + bt.pos;
        return rv;
    }
};

/////////////////////////////////////////////////////////////////////////////////

#endif


