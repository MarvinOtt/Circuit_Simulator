using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public unsafe class Sim_INF_DLL
    {
        private const string DLL_Path = "..\\..\\..\\..\\..\\x64\\Debug\\";
        public static int[] WireMap;
        private static byte[] WireStates, WireStates2;
        public static int WireStates_count;
        public static int[] CompInfos;
        public static int[] CompID;
        public static int comp_num;

        public static void SetState(int index, byte state)
        {
            WireStates[WireMap[index]] = WireStates2[WireMap[index]] = state;
        }
        public static byte GetState(int index)
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
        }



        public void GenerateSimulationData()
        {
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
                    ComponentData compdata = Sim_Component.Components_Data[curcomp.dataID];
                    for(int j = 0; j < compdata.pin_num; ++j)
                    {
                        CompInfos[infocount++] = WireMap[curcomp.pinNetworkIDs[j]];
                    }

                }
            }
            comp_num = compcount;


        }

        [DllImport(DLL_Path + "Sim_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DLL_SimOneStep(byte[] WireStatesIN, byte[] WireStatesOUT, int[] CompInfos, int[] CompID, int comp_num, int net_num);

        public void SimulateOneStep()
        {
            DLL_SimOneStep(WireStates, WireStates2, CompInfos, CompID, comp_num, WireStates_count);
        }
    }
}
