using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    public class UI_Button : UI_Element
    {
        Button_Conf conf;
        Vector2 text_pos;
        string text;
        public bool IsHovered, IsActivated, GotActivated;

        public UI_Button(Point pos, Point size, string text, Button_Conf conf) : base(pos, size)
        {
            this.text = text;
            this.conf = conf;
            Vector2 textsize = conf.font.MeasureString(text);
            text_pos = size.ToVector2() / 2 - textsize / 2;

        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            GotActivated = false;
            if (conf.behav == 2)
                IsActivated = false;
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    IsActivated ^= true;
                    if (IsActivated)
                        GotActivated = true;
                }
            }
            else
                IsHovered = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[(IsHovered ? 1 : 0) + (IsActivated ? 2 : 0)]);
            spritebatch.DrawString(conf.font, text, absolutpos.ToVector2() + text_pos, conf.fontcol);
        }
    }
}
