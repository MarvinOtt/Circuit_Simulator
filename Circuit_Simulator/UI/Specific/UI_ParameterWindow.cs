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
            this.Title = Sim_Component.Components_Data[rootcomp.dataID].name + "Parameters";
            this.ui_elements.Clear();
            if (parmetercount > 0)
            {
                this.Add_UI_Elements(new UI_String(Pos.Zero, new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, Sim_Component.Components_Data[rootcomp.dataID].parameters[0]));
                this.Add_UI_Elements(new UI_ValueInput(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, this.ui_elements[0]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 1, 12));

                for (int i = 1; i < parmetercount; i++)
                {
                    this.Add_UI_Elements(new UI_String(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, this.ui_elements[i - 1]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, Sim_Component.Components_Data[rootcomp.dataID].parameters[0]));
                    this.Add_UI_Elements(new UI_ValueInput(new Pos(5, 0, ORIGIN.TR, ORIGIN.DEFAULT, this.ui_elements[i]), new Point(size.X / 2, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 1, 12));

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

