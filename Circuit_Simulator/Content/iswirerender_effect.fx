#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

matrix WorldViewProjection;
Texture2D tex;
//int state;

sampler2D tex_samp = sampler_state
{
	Texture = <tex>;
	MipFilter = POINT;
	MinFilter = POINT;
	MagFilter = POINT;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float layers : COLOR0;
	//float4 Color : COLOR0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float layers : COLOR0;
	//float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	//output.Position = (input.Position / float4(960, -540, 1, 1)) + float4(-1, 1, 0, 0);
	output.Position = mul(input.Position, WorldViewProjection);
	output.layers = input.layers;
	//output.Color = float4(1, 0, 0, 1);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 OUT = float4(0, 0, 0, 0);
	uint tex_dat = tex[uint2(input.Position.x, input.Position.y)].a * 255.5f;

	if (tex_dat)
		OUT = float4(0, 0, 0, 0.5f);

	uint tex_datm1m1 = tex[uint2(input.Position.x - 1, input.Position.y - 1)].a * 255.5f;
	uint tex_datp1m1 = tex[uint2(input.Position.x + 1, input.Position.y - 1)].a * 255.5f;
	uint tex_datm1p1 = tex[uint2(input.Position.x - 1, input.Position.y + 1)].a * 255.5f;
	uint tex_datp1p1 = tex[uint2(input.Position.x + 1, input.Position.y + 1)].a * 255.5f;


	for (int i = 0; i < 8; ++i)
	{
		if ((tex_dat & (1 << i)) && (tex_datm1m1 & (1 << i)))
			OUT = float4(0, 0, 0, 1.0f);
		if ((tex_dat & (1 << i)) && (tex_datp1m1 & (1 << i)))
			OUT = float4(0, 0, 0, 1.0f);
		if ((tex_dat & (1 << i)) && (tex_datm1p1 & (1 << i)))
			OUT = float4(0, 0, 0, 1.0f);
		if ((tex_dat & (1 << i)) && (tex_datp1p1 & (1 << i)))
			OUT = float4(0, 0, 0, 1.0f);
	}

	return OUT;

}

technique BasicColorDrawing
{
	pass P0
	{
		AlphaBlendEnable = false;
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};