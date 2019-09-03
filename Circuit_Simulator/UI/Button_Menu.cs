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
        private byte config;
        string Title;
        float Title_height;

        public Button_Menu(Point pos, Point size, string Title, byte config) : base(pos, size)
        {
            if (font == null)
                font = Game1.content.Load<SpriteFont>("UI\\TB_Dropdown_font");
            Vector2 title_dim = font.MeasureString(Title);
            Title_height = title_dim.Y;
            if (16 + title_dim.X + 8 > size.X)
                size.X = 16 + (int)title_dim.X + 8;
            this.Title = Title;
            this.size = size;
            this.config = config;
        }

        protected override void UpdateSpecific()
        {
            size.X = parent.size.X;
            Rectangle hitbox = new Rectangle(absolutpos, size);
            if (config == 2)
                IsActivated = false;
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                    IsActivated ^= true;
            }
            else
                IsHovered = false;
        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (IsHovered && !IsActivated)                                                                             //Hover
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.main_Hover_Col);

            spritebatch.End();
            spritebatch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, null, null, Matrix.Identity);
            spritebatch.DrawString(font, Title, new Vector2(absolutpos.X + 16, absolutpos.Y + size.Y / 2 - Title_height / 2), Color.White);
            spritebatch.End();
            spritebatch.Begin();
            /*if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.main_BG_Col);
            else if (IsHovered && !IsActivated)                                                                             //Hover
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Red);
            else if (!IsHovered && IsActivated)                                                                             //Click
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Purple);
            else                                                                                                            //PostClickHover
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Red);*/

        }
    }
}
