#include <Windows.h>

#define DLL_EXPORT __declspec(dllexport)

typedef void (*compfunc)(unsigned char*, unsigned char*, int* );

compfunc* compfuncs;

// Main Component Functions


#define _COMPFUNCS_

extern "C"
{

	void DLL_EXPORT InitSimulation(int comp_num);

	void InitSimulation(int comp_num)
	{
		int index = 0;
		compfuncs = (compfunc*)new compfunc[comp_num];

		
		int _INITFUNCS_;


	}

	void DLL_EXPORT DLL_SimOneStep(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfos, int* CompID, int comp_num, int net_num);

	void DLL_SimOneStep(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfos, int* CompID, int comp_num, int net_num)
	{
		// Simulating all components
		int i;
		for (i = 0; i < comp_num; ++i)
		{
			int* curcompinfo = CompInfos + CompID[i];
			compfuncs[curcompinfo[0]](WireStatesIN, WireStatesOUT, curcompinfo);
		}

		// Copying WireStates OUT into IN
		memcpy(WireStatesIN, WireStatesOUT, net_num);
	}


	#define _AFTERUPDATEFUNCS_

}

