using Circuit_Simulator.COMP;
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
    public class UI_ProjectLibrary_Window : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Libraries;
        UI_StringButton AddButton, Reload;
        public static bool IsChange;


        public UI_ProjectLibrary_Window(Pos pos, Point size, string title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable)
        {
            Libraries = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Libraries.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            AddButton = new UI_StringButton(new Pos(-bezelsize, ORIGIN.BR, ORIGIN.BR), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            Reload = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, AddButton), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Reload", true, UI_Handler.genbutconf);
            Add_UI_Elements(AddButton, Reload);
            Add_UI_Elements(Libraries);


            AddButton.GotActivatedLeft += OpenLib;
            Reload.GotActivatedLeft += ReloadComponentBox;
            UI_Handler.EditProjectLib.ui_elements[0].GotActivatedLeft += DeleteLib;
        }

        public void EditProjectLib(object sender)
        {
            UI_Component curUIlib = sender as UI_Component;
            CompLibrary curlib = CompLibrary.AllUsedLibraries[curUIlib.ID];
            UI_Handler.EditProjectLib.ID_Name = curlib.name;
            UI_Handler.EditProjectLib.GetsUpdated = UI_Handler.EditProjectLib.GetsDrawn = true;
            UI_Handler.EditProjectLib.pos.pos = Game1.mo_states.New.Position + new Point(5, 5);
            UI_Handler.EditProjectLib.UpdatePos();
            //currlibID = curUIlib.ID;
        }
        public void OpenLib(object sender)
        {
            CompLibrary.LoadFromFile(true);
            CompLibrary.AllUsedLibraries.RemoveAll(x => x.STATE == CompLibrary.LOAD_FAILED);
            Reload_UI();
        }
        public void ReloadComponentBox(object sender)
        {
            CompLibrary.ReloadComponentData();
            UI_Handler.InitComponents();
        }

        public void LibFolded(object sender)
        {
            UI_Categorie<UI_Component> curUIlib = sender as UI_Categorie<UI_Component>;
            CompLibrary curlib = CompLibrary.AllUsedLibraries.Find(x => x.name == curUIlib.cat.ID_Name);
            curlib.IsFold = curUIlib.IsFold;
        }

        public void DeleteLib(object sender)
        {
            IsChange = true;
            UI_Element lib = sender as UI_Element;
            CompLibrary.AllUsedLibraries.RemoveAll(x => x.name == lib.parent.ID_Name);
            Reload_UI();
        }

        public void Add_Library(CompLibrary libs)
        {
            UI_Categorie<UI_Component> newlib = new UI_Categorie<UI_Component>(libs.name, UI_Handler.cat_conf);
            newlib.cat.ID = CompLibrary.AllUsedLibraries.IndexOf(libs);
            newlib.cat.ID_Name = libs.name;
            newlib.cat.GotActivatedRight += EditProjectLib; 
            newlib.GotFolded += LibFolded;
            newlib.Fold(libs.IsFold);
            for(int i = 0; i < libs.Components.Count; i++)
            {
                int ID = i;
                UI_Component cur_comp = new UI_Component(new Pos(0), new Point(20, 20), libs.Components[i].name, ID, 20, UI_Handler.componentconf);
                cur_comp.ID_Name = libs.name + "|" + libs.Components[i].name;


                newlib.AddComponents(cur_comp);
            }
            newlib.SetXSize(size.X - bezelsize * 2);
            Libraries.ui_elements[0].Add_UI_Elements(newlib);
   
        }
        public void Reload_UI()
        {
            Libraries.ui_elements[0].ui_elements.Clear();
            for (int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
            {
                Add_Library(CompLibrary.AllUsedLibraries[i]);
            }
        }

       

        public override void ChangedUpdate2True()
        {
            for (int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
            {
                int index = CompLibrary.LibraryWindow_LoadedLibrarys.FindIndex(x => x.name == CompLibrary.AllUsedLibraries[i].name);
                if (index == -1)
                {
                    CompLibrary newlib = new CompLibrary(null, CompLibrary.AllUsedLibraries[i].SaveFile, false);
                    newlib.Load();
                }
            }

            Reload_UI();

            base.ChangedUpdate2True();
        }

        public override void UpdateSpecific()
        {
            base.UpdateSpecific();
            Libraries.size = new Point(size.X - bezelsize * 2, size.Y - bezelsize * 2 - Libraries.pos.Y - UI_Handler.buttonheight);
            base.UpdatePos();
        }
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

        }
    }

   
}
