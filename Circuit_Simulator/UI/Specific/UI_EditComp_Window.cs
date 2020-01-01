using Circuit_Simulator.COMP;
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
    public class UI_EditComp_Window : UI_Window
    {
        public CompData rootcomp;
        public UI_ValueInput Name;
        public UI_Scrollable<UI_Element> Features;
        UI_StringButton Code_Sim_Button, Code_AfterSim_Button;
        //Code Boxes
        public static UI_TextBox CodeBox_Sim, CodeBox_AfterSim;

        public UI_EditComp_Window(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable )
        {
            Name = new UI_ValueInput(new Pos(bezelsize, headheight + (int)(conf.font.MeasureString("Test").Y)), new Point(size.X / 4, (int)(conf.font.MeasureString("Test").Y)), conf, 3);
            Features = new UI_Scrollable<UI_Element>(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Name), Point.Zero);
            Code_Sim_Button = new UI_StringButton(new Pos(5, 0), new Point((int)(UI_Handler.buttonwidth * 1.8), UI_Handler.buttonheight), "Edit Sim Code", true, UI_Handler.genbutconf);
            Code_AfterSim_Button = new UI_StringButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Code_Sim_Button), new Point((int)(UI_Handler.buttonwidth * 2.4), UI_Handler.buttonheight), "Edit After-Sim Code", true, UI_Handler.genbutconf);
            Features.Add_UI_Elements(Code_Sim_Button, Code_AfterSim_Button);
            GetsUpdated = GetsDrawn = false;
            Add_UI_Elements(Name, Features);
            Code_Sim_Button.GotActivatedLeft += Code_Sim_Button_Pressed;
            Code_AfterSim_Button.GotActivatedLeft += Code_AfterSim_Button_Pressed;

            // Code Boxes
            CodeBox_Sim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            CodeBox_AfterSim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            Resize();
        }
        public void SetRootComp(CompData comp)
        {
            rootcomp = comp;
            Name.value = comp.name;
            CodeBox_Sim.t.Text = comp.Code_Sim;
            CodeBox_AfterSim.t.Text = comp.Code_AfterSim;
        }

        public override void ChangedUpdate2True()
        {

            base.ChangedUpdate2True();
        }

        public void Code_Sim_Button_Pressed(object sender)
        {
            CodeBox_Sim.Show();
        }

        public void Code_AfterSim_Button_Pressed(object sender)
        {
            CodeBox_AfterSim.Show();
        }

        protected override void Resize()
        {
            Features.size = new Point(size.X - bezelsize * 2, size.Y - bezelsize * 2 - (int)(conf.font.MeasureString("Test").Y) - 5 - Features.pos.Y - UI_Handler.buttonheight);
            base.Resize();
        }

        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }
        
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }
    }

}

