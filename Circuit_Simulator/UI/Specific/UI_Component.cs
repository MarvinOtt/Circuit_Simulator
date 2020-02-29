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

        public UI_Component(Pos pos, Point size, string name, int ID, int text_offset, Generic_Conf conf) : base(pos, size, name, false, conf)
        {
            this.ID = ID;
            this.text_offset = text_offset;
            this.Sort_Name = name;
          
        }

        public override void ChangedUpdate2False()
        {
            IsHovered = false;
            base.ChangedUpdate2False();
        }

        public override void UpdateSpecific()
        {
            base.UpdateSpecific();
          
        }

        protected override void UpdateAlways()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (!hitbox.Contains(App.mo_states.New.Position))
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
