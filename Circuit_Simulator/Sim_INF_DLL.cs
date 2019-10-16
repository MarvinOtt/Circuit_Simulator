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

        public static byte[] WireStates;
        public static int[] CompInfos;
        public static int[] CompID;
        public static int comp_num;

        public Sim_INF_DLL()
        {
            WireStates = new byte[10000000];
            CompInfos = new int[25000000];
            CompID = new int[5000000];
        }

        public void GenerateSimulationData()
        {
            for(int i = 0; i <= Simulator.highestNetworkID; ++i)
            {
                if(Simulator.networks[i] != null)
                    WireStates[i] = Simulator.networks[i].state;
            }

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
                        CompInfos[infocount++] = curcomp.pinNetworkIDs[j];
                    }

                }
            }
            comp_num = compcount;


        }

        [DllImport(DLL_Path + "Sim_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DLL_SimOneStep(byte[] WireStates, int[] CompInfos, int[] CompID, int comp_num);

        public void SimulateOneStep()
        {
            DLL_SimOneStep(WireStates, CompInfos, CompID, comp_num);
        }
    }
}
