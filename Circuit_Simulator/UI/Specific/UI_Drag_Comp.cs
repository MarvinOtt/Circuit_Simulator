using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Drag_Comp : UI_Element
    {
        public UI_Component comp;

        public UI_Drag_Comp() : base(Pos.Zero, Point.Zero)
        {
            GetsDrawn = false;
            GetsUpdated = false;
        }

        public override void UpdateSpecific()
        {
            pos.pos = App.mo_states.New.Position;
            UI_Handler.UI_Active_State = 2;
            if (App.mo_states.New.LeftButton == ButtonState.Pressed)
            {
                Sim_Component.DropComponent = true;
            }
          
        }
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
     
            spritebatch.DrawString(comp.conf.font, comp.text, App.mo_states.New.Position.ToVector2(), comp.conf.font_color);

        }
    }
}
