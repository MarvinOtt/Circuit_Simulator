
#define DLL_EXPORT __declspec(dllexport)


extern "C"
{
	int DLL_EXPORT TestFunction1113(int a);

	int DLL_EXPORT TestFunction1113(int a)
	{
		/*int c = 0, i;
		for(i = 0; i < 200; ++i)
		{
			c = ((a*b -b/a + b%a) / 2) + (b*a) + c / 100;
		}*/
		return a;
	}
}
/*int main() 
{
	printf("Hello World\n");
	sdef
	getchar();
	return 0;
}*/

/*extern "C" DLL_EXPORT BOOL APIENTRY DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	switch (fdwReason)
	{
	case DLL_PROCESS_ATTACH:
		// attach to process
		// return FALSE to fail DLL load
		break;

	case DLL_PROCESS_DETACH:
		// detach from process
		break;

	case DLL_THREAD_ATTACH:
		// attach to thread
		break;

	case DLL_THREAD_DETACH:
		// detach from thread
		break;
	}
	return TRUE; // succesful
}*/