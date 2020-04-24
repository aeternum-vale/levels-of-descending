// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/SignShader" {
	Properties{

		_MainTex1("Albedo (RGB)", 2D) = "white" {}
		_MainTex2("Albedo (RGB)", 2D) = "white" {}
		_MainTex3("Albedo (RGB)", 2D) = "white" {}
		_MainTex4("Albedo (RGB)", 2D) = "white" {}

		_ActiveTextureNumber("Active Texture", Range(1, 4)) = 1

		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
		[MaterialToggle] _IsSelected("Is Enabled", Float) = 0
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
				float2 uv_MainTex1;
				float2 uv_MainTex2;
				float2 uv_MainTex3;
				float2 uv_MainTex4;

				float3 viewDir;
			};

			sampler2D _MainTex1;
			sampler2D _MainTex2;
			sampler2D _MainTex3;
			sampler2D _MainTex4;

			half _ActiveTextureNumber;

			float4 _RimColor;
			float _RimPower;
			int _IsSelected;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o) {
				int tex_number = trunc(_ActiveTextureNumber);
				fixed4 c;

				switch (tex_number) {
					case 1:
						c = tex2D(_MainTex1, IN.uv_MainTex1);
						break;
					case 2:
						c = tex2D(_MainTex2, IN.uv_MainTex1);
						break;
					case 3:
						c = tex2D(_MainTex3, IN.uv_MainTex1);
						break;
					case 4:
						c = tex2D(_MainTex4, IN.uv_MainTex1);
						break;
				}


				o.Albedo = c.rgb;

				if (_IsSelected) {
					half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
					o.Emission = _RimColor.rgb * pow(rim, _RimPower);
				}
			}
			ENDCG
		}
			FallBack "Diffuse"
}
