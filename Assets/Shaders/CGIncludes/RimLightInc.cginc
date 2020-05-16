float4 _RimColor;
float _RimPower;
int _IsRimLightEnabled;

void AddRimLight(float3 viewDir, inout SurfaceOutputStandard o) 
{
    if (_IsRimLightEnabled) {
        half rim = 1.0 - saturate(dot(normalize(viewDir), o.Normal));
        o.Emission = _RimColor.rgb * pow(rim, _RimPower);
    }
}
