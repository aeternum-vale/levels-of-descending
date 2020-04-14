﻿Shader "Custom/PseudoStandart"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		_BumpScale("Scale", Float) = 1.0
		[Normal] _BumpMap("Normal Map", 2D) = "bump" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_EmissionColor("Emission Color", Color) = (0,0,0)

		_DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		 _DetailNormalMapScale("Detail Normal Map Scale", Float) = 1
		 [Normal]  _DetailNormalMap("Detail Normal Map", 2D) = "bump" { }

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

		half _Glossiness;
		half _Metallic;

		half _BumpScale;
		half _DetailNormalMapScale;

		fixed4 _Color;
		fixed4 _EmissionColor;

		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _DetailAlbedoMap;
		sampler2D _DetailNormalMap;


		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			fixed4 sec_c = tex2D(_DetailAlbedoMap, IN.uv_DetailAlbedoMap);

			o.Albedo = c.rgb * sec_c.rgb;

			o.Normal = UnpackNormal(tex2D(_DetailNormalMap, IN.uv_DetailNormalMap));
			o.Normal *= float3(_DetailNormalMapScale, _DetailNormalMapScale, 1);

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;

			o.Emission = _EmissionColor;
			o.Alpha = c.a;
		}
		ENDCG
	}
		FallBack "Diffuse"
}
