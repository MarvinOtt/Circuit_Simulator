using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public class Button_Menu : UI_Element
    {
        public bool IsHovered, IsActivated;
        private static SpriteFont font;
        string Title;
        float Title_height;

        public Button_Menu(Point pos, Point size, string Title) : base(pos, size)
        {
            if (font == null)
                font = Game1.content.Load<SpriteFont>("UI\\TB_Dropdown_font");
            Vector2 title_dim = font.MeasureString(Title);
            Title_height = title_dim.Y;
            if (16 + title_dim.X + 8 > size.X)
                size.X = 8 + (int)title_dim.X + 8;
            this.Title = Title;
            this.size = size;
        }

        protected override void UpdateSpecific()
        {
            size.X = parent.size.X;
            Rectangle hitbox = new Rectangle(absolutpos, size);
            IsActivated = false;
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.IsLeftButtonToggleOff())
                    IsActivated ^= true;
            }
            else
                IsHovered = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (IsHovered && !IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.main_Hover_Col);

            spritebatch.DrawString(font, Title, new Vector2(absolutpos.X + 8, absolutpos.Y + size.Y / 2 - Title_height / 2), Color.White);
        }
    }
}
