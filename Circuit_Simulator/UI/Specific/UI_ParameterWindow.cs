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
    public class UI_ParameterWindow : UI_Window
    {
        public Component rootcomp;

        public UI_ParameterWindow(Pos pos, Point size, Point minsize, Generic_Conf conf) : base(pos, size, "Parameters", minsize, conf, false)
        {
            
        }



        public void SetRootcomp(Component rootcomp)
        {
            this.rootcomp = rootcomp;
            int parmetercount = Sim_Component.Components_Data[rootcomp.dataID].valuebox_length;
            int maxstringlength = (int)Sim_Component.Components_Data[rootcomp.dataID].parameters.Max(x => UI_Handler.genbutconf.font.MeasureString(x + ": ").X);
            this.Title = Sim_Component.Components_Data[rootcomp.dataID].name + " Parameters";
            this.ui_elements.RemoveRange(1, this.ui_elements.Count - 1);
            if (parmetercount > 0)
            {
                this.Add_UI_Elements(new UI_String(new Pos(bezelsize,headheight + bezelsize), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, Sim_Component.Components_Data[rootcomp.dataID].parameters[0] +": "));
                this.Add_UI_Elements(new UI_ValueInput(new Pos(maxstringlength, 0, ORIGIN.DEFAULT, ORIGIN.DEFAULT, this.ui_elements[1]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 1, 10));

                for (int i = 2; i < parmetercount + 1; i++)
                {
                    this.Add_UI_Elements(new UI_String(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, this.ui_elements[i * 2 - 3]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, Sim_Component.Components_Data[rootcomp.dataID].parameters[i - 1] + ": "));
                    this.Add_UI_Elements(new UI_ValueInput(new Pos(maxstringlength, 0, ORIGIN.DEFAULT, ORIGIN.DEFAULT, this.ui_elements[i * 2 - 1]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 1, 10));

                }
                this.GetsUpdated = this.GetsDrawn = true;
            }
            
        }
        public override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }



    }
}

