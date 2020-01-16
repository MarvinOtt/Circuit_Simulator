#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;
texture2D logictex;
float zoom;
float2 coos;
int Screenwidth, Screenheight, worldsizex, worldsizey, mousepos_X, mousepos_Y;
int origin_X, origin_Y;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

static float4 compcols[5] =
{
	float4(0.5f, 0.5f, 0.5f, 1),
	float4(0.25f, 0.25f, 0.25f, 1),
	float4(0.9f, 0.9f, 0.9f, 1),
	float4(0.1f, 0.1f, 0.1f, 1),
	float4(1, 1, 0, 1)
};

float4 getcoloratpos(float x, float y)
{
	uint ux = (uint)x;
	uint uy = (uint)y;
	float4 OUT = float4(0, 0, 0, 1);
	float type = logictex[uint2(ux, uy)].a * 255.0f;
	if (type < 0.5f) { /*Do Nothing*/ }
	else if (type < 1.5f)
		OUT = compcols[0];
	else if (type < 2.5f)
		OUT = compcols[1];
	else if (type < 3.5f)
		OUT = compcols[2];
	else if (type < 4.5f)
		OUT = compcols[3];
	else
		OUT = compcols[4];


	if (zoom > 2)
	{
		float factor = 0.8f / zoom;
		if ((x % 10.0f >= 10 - factor || x % 10.0f <= factor) || (y % 10.0f >= 10 - factor || y % 10.0f <= factor))
			OUT = float4(0.15f, 0.15f, 0.15f, 1);
		else if (zoom > 4 && ((x % 1 >= 1 - factor || x % 1 <= factor) || (y % 1 >= 1 - factor || y % 1 <= factor)))
			OUT = float4(0.04f, 0.04f, 0.04f, 1);
	}
	if ((x >= mousepos_X && x <= mousepos_X + 1) || (y >= mousepos_Y && y <= mousepos_Y + 1))
	{
		OUT = OUT * 0.85f + float4(1, 1, 1, 1) * 0.15f;
	}
	if (ux == origin_X && uy == origin_Y)
	{
		float fac = pow(max(0.5f - length(float2(x - 0.5f, y - 0.5f) - float2(ux, uy)), 0) * 2.0f + 0.5f, 10) * 0.5;
		OUT = OUT * (1.0f - fac) + float4(1, 1, 1, 1) * fac;
	}
	return OUT;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float4 OUT = float4(0, 0, 0, 1);
	uint xcoo = input.TextureCoordinates.x * Screenwidth;
	uint ycoo = input.TextureCoordinates.y * Screenheight;

	if (xcoo >= coos.x && xcoo <= coos.x + worldsizex * zoom && ycoo >= coos.y && ycoo <= coos.y + worldsizey * zoom)
	{
		OUT = getcoloratpos((xcoo - coos.x) / zoom, (ycoo - coos.y) / zoom);
	}
	else
		OUT = float4(0.25f, 0.25f, 0.25f, 1);

	return OUT + tex2D(SpriteTextureSampler, input.TextureCoordinates) * 0.000001f;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};