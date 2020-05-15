Shader "Custom/PseudoStandartWithPainting"
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

		_PaintingTex("Painting Texture", 2D) = "white" {}
		_PaintingPower("Painting Power", Range(0,1)) = 1.0
		[MaterialToggle] _IsPaintingOn("Is Painting On", Float) = 0
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
				float2 uv_PaintingTex;
			};

			half _Glossiness;
			half _Metallic;

			half _BumpScale;
			half _DetailNormalMapScale;
			half _PaintingPower;

			fixed4 _Color;
			fixed4 _EmissionColor;

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _DetailAlbedoMap;
			sampler2D _DetailNormalMap;
			sampler2D _PaintingTex;
			int _IsPaintingOn;
            
            int _IsDetailsProvided;

			UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_INSTANCING_BUFFER_END(Props)

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				fixed4 sec_c = tex2D(_DetailAlbedoMap, IN.uv_DetailAlbedoMap);
				fixed4 painting_c = tex2D(_PaintingTex, IN.uv_PaintingTex);

			    o.Albedo = c.rgb * (_IsDetailsProvided ? sec_c.rgb : 1);
				o.Albedo = o.Albedo - (_IsPaintingOn ? _PaintingPower * painting_c.a : 0);

				float3 normal_map = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				normal_map *= float3(_BumpScale, _BumpScale, 1);

				float3 detail_normal_map = UnpackNormal(tex2D(_DetailNormalMap, IN.uv_DetailNormalMap));
				detail_normal_map *= float3(_DetailNormalMapScale, _DetailNormalMapScale, 1);

			    o.Normal = normal_map + (_IsDetailsProvided ? detail_normal_map : 0);

				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;

				o.Emission = _EmissionColor;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}
