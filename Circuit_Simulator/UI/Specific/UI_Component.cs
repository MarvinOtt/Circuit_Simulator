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
        public static int height = 16;
        public string name;
        public bool IsGrab, IsHover;
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
            IsGrab = false;

            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHover = true;
            }
            else
                IsHover = false;
            
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHover)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[3]);
            spritebatch.DrawString(conf.font, name, absolutpos.ToVector2() + text_pos, conf.fontcol);
        }
    }
}
