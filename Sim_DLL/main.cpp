#include "main.h";





int DLL_EXPORT Test(int a, int b)
{
	return a + b;
}

void DLL_EXPORT DLL_SimOneStep(unsigned char* WireStates, int* CompInfos, int* CompID, int comp_num)
{
	// Simulating all components
	for (int i = 0; i < comp_num; ++i)
	{
		// Test (AND Gate)
		int* curcompinfo = CompInfos + CompID[i];
		if(curcompinfo[0] == 0)
			WireStates[curcompinfo[3]] = WireStates[curcompinfo[1]] & WireStates[curcompinfo[2]];


	}
}