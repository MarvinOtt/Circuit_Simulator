using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator
{
    public class UI_MultiElement<T> : UI_Element where T : UI_Element
    {
	    public List<T> ui_elements;

        public UI_MultiElement(Point pos) : base(pos, Point.Zero)
        {
			ui_elements = new List<T>();
        }
        public UI_MultiElement(Point pos, Point size) : base(pos, size)
        {
            ui_elements = new List<T>();
        }
        public UI_MultiElement(Point pos, Point size, UI_Element parent, Color bgc) : base(pos, size, parent)
	    {
		    ui_elements = new List<T>();
	    }

        public virtual void Add_UI_Elements(params T[] elements)
	    {
            foreach (T element in elements)
            {
                element.parent = this;
                if (element.pos.X + element.size.X > size.X)
                    size.X = element.pos.X + element.size.X;
                if (element.pos.Y + element.size.Y > size.Y)
                    size.Y = element.pos.Y + element.size.Y;
            }
            ui_elements.AddRange(elements);
	    }

        public override void ChangedUpdate2False()
        {
            base.ChangedUpdate2False();
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].ChangedUpdate2False();
            }
        }

        public override void UpdatePos()
        {
            base.UpdatePos();
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].UpdatePos();
            }
        }

        protected override void UpdateAlways()
	    {
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].Update();
            }
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
	    {
		    for (int i = 0; i < ui_elements.Count; ++i)
		    {
			    ui_elements[i].Draw(spritebatch);
		    }
        }
    }
}
