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
	float type = logictex[pos].a * 255.0f;
	return (uint)(type + 0.5f);

}

float mindist = 0.125f;

float mindistline(float2 v, float2 w, float2 p) {
	// Return minimum distance between line segment vw and point p
	float l2 = pow(v.x - w.x, 2) + pow(v.y - w.y, 2);  // i.e. |w-v|^2 -  avoid a sqrt
	//if (l2 == 0.0) return length(p - v);   // v == w case
	// Consider the line extending the segment, parameterized as v + t (w - v).
	// We find projection of point p onto the line. 
	// It falls where t = [(p-v) . (w-v)] / |w-v|^2
	// We clamp t from [0,1] to handle points outside the segment vw.
	const float t = max(0, min(1, dot(p - v, w - v) / l2));
	const float2 projection = v + t * (w - v);  // Projection falls on the segment
	return length(p - projection);
}

bool IsCloseToLine(float2 v, float2 w, float2 p, float mindis)
{
	if (mindistline(v, w, p) < mindis)
		return true;
	return false;
}

uint IsEdgeX(float x, float y)
{
	uint2 xy_m10 = uint2((uint)(x - 0.5f), (uint)(y));
	uint2 xy_p10 = uint2((uint)(x + 0.5f), (uint)(y));
	uint IsComp_m10 = (uint)(comptex[xy_m10].a * 255.5f);
	uint IsComp_p10 = (uint)(comptex[xy_p10].a * 255.5f);
	if (IsComp_m10 && IsComp_p10)
	{
		return 2;
	}
	else if (IsComp_m10 || IsComp_p10)
		return 1;
	return 0;
}

uint IsEdgeY(float x, float y)
{
	uint2 xy_0m1 = uint2((uint)(x), (uint)(y - 0.5f));
	uint2 xy_0p1 = uint2((uint)(x), (uint)(y + 0.5f));
	uint IsComp_0m1 = (uint)(comptex[xy_0m1].a * 255.5f);
	uint IsComp_0p1 = (uint)(comptex[xy_0p1].a * 255.5f);
	if (IsComp_0m1 && IsComp_0p1)
	{
		return 2;
	}
	else if (IsComp_0m1 || IsComp_0p1)
		return 1;
	return 0;
}

float4 ColorInPixel(uint type_int)
{
	if (type_int == 255)
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
float4 ColorInPixel_Comp(uint type_int)
{
	float4 OUT = float4(0, 0, 0, 0);
	if (type_int != 0)
	{
		if (type_int <= 4)
			OUT = compcols[type_int - 1];
		else
			OUT = compcols[4];
	}
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
	//float type_V = logictex_LV[uint2(ux, uy)].a;
	//if (type_V > 0.5f)
	//	OUT = float4(1, 1, 1, 1);
	//else
	//{
	//	for (int i = 0; i < layers; ++i)
	//	{
	//		float type = logictex_L[i][uint2(ux, uy)].a;

	//		if (type > 0.5f)
	//		{
	//			OUT = layercols[i];
	//			break;
	//		}
	//	}
	//}

	/*float type_L1 = logictex_L1[uint2(ux, uy)].a * 255.0f;
	if (type_L1 > 0.5f)
		OUT = layercols[0];*/

	//float type = logictex[uint2(ux, uy)].a * 255.0f;

	

	/*i = currentlayer;
	uint WireType = type_int & (1 << i);
	if (WireType > 0)
	{
		if (WireType == (type_intm10 & (1 << i)) && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y), xy, mindist))
			OUT = layercols[i];
		if (WireType == (type_intp10 & (1 << i)) && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y), xy, mindist))
			OUT = layercols[i];
		if (WireType == (type_int0m1 & (1 << i)) && IsCloseToLine(xymid, float2(xymid.x, xymid.y - 1), xy, mindist))
			OUT = layercols[i];
		if (WireType == (type_int0p1 & (1 << i)) && IsCloseToLine(xymid, float2(xymid.x, xymid.y + 1), xy, mindist))
			OUT = layercols[i];
	}*/



	//if(type_int00)

	//if (type_int == 0) { /*Do Nothing*/ }
	//else if(type_int == 255)
	//	OUT = float4(1, 1, 1, 1);
	//else if ((type_int & (1 << currentlayer)) == (1 << currentlayer))
	//	OUT = layercols[currentlayer];
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
	float edgewidth = 0.035f;
	if (zoom > 1)
	{
		float factor = edgewidth;
		if ((x % 10.0f >= 10 - factor || x % 10.0f <= factor))// || (y % 10.0f >= 10 - factor || y % 10.0f <= factor))
		{
			//if(IsEdgeX(x, y) < 2)
				OUT = float4(0.2f, 0.2f, 0.2f, 0);
		}
		else if ((y % 10.0f >= 10 - factor || y % 10.0f <= factor))
		{
			//if (IsEdgeY(x, y) < 2)
				OUT = float4(0.2f, 0.2f, 0.2f, 0);
		}
		else if (zoom > 4)
		{
			if (x % 1 >= 1 - factor || x % 1 <= factor)
			{
				//if (IsEdgeX(x, y) < 2)
					OUT = float4(0.08f, 0.08f, 0.08f, 0);
			}
			if (y % 1 >= 1 - factor || y % 1 <= factor)
			{
				//if (IsEdgeY(x, y) < 2)
					OUT = float4(0.08f, 0.08f, 0.08f, 0);
			}
		}
	}



	float comptype = comptex[uint2(ux, uy)].a * 255.0f;
	uint comptype_int = (uint)(comptype + 0.5f);
	if (comptype_int != 0)
	{
		uint comptype_p10 = (uint)(comptex[uint2(ux + 1, uy)].a * 255.5f);
		uint comptype_m10 = (uint)(comptex[uint2(ux - 1, uy)].a * 255.5f);
		uint comptype_0p1 = (uint)(comptex[uint2(ux, uy + 1)].a * 255.5f);
		uint comptype_0m1 = (uint)(comptex[uint2(ux, uy - 1)].a * 255.5f);

		uint comptype_m1m1 = (uint)(comptex[uint2(ux - 1, uy - 1)].a * 255.5f);
		uint comptype_p1m1 = (uint)(comptex[uint2(ux + 1, uy - 1)].a * 255.5f);
		uint comptype_p1p1 = (uint)(comptex[uint2(ux + 1, uy + 1)].a * 255.5f);
		uint comptype_m1p1 = (uint)(comptex[uint2(ux - 1, uy + 1)].a * 255.5f);
		


		uint IsValid = 2;

		if (comptype_int > PINOFF)
		{
			edgewidth = 0.3f;
			if ((((comptype_p10 > PINOFF && comptype_int != comptype_p10) || !comptype_p10) || !(((uint)(isedgetex[uint2(ux + 1, uy)].a * 255.5f)) & (1 << 0))) && x % 1 > 1.0f - edgewidth)
			{IsValid = 1;}
			if ((((comptype_m10 > PINOFF && comptype_int != comptype_m10) || !comptype_m10) || !(((uint)(isedgetex[uint2(ux - 1, uy)].a * 255.5f)) & (1 << 2))) && x % 1 < edgewidth)
			{IsValid = 1;}
			if ((((comptype_0p1 > PINOFF && comptype_int != comptype_0p1) || !comptype_0p1) || !(((uint)(isedgetex[uint2(ux, uy + 1)].a * 255.5f)) & (1 << 1))) && y % 1 > 1.0f - edgewidth)
			{IsValid = 1;}
			if ((((comptype_0m1 > PINOFF && comptype_int != comptype_0m1) || !comptype_0m1) || !(((uint)(isedgetex[uint2(ux, uy - 1)].a * 255.5f)) & (1 << 3))) && y % 1 < edgewidth)
			{IsValid = 1;}
		}

		if (!(((uint)(isedgetex[uint2(ux + 1, uy)].a * 255.5f)) & (1 << 0)) && comptype_p10 <= 2 && comptype_p10 && x % 1 > 1.0f - edgewidth)
			IsValid = 0;
		if (!(((uint)(isedgetex[uint2(ux - 1, uy)].a * 255.5f)) & (1 << 2)) && comptype_m10 <= 2 && comptype_m10 && x % 1 < edgewidth)
			IsValid = 0;
		if (!(((uint)(isedgetex[uint2(ux, uy + 1)].a * 255.5f)) & (1 << 1)) && comptype_0p1 <= 2 && comptype_0p1 && y % 1 > 1.0f - edgewidth)
			IsValid = 0;
		if (!(((uint)(isedgetex[uint2(ux, uy - 1)].a * 255.5f)) & (1 << 3)) && comptype_0m1 <= 2 && comptype_0m1  && y % 1 < edgewidth)
			IsValid = 0;

		//if (!(((uint)(isedgetex[uint2(ux - 1, uy - 1)].a * 255.5f)) & (12)) && comptype_m1m1 <= 2 && comptype_m1m1  && x % 1 < edgewidth && y % 1 < edgewidth && !(comptype_int > 2 && comptype_m1m1 > 2))
		//	IsValid = 0;
		//if (!(((uint)(isedgetex[uint2(ux + 1, uy - 1)].a * 255.5f)) & (9)) && comptype_p1m1 <= 2 && comptype_p1m1 && x % 1 > 1.0f - edgewidth && y % 1 < edgewidth && !(comptype_int > 2 && comptype_p1m1 > 2))
		//	IsValid = 0;
		//if (!(((uint)(isedgetex[uint2(ux + 1, uy + 1)].a * 255.5f)) & (3)) && comptype_p1p1 <= 2 && comptype_p1p1 && x % 1 > 1.0f - edgewidth && 1 && y % 1 > 1.0f - edgewidth && !(comptype_int > 2 && comptype_p1p1 > 2))
		//	IsValid = 0;
		//if (!(((uint)(isedgetex[uint2(ux - 1, uy + 1)].a * 255.5f)) & (6)) && comptype_m1p1 <= 2 && comptype_m1p1 && x % 1 < edgewidth && y % 1 > 1.0f - edgewidth && !(comptype_int > 2 && comptype_m1p1 > 2))
		//	IsValid = 0;

		if (IsValid == 2)
		{
			if (comptype_int <= PINOFF)
				OUT = compcols[comptype_int - 1];
			else
				OUT = compcols[4];
		}
		
	}
	else
		OUT.a = 0.2f;




	
	uint type_int = getWiresAtPos(uint2(ux, uy));
	if (type_int >= 128 && OUT.a < 0.5f && OUT.a > 0.15f)
	{
		OUT = float4(1, 1, 1, 1);
	}



	uint NeedsWireCalc_int = (uint)(wirecalctex[uint2(ux, uy)].a * 255.5f);
	float NeedsWireCalc_m1 = wirecalctex[uint2(ux - 1, uy)].a;
	float NeedsWireCalc_p1 = wirecalctex[uint2(ux + 1, uy)].a;
	//uint NeedsWireCalc_int = (uint)(NeedsWireCalc + 0.5f);
	bool IsWireCalc = false;
	[branch]if ((NeedsWireCalc_int || NeedsWireCalc_m1 > 0.75f || NeedsWireCalc_p1 > 0.75f) && OUT.a < 0.5f && zoom > 1)
	{
		IsWireCalc = true;
		uint type_intm1m1 = getWiresAtPos(uint2(ux - 1, uy - 1));
		uint type_int0m1 = getWiresAtPos(uint2(ux, uy - 1));
		uint type_intp1m1 = getWiresAtPos(uint2(ux + 1, uy - 1));

		uint type_intm10 = getWiresAtPos(uint2(ux - 1, uy));
		uint type_intp10 = getWiresAtPos(uint2(ux + 1, uy));

		uint type_intm1p1 = getWiresAtPos(uint2(ux - 1, uy + 1));
		uint type_int0p1 = getWiresAtPos(uint2(ux, uy + 1));
		uint type_intp1p1 = getWiresAtPos(uint2(ux + 1, uy + 1));
		int i;
		[unroll]
		for (i = 6; i >= 0; --i)
		{

			//int i = j;
			//if (j == -1)
			//	i = currentlayer;

			uint WireType = type_int & (1 << i);
			uint state_m1m1 = (type_intm1m1 & (1 << i));
			uint state_0m1 = (type_int0m1 & (1 << i));
			uint state_p1m1 = (type_intp1m1 & (1 << i));
			uint state_p10 = (type_intp10 & (1 << i));
			uint state_p1p1 = (type_intp1p1 & (1 << i));
			uint state_0p1 = (type_int0p1 & (1 << i));
			uint state_m1p1 = (type_intm1p1 & (1 << i));
			uint state_m10 = (type_intm10 & (1 << i));

			[branch]if (WireType)
			{
				IsWire = 1;
				if (WireType == state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_0m1 && IsCloseToLine(xymid, float2(xymid.x, xymid.y - 1), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_0p1 && IsCloseToLine(xymid, float2(xymid.x, xymid.y + 1), xy, mindist))
					OUT = layercols[i];

				if (WireType == state_m1m1 && WireType != state_0m1 && WireType != state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y - 1), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_p1m1 && WireType != state_0m1 && WireType != state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y - 1), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_m1p1 && WireType != state_0p1 && WireType != state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y + 1), xy, mindist))
					OUT = layercols[i];
				if (WireType == state_p1p1 && WireType != state_0p1 && WireType != state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y + 1), xy, mindist))
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
				{
					OUT = layercols[i];
				}
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

		[branch]if (WireType)
		{

			if (WireType == state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_0m1 && IsCloseToLine(xymid, float2(xymid.x, xymid.y - 1), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_0p1 && IsCloseToLine(xymid, float2(xymid.x, xymid.y + 1), xy, mindist))
				OUT = layercols[i];

			if (WireType == state_m1m1 && WireType != state_0m1 && WireType != state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y - 1), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_p1m1 && WireType != state_0m1 && WireType != state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y - 1), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_m1p1 && WireType != state_0p1 && WireType != state_m10 && IsCloseToLine(xymid, float2(xymid.x - 1, xymid.y + 1), xy, mindist))
				OUT = layercols[i];
			if (WireType == state_p1p1 && WireType != state_0p1 && WireType != state_p10 && IsCloseToLine(xymid, float2(xymid.x + 1, xymid.y + 1), xy, mindist))
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
			{
				OUT = layercols[i];
			}

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


	if (zoom <= 1)
	{
		//if(type_int00)
		if(type_int > 0)
			OUT = ColorInPixel(type_int);
		if (comptype_int > 0)
			OUT = ColorInPixel_Comp(comptype_int);

		//if (type_int == 0) { /*Do Nothing*/ }
		//else if(type_int == 255)
		//	OUT = float4(1, 1, 1, 1);
		//else if ((type_int & (1 << currentlayer)) == (1 << currentlayer))
		//	OUT = layercols[currentlayer];
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
	}

	uint type2 = 0;
	if (currenttype == 1)
	{
		uint posx = ux - mousepos_X + 40;
		uint posy = uy - mousepos_Y + 40;
		if (posx >= 0 && posx < 82 && posy >= 0 && posy < 82)
		{
			type2 = (uint)(placementtex[uint2(posx, posy)].a * 255.0f + 0.5f);
			if (type2 != 0)
			{
				if ((OUT.a > 0.5f && !IsWire) || (OUT.a > 0.5f && type2 <= 4))
					OUT = float4(1, 0, 0, 1);
				else
				{
					if (type2 <= 4)
						OUT = compcols[type2 - 1];
					else
						OUT = compcols[4];
				}
			}
		}
	}

	//if (IsWireCalc && OUT.a > 0.5f && comptype_int > 2)
	//{
	//	uint comptype_p10 = (uint)(comptex[uint2(ux + 1, uy)].a * 255.5f);
	//	uint comptype_m10 = (uint)(comptex[uint2(ux - 1, uy)].a * 255.5f);
	//	uint comptype_0p1 = (uint)(comptex[uint2(ux, uy + 1)].a * 255.5f);
	//	uint comptype_0m1 = (uint)(comptex[uint2(ux, uy - 1)].a * 255.5f);
	//	if (comptype_p10 > 2)
	//		OUT = float4(1, 1, 1, 1);
	//	if (comptype_m10 > 2)
	//		OUT = float4(1, 1, 1, 1);
	//	if (comptype_0p1 > 2)
	//		OUT = float4(1, 1, 1, 1);
	//	if (comptype_0m1 > 2)
	//		OUT = float4(1, 1, 1, 1);
	//}

	/*if ((x >= mousepos_X && x <= mousepos_X + 1) || (y >= mousepos_Y && y <= mousepos_Y + 1))
	{
		OUT = OUT * 0.85f + float4(1, 1, 1, 1) * 0.15f;
	}*/
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
		uint highlight_state = (uint)(highlighttex[uint2(abscoo.x, abscoo.y)].a * 255.0f + 0.5f);
		if (highlight_state > 0)
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
				uint wire_type_int2 = (uint)(copywiretex[uint2(abscoo.x - copyposX, abscoo.y - copyposY)].a * 255.0f + 0.5f);
				uint comp_type_int2 = (uint)(copycomptex[uint2(abscoo.x - copyposX, abscoo.y - copyposY)].a * 255.0f + 0.5f);
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