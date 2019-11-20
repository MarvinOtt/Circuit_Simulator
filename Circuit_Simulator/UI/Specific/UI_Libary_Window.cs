using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Libary_Window : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Libaries;
        public static Rectangle libhitbox;

        public UI_Libary_Window(Pos pos, Point size, string Title, Point minsize, Generic_Conf conf, bool IsResizeable = true) : base(pos, size, Title, minsize, conf, IsResizeable)
        {
            Libaries = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Libaries.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            UI_StringButton AddButton = new UI_StringButton(new Pos(-UI_Handler.buttonwidth - bezelsize, -UI_Handler.buttonheight - bezelsize, ORIGIN.BOTTOMRIGHT), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            UI_StringButton Save = new UI_StringButton(new Pos(-bezelsize - (int)(UI_Handler.buttonwidth * 2.5) - bezelsize, -bezelsize - UI_Handler.buttonheight, ORIGIN.BOTTOMRIGHT), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Save All", true, UI_Handler.genbutconf);
            //UI_StringButton AddButton = new UI_StringButton(new Pos( -bezelsize - UI_Handler.buttonwidth, -bezelsize - UI_Handler.buttonheight - 1), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            //UI_StringButton Save = new UI_StringButton(new Pos(-bezelsize - (int)(UI_Handler.buttonwidth * 2.5) -bezelsize, -bezelsize - UI_Handler.buttonheight - 1), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Save All", true, UI_Handler.genbutconf);
            Add_UI_Elements(AddButton, Save);
            Add_UI_Elements(Libaries);
        }
       
        protected override void Resize()
        {
            Libaries.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));

        }

        public void Add_Libraries(params UI_Categorie<UI_Component>[] libs)
        {
            libs.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            Libaries.ui_elements[0].Add_UI_Elements(libs);
        }

        protected override void UpdateSpecific()
        {

            base.UpdateSpecific();
            Libaries.size = new Point(size.X - bezelsize * 2, size.Y - bezelsize * 2 - Libaries.pos.Y - UI_Handler.buttonheight);

        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

        }
    }
}
