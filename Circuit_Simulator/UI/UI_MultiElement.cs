using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator
{
    public class UI_MultiElement : UI_Element
    {
	    public List<UI_Element> ui_elements;

        public UI_MultiElement(Point pos) : base(pos, Point.Zero)
        {
			ui_elements = new List<UI_Element>();
        }
        public UI_MultiElement(Point pos, Point size) : base(pos, size)
        {
            ui_elements = new List<UI_Element>();
        }
        public UI_MultiElement(Point pos, Point size, UI_Element parent, Color bgc) : base(pos, size, parent)
	    {
		    ui_elements = new List<UI_Element>();
	    }

        public void Add_UI_Element(UI_Element element)
	    {
		    element.parent = this;
			ui_elements.Add(element);
	    }

	    protected override void UpdateSpecific()
	    {
		    for (int i = 0; i < ui_elements.Count; ++i)
		    {
				ui_elements[i].Update();
		    }
	    }

	    public override void DrawSpecific(SpriteBatch spritebatch)
	    {
		    for (int i = 0; i < ui_elements.Count; ++i)
		    {
			    ui_elements[i].Draw(spritebatch);
		    }
        }
    }
}
