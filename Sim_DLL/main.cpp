#include "main.h";
#include <Windows.h>




int DLL_EXPORT Test(int a, int b)
{
	return a + b;
}

void DLL_EXPORT DLL_SimOneStep(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfos, int* CompID, int comp_num, int net_num)
{
	// Simulating all components
	for (int i = 0; i < comp_num; ++i)
	{
		// Test (AND Gate)
		int* curcompinfo = CompInfos + CompID[i];
		if(curcompinfo[0] == 0)
			WireStatesOUT[curcompinfo[3]] = WireStatesIN[curcompinfo[1]] & WireStatesIN[curcompinfo[2]];


	}

	// Copying WireStates OUT into IN
	memcpy(WireStatesIN, WireStatesOUT, net_num);
}