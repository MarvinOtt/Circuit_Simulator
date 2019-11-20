using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_ComponentBox : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Catagories;
        public static Rectangle cathitbox;

        public UI_ComponentBox(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable)
        {
            Catagories = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Catagories.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            Add_UI_Elements(Catagories);
        }

        protected override void Resize()
        {
            Catagories.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
          
        }

        public void Add_Categories(params UI_Categorie<UI_Component>[] cats)
        {
            cats.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            for(int i = 0; i < cats.Length; ++i)
            {
                for (int j = 0; j < cats[i].Components.ui_elements.Count; ++j)
                {
                    cats[i].Components.ui_elements[j].GotActivatedLeft += PlaceComp;
                }
            }
            Catagories.ui_elements[0].Add_UI_Elements(cats);
        }

        public void PlaceComp(object sender)
        {
            UI_Component comp = sender as UI_Component;
            UI_Handler.dragcomp.GetsUpdated = true;
            UI_Handler.dragcomp.GetsDrawn = true;
            UI_Handler.dragcomp.comp = comp;
            UI_Handler.ZaWarudo = UI_Handler.dragcomp;
            UI_Handler.UI_Active_State = UI_Handler.UI_Active_CompDrag;
            Game1.simulator.sim_comp.InizializeComponentDrag(comp.ID);
            System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
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
