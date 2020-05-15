﻿Shader "Custom/PseudoStandartWithRimLight"
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
		[Normal] _DetailNormalMap("Detail Normal Map", 2D) = "bump" { }
        [MaterialToggle] _IsDetailsProvided("Is Details Provided", Float) = 0

		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		[MaterialToggle] _IsRimLightEnabled("Is Rim Light Enabled", Float) = 0

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
				float3 viewDir;
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
            
            int _IsDetailsProvided;

			float4 _RimColor;
			float _RimPower;
			int _IsRimLightEnabled;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 sec_c = tex2D(_DetailAlbedoMap, IN.uv_DetailAlbedoMap);

			    o.Albedo = c.rgb * (_IsDetailsProvided ? sec_c.rgb : 1);

				float3 normal_map = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				normal_map *= float3(_BumpScale, _BumpScale, 1);
				
				float3 detail_normal_map = UnpackNormal(tex2D(_DetailNormalMap, IN.uv_DetailNormalMap));
			    detail_normal_map *= float3(_DetailNormalMapScale, _DetailNormalMapScale, 1);

				o.Normal = normal_map + (_IsDetailsProvided ? detail_normal_map : 0);

				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;

				if (_IsRimLightEnabled) {
					half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
					o.Emission = _RimColor.rgb * pow(rim, _RimPower);
				}
			}
			ENDCG
		}
			FallBack "Diffuse"
}
