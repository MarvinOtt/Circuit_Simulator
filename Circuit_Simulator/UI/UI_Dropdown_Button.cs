using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public class UI_Dropdown_Button : UI_Element
    {
        Color BackgroundColor;
        Color BorderColor;
        Color tex_color;
        Texture2D tex;
        public bool IsHovered, IsActivated, GotActivated;
        public Point tex_pos;
        public UI_Dropdown_Button(Point pos, Point size, Point tex_pos, Color tex_color, Texture2D tex) : base(pos, size)
        {
            BackgroundColor = UI_Handler.main_BG_Col;
            BorderColor = UI_Handler.BorderColor;
            this.tex = tex;
            this.tex_pos = tex_pos;
            this.tex_color = tex_color;
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            GotActivated = false;

            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.New.LeftButton == ButtonState.Pressed)
                {
                    IsActivated = true;
                }
                else if(Game1.mo_states.New.RightButton == ButtonState.Pressed)
                {
                    IsActivated = false;
                }
            }
            else
                IsHovered = false;
            base.UpdateSpecific();

        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);

            base.DrawSpecific(spritebatch);

            if(IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.ActivColor);

            if (!IsHovered && !IsActivated)
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), tex_color);
            else if (((new Rectangle(absolutpos, size)).Contains(Game1.mo_states.New.Position) && Game1.mo_states.New.LeftButton == ButtonState.Pressed))
            {
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.main_Hover_Col);
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), tex_color);
            }
            else
            {
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.main_Hover_Col);
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), tex_color);
            }

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
           
        }
    }
    

}
