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

void PseudoStandartSurf(Input IN, inout SurfaceOutputStandard o)
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

    o.Emission = _EmissionColor;
    o.Alpha = c.a;
}

