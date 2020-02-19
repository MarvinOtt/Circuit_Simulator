using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_SignalAnalyze : UI_Element
    {
        public int WireID;
        int scale_log = 2;
        int[] values;
        int memorycounter;


        public UI_SignalAnalyze(Pos pos, Point size) :base(pos, size)
        {
            values = new int[1000];
        }

        protected override void UpdateAlways()
        {
            int state = Sim_INF_DLL.GetWireState(WireID);
            if (Sim_INF_DLL.IsSimStep)
            {
                memorycounter = (memorycounter + 1) % 1000;
                values[memorycounter] = state;
                Sim_INF_DLL.IsSimStep = false;
            }

            base.UpdateAlways();          
        }

        public override void UpdateSpecific()
        {
            if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue)
            {
                scale_log--;
            }
            else if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue)
            {
                scale_log++;
            }
            base.UpdateSpecific();
        }


        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            float scale = (float)Math.Pow(2, scale_log);
            for (int i = 0; i >= -1000; i--)
            {
                int reali = i + memorycounter;
                if (reali < 0)
                    reali += 1000;
                int previndex = reali - 1;
                if (previndex < 0)
                    previndex = 999;
                if(values[reali] != values[previndex])
                {
                    spritebatch.DrawLine(absolutpos + new Point(size.X - 5 + (int)(scale * i), 0), absolutpos + new Point(size.X - 5 + (int)(scale * i), size.Y), Color.White);
                }
                spritebatch.DrawLine(absolutpos + new Point(size.X - 5 + (int)(scale * i), size.Y - values[reali] * size.Y), absolutpos + new Point(size.X - 5 + (int)(scale * (i + 1)), size.Y - values[reali] * size.Y), Color.White);
            }
            base.DrawSpecific(spritebatch);
        }
    }

}
