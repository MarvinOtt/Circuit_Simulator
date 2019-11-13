using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    public class Button_Menu : UI_Button
    {
        string Title;
        float Title_height;
        public Button_Menu(Point pos, Point size, string Title, Generic_Conf conf) : base(pos, size, false, conf)
        {  
            Vector2 title_dim = conf.font.MeasureString(Title);
            Title_height = title_dim.Y;
            if (16 + title_dim.X + 8 > size.X)
                size.X = 8 + (int)title_dim.X + 8;
            this.Title = Title;
            this.size = size;
        }

        protected override void UpdateSpecific()
        {
            size.X = parent.size.X;
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (IsHovered && !IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
            if(IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.ActiveColor);

            spritebatch.DrawString(conf.font, Title, new Vector2(absolutpos.X + 8, absolutpos.Y + size.Y / 2 - Title_height / 2), conf.font_color);
        }
    }
}
