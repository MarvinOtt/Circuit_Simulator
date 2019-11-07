﻿using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
        private const string DLL_Path = "..\\..\\..\\..\\..\\x64\\Debug\\";
        public static int[] WireMap;
        private static byte[] WireStates, WireStates2;
        public static int WireStates_count;
        public static int[] CompInfos;
        public static int[] CompID;
        public static int[] IntStatesMap;
        public static int comp_num;
        public static int[] Comp2UpdateAfterSim;
        public static int Comp2UpdateAfterSim_count;

        public static IntPtr SimDLL_Handle = IntPtr.Zero;

        //[DllImport(DLL_Path + "Sim_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public delegate void DLL_SimOneStep_prototype(byte[] WireStatesIN, byte[] WireStatesOUT, int[] CompInfos, int[] CompID, int comp_num, int net_num);
        //[DllImport(DLL_Path + "Sim_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public delegate void InitSimulation_prototype(int comp_num);

        public static DLL_SimOneStep_prototype DLL_SimOneStep;
        public static InitSimulation_prototype InitSimulation;

        public static void SetIntState(int compID, int stateID)
        {
            CompInfos[IntStatesMap[compID]] = Sim_Component.components[compID].internalstates[stateID];
        }
        public static byte GetWireState(int index)
        {
            return WireStates[WireMap[index]];
        }

        public Sim_INF_DLL()
        {
            WireMap = new int[10000000];
            WireStates = new byte[10000000];
            WireStates2 = new byte[10000000];
            CompInfos = new int[25000000];
            CompID = new int[5000000];
            IntStatesMap = new int[10000000];
            Comp2UpdateAfterSim = new int[1000000];

            InitSimulation(8);

        }

        public static void LoadLibrarys(params string[] paths)
        {
            Sim_Component.Components_Data.Clear();
            CompLibrary.AllLibrarys.Clear();

            for(int i = 0; i < paths.Length; ++i)
            {
                if(File.Exists(paths[i]))
                {
                    CompLibrary newlibrary = new CompLibrary(null, paths[i]);
                    newlibrary.Load();
                }
            }
            UI_Handler.InitComponents4CompBox();
            GenerateDllCodeAndCompile();
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
                code_withinitfuncs = code_withinitfuncs.Insert(initfuncpos, "compfuncs[index++] = " + curdata.Code_Sim_FuncName + ";");
            }

            //int clickfuncpos = code_withinitfuncs.IndexOf("#define _CLICKFUNCS_");
            //string code_withclickfuncs = code_withinitfuncs.Remove(clickfuncpos, 20);
            //for (int i = Sim_Component.Components_Data.Count - 1; i >= 0; --i)
            //{
            //    CompData curdata = Sim_Component.Components_Data[i];
            //    if(curdata.IsClickable)
            //    {
            //        code_withclickfuncs.Insert(clickfuncpos, curdata.Code_ClickAction);
            //    }
            //}

            int afterupdatefuncpos = code_withinitfuncs.IndexOf("#define _AFTERUPDATEFUNCS_");
            string code_withafterupdatefuncs = code_withinitfuncs.Remove(afterupdatefuncpos, 26);
            for (int i = Sim_Component.Components_Data.Count - 1; i >= 0; --i)
            {
                CompData curdata = Sim_Component.Components_Data[i];
                if (curdata.IsUpdateAfterSim)
                {
                    code_withafterupdatefuncs = code_withafterupdatefuncs.Insert(afterupdatefuncpos, curdata.Code_AfterSimAction);
                }
            }
            string pathtoexe = Directory.GetCurrentDirectory();
            File.WriteAllText(pathtoexe + @"\SIM_CODE\maincode.c", code_withafterupdatefuncs);
            //System.Diagnostics.Process.Start("cmd", "/c" + "C:\\MinGW\\GCC\\gcc -c -DBUILDING_EXAMPLE_DLL C:\\Users\\marvi\\code.c -o C:\\Users\\marvi\\code.o");
            //System.Diagnostics.Process.Start("cmd", "/k" + "C:\\MinGW\\GCC\\gcc -shared -o C:\\Users\\marvi\\code.dll C:\\Users\\marvi\\code.o -Wl,--out-implib,libexample_dll.a");
            //Extensions.CMD_Execute("cmd", "/k " + "cmd " + "\"" + @"C:\Users\Marvin\ Ott" + "\"");
            //string args = "/k " + "\"" + pathtoexe + @"\GCC\gcc" + "\"" + " - c -DBUILDING_EXAMPLE_DLL " + "\"" + pathtoexe + @"\SIM_CODE\maincode.c" + "\"" + " - o " + "\"" + pathtoexe + @"\SIM_CODE\maincode.o" + "\"";
            Extensions.CMD_Execute("cmd", "/c " + @"C:\GCC\mingw64\bin\g++" + " -c -m64 -DBUILDING_EXAMPLE_DLL " + "\"" + pathtoexe + @"\SIM_CODE\maincode.c" + "\"" + " -o " + "\"" + pathtoexe + @"\SIM_CODE\maincode.o" + "\"");
            Extensions.CMD_Execute("cmd", "/c" + @"C:\GCC\mingw64\bin\g++" + @" -shared -o " + "\"" + pathtoexe + @"\SIM_CODE\maincode.dll" + "\" " + "\"" + pathtoexe + @"\SIM_CODE\maincode.o" + "\"");

            //Extensions.CMD_Execute("cmd", "/c " + @"C:\GCC\mingw64\bin\g++" + " -c -m64 -DBUILDING_EXAMPLE_DLL " + "\"" + pathtoexe + @"\SIM_CODE\code.c" + "\"" + " -o " + "\"" + pathtoexe + @"\SIM_CODE\code.o" + "\"");
            //Extensions.CMD_Execute("cmd", "/k" + @"C:\GCC\mingw64\bin\g++" + @" -shared -o " + "\"" + pathtoexe + @"\SIM_CODE\code.dll" + "\" " + "\"" + pathtoexe + @"\SIM_CODE\code.o" + "\"");
            LoadSimDLL();
        }

        public static void LoadSimDLL()
        {
            if (SimDLL_Handle != IntPtr.Zero)
                DLL_Methods.FreeLibrary(SimDLL_Handle);

            string pathtoexe = Directory.GetCurrentDirectory();
            SimDLL_Handle = DLL_Methods.LoadLibrary(pathtoexe + @"\SIM_CODE\maincode.dll");
            for(int i = 0; i < Sim_Component.Components_Data.Count; ++i)
            {
                CompData curdata = Sim_Component.Components_Data[i];
                if (curdata.IsUpdateAfterSim)
                {
                    IntPtr AddressOfFunc_AfterSimUpdate = DLL_Methods.GetProcAddress(SimDLL_Handle, curdata.Code_AfterSimAction_FuncName);

                    curdata.AfterSimAction = (CompData.AfterSimAction_Prototype)Marshal.GetDelegateForFunctionPointer(AddressOfFunc_AfterSimUpdate, typeof(CompData.AfterSimAction_Prototype));
                }
            }

            IntPtr AddressOfFunc_InitSimulation = DLL_Methods.GetProcAddress(SimDLL_Handle, "InitSimulation");
            InitSimulation = (InitSimulation_prototype)Marshal.GetDelegateForFunctionPointer(AddressOfFunc_InitSimulation, typeof(InitSimulation_prototype));

            IntPtr AddressOfFunc_DLL_SimOneStep = DLL_Methods.GetProcAddress(SimDLL_Handle, "DLL_SimOneStep");
            DLL_SimOneStep = (DLL_SimOneStep_prototype)Marshal.GetDelegateForFunctionPointer(AddressOfFunc_DLL_SimOneStep, typeof(DLL_SimOneStep_prototype));

        }

        public void GenerateSimulationData()
        {
            Comp2UpdateAfterSim_count = 0;
            int count = 0;
            for(int i = 0; i <= Simulator.highestNetworkID; ++i)
            {
                if (Simulator.networks[i] != null)
                {
                    WireMap[i] = count;
                    WireStates[count] = WireStates2[count] = Simulator.networks[i].state;
                    count++;
                }
            }
            WireStates_count = count;

            int compcount = 0;
            int infocount = 0;
            for (int i = 0; i < Sim_Component.nextComponentID; ++i)
            {
                if (Sim_Component.components[i] != null)
                {
                    Component curcomp = Sim_Component.components[i];
                    CompID[compcount++] = infocount;
                    CompInfos[infocount++] = curcomp.dataID;
                    CompData compdata = Sim_Component.Components_Data[curcomp.dataID];
                    if (compdata.IsUpdateAfterSim)
                        Comp2UpdateAfterSim[Comp2UpdateAfterSim_count++] = i;
                    for (int j = 0; j < compdata.pin_num; ++j)
                    {
                        CompInfos[infocount++] = WireMap[curcomp.pinNetworkIDs[j]];
                    }
                    if(compdata.internalstate_length > 0)
                    {
                        IntStatesMap[i] = infocount;
                        for(int j = 0; j < compdata.internalstate_length; ++j)
                        {
                            CompInfos[infocount++] = curcomp.internalstates[j];
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
                    DLL_SimOneStep(WireStates, WireStates2, CompInfos, CompID, comp_num, WireStates_count);
                }
            }
            else
            {
                Simulator.simspeed_count++;
                if(Simulator.simspeed_count >= (int)Math.Pow(2, -Simulator.simspeed))
                {
                    DLL_SimOneStep(WireStates, WireStates2, CompInfos, CompID, comp_num, WireStates_count);
                    Simulator.simspeed_count = 0;
                }
            }
            for(int i = 0; i < Comp2UpdateAfterSim_count; ++i)
            {
                Component comp = Sim_Component.components[Comp2UpdateAfterSim[i]];



                Sim_Component.Components_Data[comp.dataID].AfterSimAction(comp.internalstates, CompInfos, IntStatesMap[comp.ID]);
            }
        }
    }
}
