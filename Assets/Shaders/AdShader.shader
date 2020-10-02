// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/AdShader" {
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_PaperTex("Paper Texture", 2D) = "white" {}
		_MainTextureIntensityMap("MainTextureIntensityMap", 2D) = "white" {}

		_MainTextureIntensityPower("MainTextureIntensityPower", Range(-5,5)) = 1

		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		[MaterialToggle] _IsRimLightEnabled("Is Rim Light Enabled", Float) = 0
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			struct Input {
				float2 uv_MainTex;
				float2 uv_PaperTex;
				float2 uv_MainTextureIntensityMap;
				float3 viewDir;
			};

			sampler2D _MainTex;
			sampler2D _PaperTex;
			sampler2D _MainTextureIntensityMap;

			float4 _RimColor;
			float _RimPower;
			float _MainTextureIntensityPower;
			int _IsRimLightEnabled;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {

				fixed4 main_color = tex2D(_MainTex, IN.uv_MainTex);
				fixed4 main_color_intensity_map_value = tex2D(_MainTextureIntensityMap, IN.uv_MainTextureIntensityMap);
				fixed4 paper_color = tex2D(_PaperTex, IN.uv_PaperTex);

				fixed intensity = (main_color_intensity_map_value.r - 0.5) * _MainTextureIntensityPower;

				o.Albedo = (main_color + (1.0 - main_color.r) * intensity) * paper_color;

				if (_IsRimLightEnabled) {
					half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
					o.Emission = _RimColor.rgb * pow(rim, _RimPower);
				}
			}
			ENDCG
		}
		FallBack "Diffuse"
}
