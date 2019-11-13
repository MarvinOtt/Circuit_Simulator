using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Component : UI_StringButton
    {
        public int ID;
        //Vector2 text_pos;

        public UI_Component(Point size, string name, int ID, Generic_Conf conf) : base(Point.Zero, size, name, false, conf)
        {
            this.ID = ID;
            //Vector2 textsize = conf.font.MeasureString(name);
            //text_pos = new Vector2(20, (int)(size.Y / 2 - textsize.Y / 2));
        }

        public override void ChangedUpdate2False()
        {
            IsHovered = false;
            base.ChangedUpdate2False();
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    UI_Handler.dragcomp.GetsUpdated = true;
                    UI_Handler.dragcomp.GetsDrawn = true;
                    UI_Handler.dragcomp.comp = this;
                    UI_Handler.ZaWarudo = UI_Handler.dragcomp;
                    UI_Handler.UI_Active_State = UI_Handler.UI_Active_CompDrag;
                    Game1.simulator.sim_comp.InizializeComponentDrag(ID);
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                }
                IsHovered = true;
            }
            else
                IsHovered = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHovered)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
            spritebatch.DrawString(conf.font, text, new Vector2(absolutpos.X + 20, absolutpos.Y + size.Y / 2 - text_dim.Y / 2), conf.font_color);

        }
    }
}
