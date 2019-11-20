using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_StringButton : UI_Button
    {
        public string text;
        public Vector2 text_dim;
        public UI_StringButton(Pos pos, Point size, string text, bool DrawBorder, Generic_Conf conf) : base(pos, size, DrawBorder, conf)
        {
            this.text = text;
            text_dim = conf.font.MeasureString(text);
    
        }

        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
            if (IsHovered && !IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
            if (IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.ActiveColor);

            spritebatch.DrawString(conf.font, text, new Vector2(absolutpos.X + size.X / 2 - text_dim.X / 2 , absolutpos.Y + size.Y / 2 - text_dim.Y / 2), conf.font_color);
        }
    }
}
