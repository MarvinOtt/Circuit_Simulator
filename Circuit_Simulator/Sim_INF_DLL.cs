using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Circuit_Simulator
{
    static class DLL_Methods
    {
        [DllImport("kernel32.dll")]
        public static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        public static extern bool FreeLibrary(IntPtr hModule);
    }

    public unsafe class Sim_INF_DLL
    {
        public static int[] WireMap, WireMapInv;
        public static byte[] WireStates_IN, WireStates_OUT;
        public static int WireStates_count;
        public static int[] CompInfos;	
        public static int[] CompID;
        public static int[] IntStatesMap;
        public static int comp_num, line_num;

        public static IntPtr SimDLL_Handle = IntPtr.Zero;

        public delegate void DLL_SimOneStep_prototype(byte[] WireStatesIN, byte[] WireStatesOUT, int[] CompInfos, int[] CompID, int comp_num, int net_num);
        public delegate void InitSimulation_prototype(int comp_num);

        public delegate void Sim_Steped_Handler(object sender);
        public static event Sim_Steped_Handler SimFrameStep = delegate { };
		public static event Sim_Steped_Handler SimFrameStep_maxperframe = delegate { };

		public static DLL_SimOneStep_prototype DLL_SimOneStep;
        public static InitSimulation_prototype InitSimulation;

        public static void SetIntState(int compID, int stateID)
        {
            CompInfos[IntStatesMap[compID] + stateID] = Sim_Component.components[compID].totalstates[stateID];
        }
        public static byte GetWireState(int index)
        {
            return WireStates_IN[WireMap[index]];
        }

        public Sim_INF_DLL()
        {
            WireMap = new int[10000000];
			WireMapInv = new int[10000000];
            WireStates_IN = new byte[10000000];
            WireStates_OUT = new byte[10000000];
            CompInfos = new int[25000000];
            CompID = new int[5000000];
            IntStatesMap = new int[10000000];

            //InitSimulation(Sim_Component.Components_Data.Count);

        }

        public static bool LoadLibrarys(params string[] paths)
        {
			List<string> missinglibraries = new List<string>();
			for (int i = 0; i < paths.Length; ++i)
			{
				if (!File.Exists(paths[i]))
					missinglibraries.Add(paths[i]);
			}
			if(missinglibraries.Count > 0)
			{
				string message = "Following Libraries not found:\n";
				for(int i = 0; i < missinglibraries.Count; ++i)
				{
					message += missinglibraries[i] + "\n";
				}
				System.Windows.Forms.MessageBox.Show(message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			Sim_Component.Components_Data.Clear();
            CompLibrary.AllUsedLibraries.Clear();

            for(int i = 0; i < paths.Length; ++i)
            {
                //if(File.Exists(paths[i]))
                //{
                CompLibrary newlibrary = new CompLibrary(null, paths[i]);
                newlibrary.LoadFromPath();
                //}
                //else
                //{
                //    System.Windows.Forms.MessageBox.Show("Library not found: \n" + paths[i], null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    Sim_Component.Components_Data.Clear();
                //    CompLibrary.AllUsedLibraries.Clear();
                //    UI_Handler.InitComponents();
                //    return -2;
                //}
            }
            UI_Handler.InitComponents();
            GenerateDllCodeAndCompile();
            return true;
        }

        public static void GenerateDllCodeAndCompile()
        {
            // Generating DLL Code and DLLs
            string code_original = File.ReadAllText(@"SIM_CODE\original.c");
            int compfuncspos = code_original.IndexOf("#define _COMPFUNCS_");
            string code_withcompfuncs = code_original.Remove(compfuncspos, 19);
            for (int i = 0; i < Sim_Component.Components_Data.Count; ++i)
            {
                CompData curdata = Sim_Component.Components_Data[i];
                code_withcompfuncs = code_withcompfuncs.Insert(compfuncspos, curdata.Code_Sim);
            }

            int initfuncpos = code_withcompfuncs.IndexOf("int _INITFUNCS_;");
            string code_withinitfuncs = code_withcompfuncs.Remove(initfuncpos, 16);
            for (int i = Sim_Component.Components_Data.Count - 1; i >= 0; --i)
            {
                CompData curdata = Sim_Component.Components_Data[i];
                code_withinitfuncs = code_withinitfuncs.Insert(initfuncpos, "compfuncs[index++] = " + curdata.Code_Sim_FuncName + ";\n");
            }

            //int afterupdatefuncpos = code_withinitfuncs.IndexOf("#define _AFTERUPDATEFUNCS_");
            //string code_withafterupdatefuncs = code_withinitfuncs.Remove(afterupdatefuncpos, 26);
            //for (int i = Sim_Component.Components_Data.Count - 1; i >= 0; --i)
            //{
            //    CompData curdata = Sim_Component.Components_Data[i];
            //    if (curdata.IsUpdateAfterSim)
            //    {
            //        code_withafterupdatefuncs = code_withafterupdatefuncs.Insert(afterupdatefuncpos, curdata.Code_AfterSim);
            //    }
            //}
            if (SimDLL_Handle != IntPtr.Zero)
            {
                DLL_Methods.FreeLibrary(SimDLL_Handle);
                SimDLL_Handle = (IntPtr)0;
            }

            string pathtoexe = Directory.GetCurrentDirectory();
            File.WriteAllText(pathtoexe + @"\SIM_CODE\maincode.c", code_withinitfuncs);

            string driveletter = Path.GetPathRoot(Environment.SystemDirectory);
			//string command = "/c " + driveletter + @"GCC\mingw64\bin\g++";
			//command += " -c -m64 -DBUILDING_EXAMPLE_DLL ";
			//command += "\"" + pathtoexe + @"\SIM_CODE\maincode.c" + "\" -o \"";
			//command += pathtoexe + @"\SIM_CODE\maincode.o" + "\"";
			//Extensions.ExecuteProgram("cmd", command);
			string GCC_Path = Path.GetFullPath(Config.GCC_Compiler_PATH);
			if (GCC_Path.Last() == '\\')
				GCC_Path = GCC_Path.Remove(GCC_Path.Length - 1, 1);
			string command = "/c " + "\"" + "\"" + GCC_Path + @"\g++" + "\"" + " -m64 -shared \"";
			command += pathtoexe + @"\SIM_CODE\maincode.dll" + "\" \"";
			command += pathtoexe + @"\SIM_CODE\maincode.c" + "\"" + "\"";
			Extensions.ExecuteProgram("cmd", command);

            LoadSimDLL();
        }

        public static void LoadSimDLL()
        {
            if (SimDLL_Handle != IntPtr.Zero)
            {
                DLL_Methods.FreeLibrary(SimDLL_Handle);
                SimDLL_Handle = (IntPtr)0;
            }

            string pathtoexe = Directory.GetCurrentDirectory();
            SimDLL_Handle = DLL_Methods.LoadLibrary(pathtoexe + @"\SIM_CODE\maincode.dll");

            IntPtr AddressOfFunc_InitSimulation = DLL_Methods.GetProcAddress(SimDLL_Handle, "InitSimulation");
            InitSimulation = (InitSimulation_prototype)Marshal.GetDelegateForFunctionPointer(
							 AddressOfFunc_InitSimulation, typeof(InitSimulation_prototype));

            IntPtr AddressOfFunc_DLL_SimOneStep = DLL_Methods.GetProcAddress(SimDLL_Handle, "DLL_SimOneStep");
            DLL_SimOneStep = (DLL_SimOneStep_prototype)Marshal.GetDelegateForFunctionPointer(
							 AddressOfFunc_DLL_SimOneStep, typeof(DLL_SimOneStep_prototype));

        }

        public static void GenerateSimulationData()
        {
            InitSimulation(Sim_Component.Components_Data.Count);
            int count = 1;
			line_num = 0;
            for(int i = 0; i <= Simulator.highestNetworkID; ++i)
            {
                if (Simulator.networks[i] != null)
                {
                    WireMap[i] = count;
					WireMapInv[count] = i;
                    WireStates_IN[count] = WireStates_OUT[count] = Simulator.networks[i].state;
                    count++;
					line_num += Simulator.networks[i].lines.Count;
				}
            }
            WireStates_count = count;

            int compcount = 0, infocount = 0;
            for (int i = 0; i < Sim_Component.nextComponentID; ++i)
            {
                if (Sim_Component.components[i] != null)
                {
                    Component curcomp = Sim_Component.components[i];
                    CompID[compcount++] = infocount;
                    CompInfos[infocount++] = curcomp.dataID;
                    CompData compdata = Sim_Component.Components_Data[curcomp.dataID];

                    for (int j = 0; j < compdata.pin_num; ++j)
                    {
                        CompInfos[infocount++] = WireMap[curcomp.pinNetworkIDs[j]] - 1;
                    }
                    if(compdata.totalstate_length > 0)
                    {
                        IntStatesMap[i] = infocount;
                        for(int j = 0; j < compdata.totalstate_length; ++j)
                        {
                            CompInfos[infocount++] = curcomp.totalstates[j];
                        }
                    }
                }
            }
            comp_num = compcount;
        }

        public void SimulateOneStep()
        {
            if (Simulator.simspeed >= 0)
            {
                for (int i = 0; i < (int)Math.Pow(2, Simulator.simspeed); ++i)
                {
                    Simulator.cursimframe++;
                    DLL_SimOneStep(WireStates_IN, WireStates_OUT, CompInfos, CompID, comp_num, WireStates_count);
                    SimFrameStep(this);
                }
				SimFrameStep_maxperframe(this);

			}
            else
            {
                Simulator.simspeed_count++;
                if(Simulator.simspeed_count >= (int)Math.Pow(2, -Simulator.simspeed))
                {
                    Simulator.cursimframe++;
                    DLL_SimOneStep(WireStates_IN, WireStates_OUT, CompInfos, CompID, comp_num, WireStates_count);
                    Simulator.simspeed_count = 0;
                    SimFrameStep(this);
					SimFrameStep_maxperframe(this);

				}
            }
        }
    }
}
