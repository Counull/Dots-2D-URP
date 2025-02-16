Shader "BoneRendererGPU_URP"
{
SubShader
{
    PackageRequirements
    {
        "com.unity.render-pipelines.universal": "1.0.0"
    }
	Tags
	{
        "RenderPipeline"="UniversalPipeline"
		"Queue" = "Transparent+0"
	}

	Pass
	{
		Tags
		{
            "LightMode" = "UniversalForward"
		}

        Blend SrcAlpha OneMinusSrcAlpha
		ZTest off

		HLSLPROGRAM
		#pragma target 5.0

		#pragma vertex VSGPUAnimator
		#pragma fragment PS

		#include "BoneRendererGPU.hlsl"

		ENDHLSL
	}
}
}
