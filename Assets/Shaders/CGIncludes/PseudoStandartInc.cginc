half _Metallic;
half _Glossiness;

half _BumpScale;

fixed4 _Color;
fixed4 _EmissionColor;

sampler2D _MainTex;

sampler2D _MetallicMap;
sampler2D _OcclusionMap;

sampler2D _BumpMap;

int _IsOcclusionMapProvided;
int _IsMetallicMapProvided;

void PseudoStandartSurf(Input IN, inout SurfaceOutputStandard o)
{
    fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

    o.Albedo = c.rgb;

    float3 normal_map = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
    normal_map *= float3(_BumpScale, _BumpScale, 1);

    o.Normal = normal_map;

    fixed4 metallic_map = tex2D(_MetallicMap, IN.uv_MainTex);
    o.Metallic = _IsMetallicMapProvided ? metallic_map : _Metallic;
    o.Smoothness = _IsMetallicMapProvided ? metallic_map.a * _Glossiness : _Glossiness;
    o.Occlusion = _IsOcclusionMapProvided ? tex2D(_OcclusionMap, IN.uv_MainTex) : 1;

    o.Emission = _EmissionColor;
    o.Alpha = c.a;
}

