Shader "Custom/FrontWallShader"
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


		_FloorNumber("Floor Number", Range(0,99)) = 0

		_FloorNumberFontTex("Floor Number Font Texture", 2D) = "white" {}
		_FloorNumberAreaX("Floor Number Area X", Range(0,1)) = 0.4
		_FloorNumberAreaY("Floor Number Area Y", Range(0,1)) = 0.7
		_FloorNumberAreaWidth("Floor Number Area Width", Float) = 0.2
		_FloorNumberAreaHeight("Floor Number Area Height", Float) = 0.2
		_FloorNumberRankDist("Floor Number Rank Distance", Range(-1,1)) = 1

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
				float2 uv_FloorNumberFontTex;
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
			sampler2D _FloorNumberFontTex;

			half _FloorNumber;
			half _FloorNumberAreaX;
			half _FloorNumberAreaY;
			half _FloorNumberAreaWidth;
			half _FloorNumberAreaHeight;
			half _FloorNumberRankDist;

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

				float bitmap_font_col_count = 4;
				float bitmap_font_row_count = 4;
				float floor_number_rank1 = trunc(_FloorNumber / 10);
				float floor_number_rank2 = trunc(_FloorNumber - floor_number_rank1 * 10);
				float floor_number_area_half_width = _FloorNumberAreaWidth / 2;

				float floor_number_area_x_2 = _FloorNumberAreaX + _FloorNumberRankDist;

				float2 cell_size = float2(1 / bitmap_font_col_count, 1 / bitmap_font_row_count);
				float2 first_cell_coords = float2(0, 1 - cell_size.y);

				float2 new_uv_scale = float2(cell_size.x / floor_number_area_half_width, cell_size.y / _FloorNumberAreaHeight);
				float2 new_uv_offset_1 = float2(first_cell_coords.x - (_FloorNumberAreaX * new_uv_scale.x), first_cell_coords.y - (_FloorNumberAreaY * new_uv_scale.y));
				float2 new_uv_offset_2 = float2(first_cell_coords.x - ((floor_number_area_x_2 + floor_number_area_half_width) * new_uv_scale.x), first_cell_coords.y - (_FloorNumberAreaY * new_uv_scale.y));

				float2 new_uv_font_offset_1 = float2(cell_size.x * (floor_number_rank1 % bitmap_font_col_count), -cell_size.y * trunc(floor_number_rank1 / bitmap_font_row_count));
				float2 new_uv_font_offset_2 = float2(cell_size.x * (floor_number_rank2 % bitmap_font_col_count), -cell_size.y * trunc(floor_number_rank2 / bitmap_font_row_count));

				float2 new_uv_1 = IN.uv_FloorNumberFontTex * new_uv_scale + new_uv_offset_1 + new_uv_font_offset_1;
				float2 new_uv_2 = IN.uv_FloorNumberFontTex * new_uv_scale + new_uv_offset_2 + new_uv_font_offset_2;

				bool is_first_rank_x = (IN.uv_MainTex.x > _FloorNumberAreaX) && (IN.uv_MainTex.x < (_FloorNumberAreaX + floor_number_area_half_width));
				bool is_second_rank_x = (IN.uv_MainTex.x > floor_number_area_x_2 + floor_number_area_half_width) && (IN.uv_MainTex.x < (floor_number_area_x_2 + _FloorNumberAreaWidth));
				bool is_in_area = ((is_first_rank_x || is_second_rank_x) && (IN.uv_MainTex.y > _FloorNumberAreaY) && (IN.uv_MainTex.y < (_FloorNumberAreaY + _FloorNumberAreaHeight)));

				fixed4 fn_c = is_in_area ? 
					((is_first_rank_x && is_second_rank_x) ? 
						1 - tex2D(_FloorNumberFontTex, new_uv_1).a - tex2D(_FloorNumberFontTex, new_uv_2).a
						: 1 - tex2D(_FloorNumberFontTex, is_first_rank_x ? new_uv_1 : new_uv_2).a)
					: 1;

				o.Albedo = c.rgb * sec_c.rgb * fn_c;
				// Metallic and smoothness come from slider variables

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
