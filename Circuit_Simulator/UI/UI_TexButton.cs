using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator
{
    public class TexButton : UI_Element
    {
	    public Texture2D tex;
	    public Point tex_pos;
        TexButton_Conf conf;
	    public bool IsHovered, IsActivated, GotActivated;




        public TexButton(Point pos, Point size, Point tex_pos, Texture2D tex, TexButton_Conf conf) : base(pos, size)
        {
            this.conf = conf;
            this.tex = tex;
            this.tex_pos = tex_pos;
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
            if (conf.behav == 1)
            {
                if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
                else if (IsHovered && !IsActivated)                                                                             //Hover
                {
                    if (conf.IsHoverEnabled)
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverCol);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), Color.White);
                }
                else if (!IsHovered && IsActivated)                                                                             //Click
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 2 + 2), size), Color.White);
                else                                                                                                            //ClickHover
                {
                    if (conf.IsHoverEnabled)
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverCol);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), Color.White);
                }
            }
            else
            {
                if(!IsHovered && !IsActivated)
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
                else if(((new Rectangle(absolutpos, size)).Contains(Game1.mo_states.New.Position) && Game1.mo_states.New.LeftButton == ButtonState.Pressed))
                {
                    if (conf.IsHoverEnabled)
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverCol);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3 + 3), size), Color.White);
                }
                else
                {
                    if(conf.IsHoverEnabled)
                        spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.HoverCol);
                    spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y + 1), size), Color.White);
                }
            }
        }  
    }
}
