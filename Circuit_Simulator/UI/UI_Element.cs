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
	    public Vector2 pos;
	    public Vector2 absolutpos;
	    public byte ActivationStates;
        public UI_Element parent;
	    public List<Action> UpdateFunctions;
	    public List<Action> DrawFunctions;

        public UI_Element(Vector2 pos)
	    {
		    this.pos = pos;
		    this.parent = null;
			UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public UI_Element(Vector2 pos, UI_Element parent)
	    {
		    this.pos = pos;
		    this.parent = parent;
		    UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public virtual void Update()
	    {
		    absolutpos = parent == null ? pos : pos + parent.absolutpos;
		    for (int i = 0; i < UpdateFunctions.Count; ++i)
		    {
			    UpdateFunctions[i]();
		    }
	    }

	    public virtual void Draw(SpriteBatch spritebatch)
        {
		    for (int i = 0; i < DrawFunctions.Count; ++i)
		    {
			    DrawFunctions[i]();
            }
	    }
    }
}
