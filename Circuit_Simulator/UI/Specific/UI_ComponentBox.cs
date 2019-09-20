using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_ComponentBox : UI_Window
    {
        public UI_List<UI_Comp_Cat> Catagories;
        public UI_ComponentBox(Point pos, Point size, string title) : base(pos, size, title )
        {
            Catagories = new UI_List<UI_Comp_Cat>(new Point(0, 50));
            Add_UI_Elements(Catagories);
        }

        protected override void Resize()
        {
            Catagories.ui_elements.ForEach(x => x.SetXSize(size.X));
            //ui_elements.Where(x => x.GetType() == typeof(UI_Comp_Cat)).ForEach(c => ((UI_Comp_Cat)c).SetXSize(size.X));
        }

        public void Add_Categories(params UI_Comp_Cat[] cats)
        {
            cats.ForEach(x => x.SetXSize(size.X));
            Catagories.Add_UI_Elements(cats);
        }

        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }

    }
}
