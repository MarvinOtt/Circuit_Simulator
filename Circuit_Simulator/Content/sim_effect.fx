﻿#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0
	#define PS_SHADERMODEL ps_4_0
#endif

texture2D logictex_L1;
texture2D logictex_L2;
texture2D logictex_L3;
texture2D logictex_L4;
texture2D logictex_L5;
texture2D logictex_L6;
texture2D logictex_L7;

static texture2D logictex_L[7] =
{
	logictex_L1,
	logictex_L2,
	logictex_L3,
	logictex_L4,
	logictex_L5,
	logictex_L6,
	logictex_L7
};



texture2D logictex_LV;

int layers = 7;
float zoom;
float2 coos;
int currentselectiontype;
int copiedwidth, copiedheight, copiedposx, copiedposy; // Copying
//int Selection_StartX, Selection_EndX, Selection_StartY, Selection_EndY; // Selecting
int Screenwidth, Screenheight, worldsizex, worldsizey, mousepos_X, mousepos_Y;

static float4 layercols[8] =
{
	float4(1, 0, 0, 1),
	float4(0, 1, 0, 1),
	float4(0, 0, 1, 1),
	float4(1, 1, 0, 1),
	float4(1, 0, 1, 1),
	float4(0, 1, 1, 1),
	float4(1, 0.5f, 0, 1),
	float4(0.2f, 0, 0.8f, 1)
};

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

float4 getcoloratpos(float x, float y)
{
	uint ux = (uint)x;
	uint uy = (uint)y;
	float4 OUT = float4(0, 0, 0, 1);
	float type_V = logictex_LV[uint2(ux, uy)].a;
	if (type_V > 0.5f)
		OUT = float4(1, 1, 1, 1);
	else
	{
		for (int i = 0; i < layers; ++i)
		{
			float type = logictex_L[i][uint2(ux, uy)].a;

			if (type > 0.5f)
			{
				OUT = layercols[i];
				break;
			}
		}
	}

	/*float type_L1 = logictex_L1[uint2(ux, uy)].a * 255.0f;
	if (type_L1 > 0.5f)
		OUT = layercols[0];*/

	//float type = 0;// logictex[uint2(ux, uy)].a * 255.0f;
	//uint type_int = (uint)(type + 0.5f);

	//if (type_int == 0) { /*Do Nothing*/ }
	//else if(type_int == 255)
	//	OUT = float4(1, 1, 1, 1);
	//else if ((type_int & 1) > 0)
	//	OUT = layercols[0];
	//else if ((type_int & 2) > 0)
	//	OUT = layercols[1];
	//else if ((type_int & 4) > 0)
	//	OUT = layercols[2];
	//else if ((type_int & 8) > 0)
	//	OUT = layercols[3];
	//else if ((type_int & 16) > 0)
	//	OUT = layercols[4];
	//else if ((type_int & 32) > 0)
	//	OUT = layercols[5];
	//else if ((type_int & 64) > 0)
	//	OUT = layercols[6];
	//else
	//	OUT = float4(1, 1, 1, 1);


	//uint value1 = ((uint)(type + 0.5f)) % 17;
	//uint value2 = ((uint)(type + 0.5f)) / 17;
	//if (value1 == 0 && value2 == 0) {/*Do Nothing*/ }
	////else if (value1 == 0 && value2 != 0)
	//	//OUT = float4(0.125f, 0.125f, 0.125f, 1.0f);
	//else if (type != 0)
	//	OUT = float4(type * 0.001f, 0, 0, 1);
	//else if (value1 > 0 && value1 < 5)
	//	OUT = float4(1, 1, 1, 1);
	//else if (value1 == 5)
	//	OUT = float4(0.24f, 0.47f, 0, 1);
	//else if (value1 == 6)
	//	OUT = float4(0.2f, 0.4f, 0.82f, 1);
	//else if (value1 == 7)
	//	OUT = float4(1.0f, 1.0f, 0, 1);
	//else if (value1 == 8)
	//	OUT = float4(0.294f, 0.196f, 0, 1);
	//else if (value1 == 9)
	//	OUT = float4(0.392f, 0.0f, 0.49f, 1);
	//else if (value1 == 10 || value1 == 11)
	//	OUT = float4(0.8f, 0, 0, 1);
	//else if (value1 == 12 || value1 == 13)
	//	OUT = float4(0.196f, 0, 0, 1);

	/*if (currentselectiontype == 2)
	{
		if (ux >= copiedposx && uy >= copiedposy && ux < copiedposx + copiedwidth && uy < copiedposy + copiedheight)
		{
			float type2 = CopyTexture[uint2(ux - copiedposx, uy - copiedposy)].a * 255.0f;
			uint value1 = ((uint)(type2 + 0.5f)) % 17;
			uint value2 = ((uint)(type2 + 0.5f)) / 17;
			if (value1 == 0 && value2 == 0)
				OUT = float4(0, 0, 0, 1);
			else if (value1 == 0 && value2 != 0)
				OUT = float4(0.125f, 0.125f, 0.125f, 1.0f);
			else if (value1 > 0 && value1 < 5)
				OUT = float4(1, 1, 1, 1);
			else if (value1 == 5)
				OUT = float4(0.24f, 0.47f, 0, 1);
			else if (value1 == 6)
				OUT = float4(0.2f, 0.4f, 0.82f, 1);
			else if (value1 == 7)
				OUT = float4(1.0f, 1.0f, 0, 1);
			else if (value1 == 8)
				OUT = float4(0.294f, 0.196f, 0, 1);
			else if (value1 == 9)
				OUT = float4(0.392f, 0.0f, 0.49f, 1);
			else if (value1 == 10 || value1 == 11)
				OUT = float4(0.8f, 0, 0, 1);
			else if (value1 == 12 || value1 == 13)
				OUT = float4(0.196f, 0, 0, 1);
			OUT = OUT * 0.85f + float4(1, 0, 0, 1) * 0.15f;
		}
	}*/

	if (zoom > 1)
	{
		float factor = 0.8f / zoom;
		if ((x % 10.0f >= 10 - factor || x % 10.0f <= factor) || (y % 10.0f >= 10 - factor || y % 10.0f <= factor))
			OUT = float4(0.15f, 0.15f, 0.15f, 1);
		else if (zoom > 4 && ((x % 1 >= 1 - factor || x % 1 <= factor) || (y % 1 >= 1 - factor || y % 1 <= factor)))
			OUT = float4(0.08f, 0.08f, 0.08f, 1);
	}
	if ((x >= mousepos_X && x <= mousepos_X + 1) || (y >= mousepos_Y && y <= mousepos_Y + 1))
	{
		OUT = OUT * 0.85f + float4(1, 1, 1, 1) * 0.15f;
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

	return OUT + tex2D(SpriteTextureSampler, input.TextureCoordinates);// +tex2D(SpriteTextureSampler, input.TextureCoordinates);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};