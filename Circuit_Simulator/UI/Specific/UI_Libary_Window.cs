using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Libary_Window : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Libraries;
        public static Rectangle libhitbox;

        public UI_Libary_Window(Pos pos, Point size, string Title, Point minsize, Generic_Conf conf, bool IsResizeable = true) : base(pos, size, Title, minsize, conf, IsResizeable)
        {
            Libraries = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Libraries.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            UI_StringButton AddButton = new UI_StringButton(new Pos(-bezelsize, ORIGIN.BR, ORIGIN.BR), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            UI_StringButton Open = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, AddButton), new Point((int)(UI_Handler.buttonwidth), UI_Handler.buttonheight), "Open", true, UI_Handler.genbutconf);
            UI_StringButton SaveAll = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, Open), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Save All", true, UI_Handler.genbutconf);

            Add_UI_Elements(AddButton, Open, SaveAll);
            Add_UI_Elements(Libraries);
        }
       
        protected override void Resize()
        {
            Libraries.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));

        }

        public void Add_Library( CompLibrary libs)
        {
            UI_Categorie<UI_Component> newlib = new UI_Categorie<UI_Component>(libs.name, UI_Handler.cat_conf);
            newlib.cat.ID = CompLibrary.AllLibraries.IndexOf(libs);
            newlib.cat.GotActivatedRight += DeleteLib;
            newlib.GotFolded += LibFolded;
            newlib.Fold(libs.IsFold);
            for(int i = 0; i < libs.Components.Count; i++)
            {
                int ID = Sim_Component.Components_Data.IndexOf(libs.Components[i]);
                UI_Component cur_comp = new UI_Component(new Pos(0), new Point(20, 20), libs.Components[i].name, ID, 20, UI_Handler.componentconf);
                cur_comp.GotActivatedLeft += EditComp;
                cur_comp.GotActivatedRight += DeleteComp;

                newlib.AddComponents(cur_comp);
            }
            newlib.SetXSize(size.X - bezelsize * 2);
            Libraries.ui_elements[0].Add_UI_Elements(newlib);
            //libs.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            //Libaries.ui_elements[0].Add_UI_Elements(libs);
        }

        public void EditComp(object sender)
        {
            UI_Component comp = sender as UI_Component;
            UI_Handler.editcompwindow.GetsUpdated = UI_Handler.editcompwindow.GetsDrawn = true;
            UI_Window.All_Highlight(UI_Handler.editcompwindow);
            UI_Handler.editcompwindow.rootcomp = Sim_Component.Components_Data[comp.ID];

        }
        public void DeleteComp(object sender)
        {
            UI_Component comp = sender as UI_Component;
            CompData compdata = Sim_Component.Components_Data[comp.ID];
            CompLibrary lib = compdata.library;
            int libindex = lib.Components.IndexOf(compdata);
            lib.Components.RemoveAt(libindex);
            Sim_Component.Components_Data.RemoveAt(comp.ID);
            UI_Handler.InitComponents();
        }
        public void DeleteLib(object sender)
        {
            UI_Component lib = sender as UI_Component;
        }
        public void LibFolded(object sender)
        {
            UI_Categorie<UI_Component> curUIlib = sender as UI_Categorie<UI_Component>;
            CompLibrary curlib = CompLibrary.AllLibraries[curUIlib.cat.ID];
            curlib.IsFold = curUIlib.IsFold;
        }
        public void EditLib(object sender)
        {
            UI_Categorie<UI_Component> curUIlib = sender as UI_Categorie<UI_Component>;
            CompLibrary curlib = CompLibrary.AllLibraries[curUIlib.cat.ID];
        }

        protected override void UpdateSpecific()
        {

            base.UpdateSpecific();
            Libraries.size = new Point(size.X - bezelsize * 2, size.Y - bezelsize * 2 - Libraries.pos.Y - UI_Handler.buttonheight);

        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

        }
    }
}
