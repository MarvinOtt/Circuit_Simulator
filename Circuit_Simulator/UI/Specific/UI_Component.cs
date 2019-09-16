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
    public class UI_Component : UI_Element
    {
        public static int height = 20;
        public string name;
        public int ID;
        public bool IsDrag, IsHover;
        Button_Conf conf;
        Vector2 text_pos;

        public UI_Component(string name, Button_Conf conf) : base(Point.Zero, new Point(0, height))
        {
            
            this.name = name;
            this.conf = conf;
            Vector2 textsize = conf.font.MeasureString(name);
            text_pos = new Vector2(20, (int)(size.Y / 2 - textsize.Y / 2));
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            IsDrag = false;

            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsDrag = true;
                }

                IsHover = true;
            }
            else
                IsHover = false;
            if (IsDrag && Game1.mo_states.IsLeftButtonToggleOff())
            {
                IsDrag = false;
            }
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHover)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[1]);
            spritebatch.DrawString(conf.font, name, absolutpos.ToVector2() + text_pos, conf.fontcol);
        }
    }
}
