#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

matrix WorldViewProjection;
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
	output.layers = input.layers / 256.0f;
	//output.Color = float4(1, 0, 0, 1);

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{

	return float4(0, 0, 0, input.layers);

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