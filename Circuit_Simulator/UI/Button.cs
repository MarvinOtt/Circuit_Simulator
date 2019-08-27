using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circuit_Simulator
{
    public class Button : UI_Element
    {
	    public Texture2D tex;
	    public Point tex_pos;
	    public bool IsHovered, IsActivated, GotActivated;
	    private byte config;




        public Button(Point pos, Point size, Point tex_pos, Texture2D tex, byte config) : base(pos, size)
        {
            this.tex = tex;
            
            this.tex_pos = tex_pos;
            this.config = config;
        }

	    protected override void UpdateSpecific()
	    {
			Rectangle hitbox = new Rectangle(absolutpos, size);
            GotActivated = false;
		    if (config == 2)
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
            if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
            else if (IsHovered && !IsActivated)                                                                             //Hover
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y), size), Color.White);
            else if (!IsHovered && IsActivated)                                                                             //Click
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 2), size), Color.White);
            else                                                                                                            //PostClickHover
                spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos + new Point(0, size.Y * 3), size), Color.White);
        }  
    }
}
