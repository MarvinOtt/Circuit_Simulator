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
        public Button source;
	    public Texture2D tex;
	    public Point size, tex_pos;
	    public bool IsHovered, IsActivated;
	    private byte config;




        public Button(Vector2 pos, Point size, Point tex_pos, Texture2D tex, byte config) : base(pos)
        {
            this.size = size;
            this.tex = tex;
            
            this.tex_pos = tex_pos;
            this.config = config;
        }

        public Button(Vector2 pos, Point size, Point tex_pos, UI_Element source, Texture2D tex, byte config) : base(pos)
	    {
		    this.size = size;
		    this.tex = tex;
            this.source = (Button)source;
		    this.tex_pos = tex_pos;
		    this.config = config;
	    }

	    protected override void UpdateSpecific()
	    {
			Rectangle hitbox = new Rectangle(absolutpos.ToPoint(), size);
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
       
	    public override void Draw(SpriteBatch spritebatch)
	    {
            if (tex.Width > 1) //PreDrawn
            {
                base.Draw(spritebatch);
                if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                    spritebatch.Draw(tex, absolutpos, new Rectangle(tex_pos, size), Color.White);
                else if (IsHovered && !IsActivated)                                                                             //Hover
                    spritebatch.Draw(tex, absolutpos, new Rectangle(tex_pos + new Point(0, size.Y), size), Color.White);
                else if (!IsHovered && IsActivated)                                                                             //Click
                    
                    spritebatch.Draw(tex, absolutpos, new Rectangle(tex_pos + new Point(0, size.Y * 2), size), Color.White);
                else                                                                                                            //PostClickHover
                    spritebatch.Draw(tex, absolutpos, new Rectangle(tex_pos + new Point(0, size.Y * 3), size), Color.White);
            }
            else if(source.IsActivated) //LiveDrawn
            {
                if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                    Extensions.DrawFilledRectangle(spritebatch, new Rectangle((int)pos.X, (int)pos.Y, size.X, size.Y), Color.Blue);
                else if (IsHovered && !IsActivated)                                                                             //Hover
                    Extensions.DrawFilledRectangle(spritebatch, new Rectangle((int)pos.X, (int)pos.Y, size.X, size.Y), Color.Red);
                else if (!IsHovered && IsActivated)                                                                             //Click
                    Extensions.DrawFilledRectangle(spritebatch, new Rectangle((int)pos.X, (int)pos.Y, size.X, size.Y), Color.Purple);
                else                                                                                                            //PostClickHover
                    Extensions.DrawFilledRectangle(spritebatch, new Rectangle((int)pos.X, (int)pos.Y, size.X, size.Y), Color.Red);
            }

        }

        
    }
}
