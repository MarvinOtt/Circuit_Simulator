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
    public class UI_Component : UI_StringButton
    {
        public int ID, text_offset;
        //Vector2 text_pos;

        public UI_Component(Pos pos, Point size, string name, int ID, int text_offset, Generic_Conf conf) : base(pos, size, name, false, conf)
        {
            this.ID = ID;
            this.text_offset = text_offset;
            this.Sort_Name = name;
            //Vector2 textsize = conf.font.MeasureString(name);
            //text_pos = new Vector2(20, (int)(size.Y / 2 - textsize.Y / 2));
        }

        public override void ChangedUpdate2False()
        {
            IsHovered = false;
            base.ChangedUpdate2False();
        }

        public override void UpdateSpecific()
        {
            base.UpdateSpecific();
            //Rectangle hitbox = new Rectangle(absolutpos, size);

            //if (hitbox.Contains(Game1.mo_states.New.Position))
            //{
            //    if (Game1.mo_states.IsLeftButtonToggleOff())
            //    {
            //        UI_Handler.dragcomp.GetsUpdated = true;
            //        UI_Handler.dragcomp.GetsDrawn = true;
            //        UI_Handler.dragcomp.comp = this;
            //        UI_Handler.ZaWarudo = UI_Handler.dragcomp;
            //        UI_Handler.UI_Active_State = UI_Handler.UI_Active_CompDrag;
            //        Game1.simulator.sim_comp.InizializeComponentDrag(ID);
            //        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            //    }
            //    IsHovered = true;
            //}
        }

        protected override void UpdateAlways()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (!hitbox.Contains(Game1.mo_states.New.Position))
                IsHovered = false;
            base.UpdateAlways();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHovered)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
            spritebatch.DrawString(conf.font, text, new Vector2(absolutpos.X + text_offset, absolutpos.Y + size.Y / 2 - text_dim.Y / 2), conf.font_color);

        }
    }
}
