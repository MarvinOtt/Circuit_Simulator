using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Drag_Comp : UI_Element
    {
        public UI_Component comp;

        public UI_Drag_Comp() : base(Point.Zero, Point.Zero)
        {
            GetsDrawn = false;
            GetsUpdated = false;
        }

        protected override void UpdateSpecific()
        {
            pos = Game1.mo_states.New.Position;
            UI_Handler.UI_Active_State = 2;
            if (Game1.mo_states.New.LeftButton == ButtonState.Released)
            {
                comp.IsDrag = false;
                GetsDrawn = false;
                GetsUpdated = false;
                UI_Handler.ZaWarudo = null;
                Game1.simulator.sim_comp.IsCompDrag = false;
                Game1.simulator.sim_comp.ComponentDrop(comp.ID);
            }
        }
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
     
            spritebatch.DrawString(comp.conf.font, comp.name, Game1.mo_states.New.Position.ToVector2(), comp.conf.fontcol);

        }
    }
}
