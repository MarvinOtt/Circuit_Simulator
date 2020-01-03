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
        public UI_ValueInput Box_Name;
        public UI_Scrollable<UI_Element> Features;
        UI_StringButton Code_Sim_Button, Code_AfterSim_Button;
        UI_GridPaint gridpaint;

        //Code Boxes
        public static UI_TextBox CodeBox_Sim, CodeBox_AfterSim;

        public UI_EditComp_Window(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable )
        {

            UI_String spooky = new UI_String(new Pos(0), Point.Zero, conf, "");
            UI_String Box_Name_Label = new UI_String(new Pos(bezelsize, bezelsize + headheight), Point.Zero, UI_Handler.genbutconf, "Name: ");
            Box_Name = new UI_ValueInput(new Pos(0, ORIGIN.TR, ORIGIN.DEFAULT, Box_Name_Label), new Point(size.X / 4, (int)(UI_Handler.genbutconf.font.MeasureString("Test").Y)), UI_Handler.genbutconf, 3);
            Box_Name.ValueChanged += BoxName_ValueChange;
            Features = new UI_Scrollable<UI_Element>(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Box_Name_Label), Point.Zero);
            Code_Sim_Button = new UI_StringButton(new Pos(5, 5), new Point((int)(UI_Handler.buttonwidth * 1.8), UI_Handler.buttonheight), "Edit Sim Code", true, UI_Handler.genbutconf);
            Code_AfterSim_Button = new UI_StringButton(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Code_Sim_Button), new Point((int)(UI_Handler.buttonwidth * 2.4), UI_Handler.buttonheight), "Edit After-Sim Code", true, UI_Handler.genbutconf);
            gridpaint = new UI_GridPaint(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Code_AfterSim_Button), new Point(400), 300, new Point(150), new Point(-2, 6));
            gridpaint.UpdateFunctions.Add(delegate ()
            {
                if (new Rectangle(gridpaint.absolutpos, gridpaint.size).Contains(Game1.mo_states.New.Position))
                    Features.DenyScroll = true;
            });
            Features.Add_UI_Elements(spooky, Code_Sim_Button, Code_AfterSim_Button, gridpaint);
            Add_UI_Elements(Box_Name_Label, Box_Name, Features);
            
            Code_Sim_Button.GotActivatedLeft += Code_Sim_Button_Pressed;
            Code_AfterSim_Button.GotActivatedLeft += Code_AfterSim_Button_Pressed;

            // Code Boxes
            CodeBox_Sim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            CodeBox_AfterSim = new UI_TextBox(new Pos(0), new Point(250, 400), UI_Handler.gen_conf);
            Resize();
            GetsUpdated = GetsDrawn = false;
        }
        public void SetRootComp(CompData comp)
        {
            rootcomp = comp;
            Box_Name.value = comp.name;
            CodeBox_Sim.t.Text = comp.Code_Sim;
            CodeBox_AfterSim.t.Text = comp.Code_AfterSim;
            gridpaint.pixel = rootcomp.data[0];
            gridpaint.ApplyPixel();
        }

        public override void ChangedUpdate2True()
        {
            base.ChangedUpdate2True();
        }

        public void BoxName_ValueChange(object sender)
        {
            rootcomp.name = Box_Name.value;
            UI_Handler.LibaryWindow.Reload_UI();
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

