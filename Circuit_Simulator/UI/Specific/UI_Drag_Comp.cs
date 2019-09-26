using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
            if (Game1.mo_states.IsLeftButtonToggleOff())
            {
                comp.IsDrag = false;
                GetsDrawn = false;
                GetsUpdated = false;
                UI_Handler.ZaWarudo = null;
            }
        }
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
     
            spritebatch.DrawString(comp.conf.font, comp.name, Game1.mo_states.New.Position.ToVector2(), comp.conf.fontcol);

        }
    }
}
