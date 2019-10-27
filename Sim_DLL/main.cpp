#include "main.h";
#include <Windows.h>


typedef void (*compfunc)(unsigned char*, unsigned char*, int* );

compfunc* compfuncs;


// Main Component Functions
void CF_AND(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
{
	WireStatesOUT[CompInfo[3]] = WireStatesIN[CompInfo[1]] & WireStatesIN[CompInfo[2]];
}

void CF_SWITCH(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
{
	WireStatesOUT[CompInfo[1]] = CompInfo[5];
	WireStatesOUT[CompInfo[2]] = CompInfo[5];
	WireStatesOUT[CompInfo[3]] = CompInfo[5];
	WireStatesOUT[CompInfo[4]] = CompInfo[5];
}

void DLL_EXPORT InitSimulation(int comp_num)
{
	int index = 0;
	compfuncs = (compfunc*)new compfunc[comp_num];
	compfuncs[index++] = CF_AND;
	compfuncs[index++] = CF_SWITCH;


}


void DLL_EXPORT DLL_SimOneStep(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfos, int* CompID, int comp_num, int net_num)
{
	// Simulating all components
	for (int i = 0; i < comp_num; ++i)
	{
		// Test (AND Gate)
		int* curcompinfo = CompInfos + CompID[i];

		compfuncs[curcompinfo[0]](WireStatesIN, WireStatesOUT, curcompinfo);


		//if(curcompinfo[0] == 0)
		//	WireStatesOUT[curcompinfo[3]] = WireStatesIN[curcompinfo[1]] & WireStatesIN[curcompinfo[2]];


	}

	// Copying WireStates OUT into IN
	memcpy(WireStatesIN, WireStatesOUT, net_num);
}