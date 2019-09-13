using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_ComponentBox : UI_Window
    {
        public List<UI_comp_cat> catlist;
        public UI_ComponentBox(Point pos, Point size, string title) : base(pos, size, title )
        {

        }
        public override void Add_UI_Element(UI_Element element)
        {
            base.Add_UI_Element(element);
            if (element.GetType() == typeof(UI_Component))
                element.size.X = size.X;
        }


    }
}
