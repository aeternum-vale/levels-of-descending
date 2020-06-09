Shader "Custom/PseudoStandart"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_MetallicMap("Metallic Map", 2D) = "white" {}
		[MaterialToggle] _IsMetallicMapProvided("Is Metallic Map Provided", Float) = 0
		_EmissionColor("Emission Color", Color) = (0,0,0)
		_OcclusionMap("Occlusion Map", 2D) = "white" {}
		[MaterialToggle] _IsOcclusionMapProvided("Is Occlusion Map Provided", Float) = 0

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		_DetailNormalMapScale("Detail Normal Map Scale", Float) = 1
		[Normal] _DetailNormalMap("Detail Normal Map", 2D) = "bump" { }
        [MaterialToggle] _IsDetailsProvided("Is Details Provided", Float) = 0
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float2 uv_DetailAlbedoMap;
			float2 uv_DetailNormalMap;
		};
		
		#include "CGIncludes/PseudoStandartInc.cginc"

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			PseudoStandartSurf(IN, o);
		}
		ENDCG
	}
		FallBack "Diffuse"
}
