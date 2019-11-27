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
       public UI_Scrollable<UI_Element> Features;

        public UI_EditComp_Window(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable )
        {
            UI_ValueInput Name = new UI_ValueInput(new Pos(bezelsize, headheight + (int)(conf.font.MeasureString("Test").Y)), new Point(size.X / 4, (int)(conf.font.MeasureString("Test").Y)), conf, 3);
            Features = new UI_Scrollable<UI_Element>(new Pos(0, 5, ORIGIN.BL, ORIGIN.DEFAULT, Name), Point.Zero);
            UI_StringButton Code = new UI_StringButton(new Pos(0, 0), new Point((int)(UI_Handler.buttonwidth * 1.2), UI_Handler.buttonheight), "Edit Code", true, UI_Handler.genbutconf);
            Features.Add_UI_Elements(Code);
            GetsUpdated = GetsDrawn = false;
            Add_UI_Elements(Name, Features);
            Resize();
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

