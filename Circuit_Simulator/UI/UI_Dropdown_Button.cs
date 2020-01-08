using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_Dropdown_Button : UI_Button
    {
        
        Texture2D tex;
        public Point tex_pos;
        public UI_Dropdown_Button(Pos pos, Point size, Point tex_pos, Texture2D tex, Generic_Conf conf) : base(pos, size, false, conf)
        {
            this.tex = tex; 
            this.tex_pos = tex_pos;
        }

        public override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

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
        }
        protected override void UpdateAlways()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (!hitbox.Contains(Game1.mo_states.New.Position))
                IsHovered = false;
            base.UpdateAlways();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

            if(IsActivated)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.ActiveColor);

            if (!IsHovered && !IsActivated)
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), conf.tex_color);
            else if (((new Rectangle(absolutpos, size)).Contains(Game1.mo_states.New.Position) && Game1.mo_states.New.LeftButton == ButtonState.Pressed))
            {
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), conf.tex_color);
            }
            else
            {
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), conf.tex_color);
            }

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), conf.BorderColor, 1);
           
        }
    }
    

}
