using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator
{
    public class UI_Element
    {
	    public Point pos, size;
	    public Point absolutpos;
        public bool GetsDrawn = true, GetsUpdated = true, CanBeSizeRelated = true;
        public UI_Element parent;
        public UI_Element child;
        public List<Action> UpdateFunctions;
	    public List<Action> DrawFunctions;

        public UI_Element(Point pos, Point size)
	    {
		    this.pos = pos;
            this.size = size;
            UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public UI_Element(Point pos, Point size, UI_Element parent)
	    {
		    this.pos = pos;
            this.size = size;
		    this.parent = parent;
		    UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public void Update()
	    {
            
		    absolutpos = parent == null ? pos : pos + parent.absolutpos;
            if(parent != null && CanBeSizeRelated)
            {
                if (pos.X < 0)
                    absolutpos.X += parent.size.X;
                if (pos.Y < 0)
                    absolutpos.Y += parent.size.Y;
            }
            if (GetsUpdated)
            {
                if(!UI_Handler.UI_Element_Pressed)
                    UpdateSpecific();
                child?.Update();
                if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position) && (Game1.mo_states.IsLeftButtonToggleOn() || Game1.mo_states.IsLeftButtonToggleOff()))
                    UI_Handler.UI_Element_Pressed = true;
                if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position))
                    UI_Handler.UI_Active = true;
            }
		    for (int i = 0; i < UpdateFunctions.Count; ++i)
		    {
			    UpdateFunctions[i]();
		    }
	    }

        protected virtual void UpdateSpecific()
        {
            // Should be overridden
        }

	    public void Draw(SpriteBatch spritebatch)
        {
            absolutpos = parent == null ? pos : pos + parent.absolutpos;
            if (parent != null && CanBeSizeRelated)
            {
                if (pos.X < 0)
                    absolutpos.X += parent.size.X;
                if (pos.Y < 0)
                    absolutpos.Y += parent.size.Y;
            }
            if (GetsDrawn)
            {
                DrawSpecific(spritebatch);
                child?.Draw(spritebatch);
                for (int i = 0; i < DrawFunctions.Count; ++i)
                {
                    DrawFunctions[i]();
                }
            }

	    }

        protected virtual void DrawSpecific(SpriteBatch spritebatch)
        {
            // Should be overridden
        }
    }
}
