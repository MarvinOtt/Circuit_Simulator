using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_ComponentBox : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Catagories;
        public static Rectangle cathitbox;

        public UI_ComponentBox(Point pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable)
        {
            Catagories = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Point(bezelsize, 50), Point.Zero);
            Catagories.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Point.Zero, false));
            Add_UI_Elements(Catagories);
        }

        protected override void Resize()
        {
            Catagories.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
          
        }

        public void Add_Categories(params UI_Categorie<UI_Component>[] cats)
        {
            cats.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            Catagories.ui_elements[0].Add_UI_Elements(cats);
        }

        protected override void UpdateSpecific()
        {

            base.UpdateSpecific();
            Catagories.size = new Point(size.X - bezelsize * 2, size.Y - 50 - bezelsize);

        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
          
        }

    }
}
