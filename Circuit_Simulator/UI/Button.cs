using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.Configs;

namespace Circuit_Simulator.UI
{
    public class Button : UI_Element
    {
        Button_Conf conf;
        Vector2 text_pos;
        string text;
        public bool IsHovered, IsActivated, GotActivated;
        private byte behav;




        public Button(Point pos, Point size, string text, Button_Conf conf, byte behav) : base(pos, size)
        {
            this.text = text;
            this.conf = conf;
            this.behav = behav;
            Vector2 textsize = conf.font.MeasureString(text);
            text_pos = size.ToVector2() / 2 - textsize / 2;

        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            GotActivated = false;
            if (behav == 2)
                IsActivated = false;
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsActivated ^= true;
                    if (IsActivated)
                        GotActivated = true;
                }
            }
            else
                IsHovered = false;
        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[(IsHovered ? 1 : 0) + (IsActivated ? 2 : 0)]);
            spritebatch.DrawString(conf.font, text, absolutpos.ToVector2() + text_pos, conf.fontcol);
            //if (!IsHovered && !IsActivated)                                                                                 //PassiveState
            //    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
            //else if (IsHovered && !IsActivated)                                                                             //Hover
            //    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y), size), Color.White);
            //else if (!IsHovered && IsActivated)                                                                             //Click
            //    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 2), size), Color.White);
            //else                                                                                                            //PostClickHover
            //    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3), size), Color.White);
        }
    }
}
