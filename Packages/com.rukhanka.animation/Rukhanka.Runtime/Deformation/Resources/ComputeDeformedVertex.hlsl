#pragma once

/////////////////////////////////////////////////////////////////////////////////

#include "DeformationCommon.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"

#if defined(DOTS_INSTANCING_ON)
StructuredBuffer<PackedDeformedVertex> _DeformedMeshData;

UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
    UNITY_DOTS_INSTANCED_PROP(float, _ComputeMeshIndex)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)
#endif

/////////////////////////////////////////////////////////////////////////////////

void ComputeDeformedVertex_float(in uint vertexID, in float3 vertex, in float3 normal, in float3 tangent, out float3 deformedVertex, out float3 deformedNormal, out float3 deformedTangent)
{
    deformedVertex = vertex;
    deformedNormal = normal;
    deformedTangent = tangent;

#if defined(DOTS_INSTANCING_ON)
    uint index = asuint(UNITY_ACCESS_DOTS_INSTANCED_PROP(float, _ComputeMeshIndex));
    PackedDeformedVertex vertexData = _DeformedMeshData[index + vertexID];
    DeformedVertex v = vertexData.Unpack();
    deformedVertex = v.position;
    deformedNormal = v.normal;
    deformedTangent = v.tangent;
#endif
}
