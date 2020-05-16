sampler2D _PaintingTex;
int _IsPaintingOn;
half _PaintingPower;

void AddPainting(float2 uv_PaintingTex, inout fixed3 albedo) 
{
    fixed4 painting_c = tex2D(_PaintingTex, uv_PaintingTex);
    albedo = albedo - (_IsPaintingOn ? _PaintingPower * painting_c.a : 0);
}
