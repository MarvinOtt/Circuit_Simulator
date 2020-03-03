#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

#define PINOFF 4

texture2D logictex, wirecalctex, isedgetex;
texture2D placementtex;
texture2D comptex, highlighttex;
texture2D copywiretex, copycomptex;

int currentlayer;
float zoom;
float2 coos;
int currenttype, selectstate;
int copiedwidth, copiedheight, copiedposx, copiedposy; // Copying
int selection_startX, selection_endX, selection_startY, selection_endY; // Selecting
int Screenwidth, Screenheight, worldsizex, worldsizey, mousepos_X, mousepos_Y;
int copyposX, copyposY;

static float4 layercols[8] =
{
	float4(1, 0, 0, 1),
	float4(0, 1, 0, 1),
	float4(0, 0, 1, 1),
	float4(1, 1, 0, 1),
	float4(1, 0, 1, 1),
	float4(0, 1, 1, 1),
	float4(1, 0.5f, 0, 1),
	float4(1, 1, 0, 1)
};
static float4 compcols[5] =
{
	float4(0.5f, 0.5f, 0.5f, 1),
	float4(0.25f, 0.25f, 0.25f, 1),
	float4(0.95f, 0.95f, 0.95f, 1),
	float4(0.15f, 0.15f, 0.15f, 1),
	float4(1, 1, 0, 1)
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
inline uint getWiresAtPos(uint2 pos)
{
	return (uint)(logictex[pos].r + 0.5f);
}

const float mindist = 0.125f;

inline float mindist2line(float2 v, float2 w, float2 p) 
{
	float l = pow(v.x - w.x, 2) + pow(v.y - w.y, 2);
	float t = max(0, min(1, dot(p - v, w - v) / l));
	float2 projection = v + t * (w - v);
	return length(p - projection);
}

inline bool IsCloseToLine(float2 v, float2 w, float2 p, float mindis)
{
	return mindist2line(v, w, p) < mindis;
}

float4 ColorInPixel(uint type_int)
{
	if (type_int >= 128)
		return float4(1, 1, 1, 1);
	else if ((type_int & (1 << currentlayer)) == (1 << currentlayer))
		return layercols[currentlayer];
	else if ((type_int & 1) > 0)
		return layercols[0];
	else if ((type_int & 2) > 0)
		return layercols[1];
	else if ((type_int & 4) > 0)
		return layercols[2];
	else if ((type_int & 8) > 0)
		return layercols[3];
	else if ((type_int & 16) > 0)
		return layercols[4];
	else if ((type_int & 32) > 0)
		return layercols[5];
	else if ((type_int & 64) > 0)
		return layercols[6];
	else
		return float4(0, 0, 0, 1);
}
float4 ColorInPixel_Comp(uint type_int2)
{
	float4 OUT = float4(0, 0, 0, 0);
	if (type_int2 != 0)
	{
		if (type_int2 <= 4)
			OUT = compcols[type_int2 - 1];
		else
			OUT = compcols[4];
	}
	return OUT;
}

bool IsNotCompMask(uint comptype_int, uint comptype_edge, uint edge_state, bool edge1, bool edge2)
{
	bool OUT = false;
	if(comptype_int > PINOFF) OUT = OUT || ((((comptype_edge > PINOFF && comptype_int != comptype_edge) || !comptype_edge) || !edge_state) && edge1);
	OUT = OUT || ((!edge_state && comptype_edge <= 2 && comptype_edge) && edge2);
	return OUT;
}

float4 getcoloratpos(float x, float y)
{
	uint IsWire = 0;
	uint ux = (uint)x;
	uint uy = (uint)y;
	float2 xy = float2(x, y);
	float2 xymid = float2(ux + 0.5f, uy + 0.5f);

	float4 OUT = float4(0, 0, 0, 0);
	float edgewidth = 0.035f;
	if (zoom > 1)
	{
		if ((x % 10.0f >= 10 - edgewidth || x % 10.0f <= edgewidth))// || (y % 10.0f >= 10 - factor || y % 10.0f <= factor))
			OUT = float4(0.2f, 0.2f, 0.2f, 0);
		else if ((y % 10.0f >= 10 - edgewidth || y % 10.0f <= edgewidth))
			OUT = float4(0.2f, 0.2f, 0.2f, 0);
		else if (zoom > 4)
		{
			if (x % 1 >= 1 - edgewidth || x % 1 <= edgewidth)
				OUT = float4(0.08f, 0.08f, 0.08f, 0);
			if (y % 1 >= 1 - edgewidth || y % 1 <= edgewidth)
				OUT = float4(0.08f, 0.08f, 0.08f, 0);
		}
	}

	uint type_int = getWiresAtPos(uint2(ux, uy));
	uint comptype_int = (uint)(comptex[uint2(ux, uy)].r + 0.5f);

	if (zoom <= 1)
	{
		if (type_int > 0)
			OUT = ColorInPixel(type_int);
		if (comptype_int > 0)
			OUT = ColorInPixel_Comp(comptype_int);
	}
	else
	{
		if (comptype_int != 0)
		{
			uint comptype_p10 = (uint)(comptex[uint2(ux + 1, uy)].r + 0.5f);
			uint comptype_m10 = (uint)(comptex[uint2(ux - 1, uy)].r + 0.5f);
			uint comptype_0p1 = (uint)(comptex[uint2(ux, uy + 1)].r + 0.5f);
			uint comptype_0m1 = (uint)(comptex[uint2(ux, uy - 1)].r + 0.5f);

			uint IsValid = 1;

			uint edge_state = (((uint)(isedgetex[uint2(ux + 1, uy)].r + 0.5f)) & (1 << 0));
			if (IsNotCompMask(comptype_int, comptype_p10, edge_state, x % 1 > 0.7f, x % 1 > 0.965f))
				IsValid = 0;
			edge_state = (((uint)(isedgetex[uint2(ux - 1, uy)].r + 0.5f)) & (1 << 2));
			if (IsNotCompMask(comptype_int, comptype_m10, edge_state, x % 1 < 0.3f, x % 1 < 0.035f))
				IsValid = 0;
			edge_state = (((uint)(isedgetex[uint2(ux, uy + 1)].r + 0.5f)) & (1 << 1));
			if (IsNotCompMask(comptype_int, comptype_0p1, edge_state, y % 1 > 0.7f, y % 1 > 0.965f))
				IsValid = 0;
			edge_state = (((uint)(isedgetex[uint2(ux, uy - 1)].r + 0.5f)) & (1 << 3));
			if (IsNotCompMask(comptype_int, comptype_0m1, edge_state, y % 1 < 0.3f, y % 1 < 0.035f))
				IsValid = 0;

			if (IsValid)
			{
				if (comptype_int <= PINOFF)
					OUT = compcols[comptype_int - 1];
				else
					OUT = compcols[4];
			}
		}
		else 
		{
			if(type_int >= 128)
				OUT = float4(1, 1, 1, 1);
			else
				OUT.a = 0.2f;
		}

		float NeedsWireCalc = wirecalctex[uint2(ux, uy)].r;
		float NeedsWireCalc_m1 = wirecalctex[uint2(ux - 1, uy)].r;
		float NeedsWireCalc_p1 = wirecalctex[uint2(ux + 1, uy)].r;
		[branch]if (OUT.a < 0.5f && (NeedsWireCalc > 0.1f || NeedsWireCalc_m1 > 0.75f || NeedsWireCalc_p1 > 0.75f))
		{
			uint type_intm1m1 = getWiresAtPos(uint2(ux - 1, uy - 1));
			uint type_int0m1 = getWiresAtPos(uint2(ux, uy - 1));
			uint type_intp1m1 = getWiresAtPos(uint2(ux + 1, uy - 1));

			uint type_intm10 = getWiresAtPos(uint2(ux - 1, uy));
			uint type_intp10 = getWiresAtPos(uint2(ux + 1, uy));

			uint type_intm1p1 = getWiresAtPos(uint2(ux - 1, uy + 1));
			uint type_int0p1 = getWiresAtPos(uint2(ux, uy + 1));
			uint type_intp1p1 = getWiresAtPos(uint2(ux + 1, uy + 1));

			uint high_int = (uint)(highlighttex[uint2(ux, uy)].r + 0.5f);

			uint high_intm1m1 = (uint)(highlighttex[uint2(ux - 1, uy - 1)].r + 0.5f);
			uint high_int0m1 = (uint)(highlighttex[uint2(ux, uy - 1)].r + 0.5f);
			uint high_intp1m1 = (uint)(highlighttex[uint2(ux + 1, uy - 1)].r + 0.5f);
			uint high_intm10 = (uint)(highlighttex[uint2(ux - 1, uy)].r + 0.5f);
			uint high_intp10 = (uint)(highlighttex[uint2(ux + 1, uy)].r + 0.5f);
			uint high_intm1p1 = (uint)(highlighttex[uint2(ux - 1, uy + 1)].r + 0.5f);
			uint high_int0p1 = (uint)(highlighttex[uint2(ux, uy + 1)].r + 0.5f);
			uint high_intp1p1 = (uint)(highlighttex[uint2(ux + 1, uy + 1)].r + 0.5f);
			float IsHighlight = 0.0f;
			[unroll]
			for (int ii = 6; ii >= -1; --ii)
			{
				int i = ii;
				if (ii == -1)
					i = currentlayer;

				uint WireType = type_int & (1 << i);
				uint state_m1m1 = (type_intm1m1 & (1 << i));
				uint state_0m1 = (type_int0m1 & (1 << i));
				uint state_p1m1 = (type_intp1m1 & (1 << i));
				uint state_p10 = (type_intp10 & (1 << i));
				uint state_p1p1 = (type_intp1p1 & (1 << i));
				uint state_0p1 = (type_int0p1 & (1 << i));
				uint state_m1p1 = (type_intm1p1 & (1 << i));
				uint state_m10 = (type_intm10 & (1 << i));
				float mindisthigh = 0.3f;
				[branch]
				if (WireType)
				{
					IsWire = 1;

					if (WireType == state_m10)
					{
						if(IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y), xy, mindist))
							OUT = layercols[i];
						if (high_int > 0.1f && high_intm10 > 0.1f && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y), xy, mindisthigh))
							IsHighlight = 1.0f;
					}
					if (WireType == state_p10)
					{
						if(IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y), xy, mindist))
							OUT = layercols[i];
						if (high_int > 0.1f && high_intp10 > 0.1f && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y), xy, mindisthigh))
							IsHighlight = 1.0f;
					}
					if (WireType == state_0m1)
					{
						if(IsCloseToLine(xymid, float2(xymid.x, xymid.y - 1), xy, mindist))
							OUT = layercols[i];
						if (high_int > 0.1f && high_int0m1 > 0.1f && IsCloseToLine(xymid, float2(xymid.x, xymid.y - 1), xy, mindisthigh))
							IsHighlight = 1.0f;
					}
					if (WireType == state_0p1)
					{
						if(IsCloseToLine(xymid, float2(xymid.x, xymid.y + 1), xy, mindist))
							OUT = layercols[i];
						if (high_int > 0.1f && high_int0p1 > 0.1f && IsCloseToLine(xymid, float2(xymid.x, xymid.y + 1), xy, mindisthigh))
							IsHighlight = 1.0f;
					}

					if (WireType == state_m1m1 && !state_0m1 && !state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y - 1), xy, mindist))
						OUT = layercols[i];
					if (WireType == state_p1m1 && !state_0m1 && !state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y - 1), xy, mindist))
						OUT = layercols[i];
					if (WireType == state_m1p1 && !state_0p1 && !state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y + 1), xy, mindist))
						OUT = layercols[i];
					if (WireType == state_p1p1 && !state_0p1 && !state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y + 1), xy, mindist))
						OUT = layercols[i];

					int count = 0;
					if (state_m10)
						count++;
					if (state_p10)
						count++;
					if (state_0m1)
						count++;
					if (state_0p1)
						count++;

					if (state_m1p1 && !state_m10 && !state_0p1)
						count++;
					if (state_p1p1 && !state_0p1 && !state_p10)
						count++;
					if (state_p1m1 && !state_0m1 && !state_p10)
						count++;
					if (state_m1m1 && !state_m10 && !state_0m1)
						count++;

					if ((count > 2 || (!count && comptype_int == 0)) && IsCloseToLine(xymid, xymid, xy, min(mindist * 1.75f, 0.5f)))
						OUT = layercols[i];
				}
				if (state_0m1 && state_p10 && !state_p1m1 && !WireType && IsCloseToLine(float2(xymid.x, xymid.y - 1), float2(xymid.x + 1, xymid.y), xy, mindist))
					OUT = layercols[i];
				if (state_0p1 && state_m10 && !state_m1p1 && !WireType && IsCloseToLine(float2(xymid.x, xymid.y + 1), float2(xymid.x - 1, xymid.y), xy, mindist))
					OUT = layercols[i];

				if (state_m10 && state_0m1 && !state_m1m1 && !WireType && IsCloseToLine(float2(xymid.x - 1, xymid.y), float2(xymid.x, xymid.y - 1), xy, mindist))
					OUT = layercols[i];
				if (state_p10 && state_0p1 && !state_p1p1 && !WireType && IsCloseToLine(float2(xymid.x + 1, xymid.y), float2(xymid.x, xymid.y + 1), xy, mindist))
					OUT = layercols[i];

			}
			//if (IsHighlight > 0.5f)
			//	OUT = OUT * 0.5f + float4(1, 1, 1, 1) * 0.5f;
		}
	}

	if (currenttype == 1)
	{
		uint posx = ux - mousepos_X + 40;
		uint posy = uy - mousepos_Y + 40;
		if (posx >= 0 && posx < 82 && posy >= 0 && posy < 82)
		{
			uint type = (uint)(placementtex[uint2(posx, posy)].r + 0.5f);
			if (type != 0)
			{
				if ((OUT.a > 0.5f && !IsWire) || (OUT.a > 0.5f && type <= 4))
					OUT = float4(1, 0, 0, 1);
				else
				{
					if (type <= 4)
						OUT = compcols[type - 1];
					else
						OUT = compcols[4];
				}
			}
		}
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
		uint2 abscoo = uint2((xcoo - coos.x) / zoom, (ycoo - coos.y) / zoom);
		OUT = getcoloratpos((xcoo - coos.x) / zoom, (ycoo - coos.y) / zoom);
		float highlight_state = highlighttex[uint2(abscoo.x, abscoo.y)].r;
		if (highlight_state > 0.0001f)
			OUT = OUT * 0.5f + float4(1, 1, 1, 1) * 0.5f;

		if (selectstate >= 1 && selectstate <= 2)
		{
			if (abscoo.x >= selection_startX && abscoo.x <= selection_endX && abscoo.y >= selection_startY && abscoo.y <= selection_endY)
			{
				OUT = OUT * 0.75f + float4(1, 1, 1, 1) * 0.25f;
			}
		}
		if (selectstate == 4)
		{
			if (abscoo.x >= copyposX && abscoo.x <= selection_endX && abscoo.y >= copyposY && abscoo.y <= selection_endY)
			{
				uint wire_type_int2 = (uint)(copywiretex[uint2(abscoo.x - copyposX, abscoo.y - copyposY)].r + 0.5f);
				uint comp_type_int2 = (uint)(copycomptex[uint2(abscoo.x - copyposX, abscoo.y - copyposY)].r + 0.5f);
				float4 newcol = ColorInPixel(wire_type_int2);
				float4 compcol = ColorInPixel_Comp(comp_type_int2);
				if (compcol.a > 0.5f)
					OUT = (compcol) * 0.75f + float4(1, 0, 0, 1) * 0.25f;
				else
					OUT = (OUT * 0.5f + newcol * 0.5f) * 0.75f + float4(1, 0, 0, 1) * 0.25f;
			}
		}
	}
	else
		OUT = float4(0.25f, 0.25f, 0.25f, 1);

	OUT.a = 1.0f;

	return OUT + tex2D(SpriteTextureSampler, input.TextureCoordinates) * 0.00001f;// +tex2D(SpriteTextureSampler, input.TextureCoordinates);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};