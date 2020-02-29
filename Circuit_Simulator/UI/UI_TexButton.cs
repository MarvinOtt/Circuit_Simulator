using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_TexButton : UI_Button
    {
	    public Texture2D tex;
	    public Point tex_pos;

        public UI_TexButton(Pos pos, Point size, Point tex_pos, Texture2D tex, Generic_Conf conf) : base(pos, size, false, conf)
        {
            this.tex = tex;
            this.tex_pos = tex_pos;
        }

        public override void UpdateSpecific()
	    {
            
            base.UpdateSpecific();
	    }

        protected override void DrawSpecific(SpriteBatch spritebatch)
	    {
            if (conf.behav == 1)
            {
                if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), conf.tex_color);
                else if (IsHovered && !IsActivated)                                                                             //Hover
                {
                    spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), conf.tex_color);
                }
                else if (!IsHovered && IsActivated)                                                                             //Click
                {
                    spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 2 + 2), size), conf.tex_color);
                }
                else                                                                                                            //ClickHover
                {
                    spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), conf.tex_color);
                }
            }
            else
            {
                if(!IsHovered && !IsActivated)
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
                else if(((new Rectangle(absolutpos, size)).Contains(App.mo_states.New.Position) && App.mo_states.New.LeftButton == ButtonState.Pressed))
                {
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), conf.tex_color);
                }
                else
                {
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverColor);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), conf.tex_color);
                }
            }
            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), conf.BorderColor, 1);
        }  
    }
}
