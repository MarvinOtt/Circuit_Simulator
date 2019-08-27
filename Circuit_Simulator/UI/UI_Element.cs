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
        public bool GetsDrawn = true, GetsUpdated = true;
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
            if (GetsUpdated)
            {
                UpdateSpecific();
                child?.Update();
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
            if (GetsDrawn)
            {
                DrawSpecific(spritebatch);
                child?.Draw(spritebatch);
            }
		    for (int i = 0; i < DrawFunctions.Count; ++i)
		    {
			    DrawFunctions[i]();
            }
	    }

        public virtual void DrawSpecific(SpriteBatch spritebatch)
        {
            // Should be overridden
        }
    }
}
