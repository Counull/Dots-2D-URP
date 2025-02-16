Shader "BoneRendererGPU_HDRP"
{
SubShader
{
    PackageRequirements
    {
        "com.unity.render-pipelines.high-definition": "1.0.0"
    }
	Tags
	{
		"RenderPipeline" = "HDRenderPipeline"
		"RenderType" = "HDUnlitShader"
		"Queue" = "Transparent+0"
	}

	Pass
	{
		Name "ForwardOnly"
		Tags
		{
			"LightMode" = "ForwardOnly"
		}

        Blend SrcAlpha OneMinusSrcAlpha
		ZTest off

		HLSLPROGRAM
		#pragma target 5.0
        //#pragma enable_d3d11_debug_symbols

		#pragma vertex VSGPUAnimator
		#pragma fragment PS
        #define IS_HDRP

		#include "BoneRendererGPU.hlsl"

		ENDHLSL
	}
}
}
