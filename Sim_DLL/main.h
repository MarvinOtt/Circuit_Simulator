#pragma once

#ifdef BUILD_DLL
#define DLL_EXPORT _declspec(dllexport)
#else
#define DLL_EXPORT _declspec(dllexport)
#endif

#ifdef __cplusplus
extern "C"
{
#endif




	int DLL_EXPORT Test(int a, int b);

	void DLL_EXPORT DLL_SimOneStep(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfos, int* CompID, int comp_num, int net_num);


#ifdef __cplusplus
}
#endif