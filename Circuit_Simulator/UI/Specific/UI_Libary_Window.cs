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
        //UI_String NeedSavingString;
        UI_StringButton AddButton, Open, SaveAll, Reload;
        UI_ValueInput RenameBox;
        public static bool IsChange;

        public UI_Libary_Window(Pos pos, Point size, string Title, Point minsize, Generic_Conf conf, bool IsResizeable = true) : base(pos, size, Title, minsize, conf, IsResizeable)
        {
            Libraries = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Libraries.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            AddButton = new UI_StringButton(new Pos(-bezelsize, ORIGIN.BR, ORIGIN.BR), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            Open = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, AddButton), new Point((int)(UI_Handler.buttonwidth), UI_Handler.buttonheight), "Open", true, UI_Handler.genbutconf);
            SaveAll = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, Open), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Save All", true, UI_Handler.genbutconf);
            Reload = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, SaveAll), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Reload", true, UI_Handler.genbutconf);
            RenameBox = new UI_ValueInput(new Pos(0, 0), Point.Zero, UI_Handler.genbutconf, 3);

            //NeedSavingString = new UI_String(new Pos(10, 10 + headheight), Point.Zero, conf, "In order to edit Librarys, the circuit has to be saved.");

            Add_UI_Elements(RenameBox, AddButton, Open, SaveAll, Reload);
            Add_UI_Elements(Libraries);

            //Reload.GotActivatedLeft += Reload_All;

            UI_Handler.EditLib.ui_elements[0].GotActivatedLeft += AddComp;
            UI_Handler.EditLib.ui_elements[1].GotActivatedLeft += DeleteLib;
            UI_Handler.EditLib.ui_elements[2].GotActivatedLeft += RenameLib;
            RenameBox.ValueChanged += Rename_Finish;


        }
       
        protected override void Resize()
        {
            Libraries.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));

        }

        public void Add_Library(CompLibrary libs)
        {
            UI_Categorie<UI_Component> newlib = new UI_Categorie<UI_Component>(libs.name, UI_Handler.cat_conf);
            newlib.cat.ID = CompLibrary.LibraryWindow_LoadedLibrarys.IndexOf(libs);
            newlib.cat.ID_Name = libs.name;
            newlib.cat.GotActivatedRight += EditLib; 
            newlib.GotFolded += LibFolded;
            newlib.Fold(libs.IsFold);
            for(int i = 0; i < libs.Components.Count; i++)
            {
                int ID = i;
                UI_Component cur_comp = new UI_Component(new Pos(0), new Point(20, 20), libs.Components[i].name, ID, 20, UI_Handler.componentconf);
                cur_comp.ID_Name = libs.name + "|" + libs.Components[i].name;
                cur_comp.GotActivatedLeft += EditComp;
                cur_comp.GotActivatedRight += DeleteComp;

                newlib.AddComponents(cur_comp);
            }
            newlib.SetXSize(size.X - bezelsize * 2);
            Libraries.ui_elements[0].Add_UI_Elements(newlib);
   
        }

        public override void ChangedUpdate2True()
        {
            for(int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
            {
                int index = CompLibrary.LibraryWindow_LoadedLibrarys.FindIndex(x => x.name == CompLibrary.AllUsedLibraries[i].name);
                if(index == -1)
                {
                    CompLibrary newlib = new CompLibrary(CompLibrary.AllUsedLibraries[i].name, CompLibrary.AllUsedLibraries[i].SaveFile, false);
                    newlib.Load();
                }
            }

            Reload_UI();

            base.ChangedUpdate2True();
        }

        public void Reload_UI()
        {
            Libraries.ui_elements[0].ui_elements.Clear();
            for (int i = 0; i < CompLibrary.LibraryWindow_LoadedLibrarys.Count; ++i)
            {
                Add_Library(CompLibrary.LibraryWindow_LoadedLibrarys[i]);
            }
        }

        //public void Reload_All(object sender)
        //{
        //    IsChange = false;
        //    Sim_Component.Components_Data.Clear();
        //    for(int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
        //    {
        //        CompLibrary curlib = CompLibrary.AllUsedLibraries[i];
        //        Sim_Component.Components_Data.AddRange(curlib.Components);
        //    }
        //    UI_Handler.InitComponents();
        //    FileHandler.OpenCurrent();

        //}
        public void AddComp(object sender)
        {

        }
        public void EditComp(object sender)
        {
            IsChange = true;
            UI_Component comp = sender as UI_Component;
            
            UI_Handler.editcompwindow.GetsUpdated = UI_Handler.editcompwindow.GetsDrawn = true;
            UI_Window.All_Highlight(UI_Handler.editcompwindow);
            UI_Handler.editcompwindow.rootcomp = Sim_Component.Components_Data[comp.ID];

        }
        public void DeleteComp(object sender)
        {
            IsChange = true;
            UI_Component comp = sender as UI_Component;
            string[] names = comp.ID_Name.Split('|');
            UI_List<UI_Component> complist = (comp.parent as UI_List<UI_Component>);
            CompLibrary.LibraryWindow_LoadedLibrarys.Find(x => x.name == names[0]).Components.RemoveAll(x => x.name == names[1]); //RemoveAt(complist.ui_elements.IndexOf(comp));
            //complist.ui_elements.Remove(comp);
            Reload_UI();
            //CompData compdata = Sim_Component.Components_Data[comp.ID];
            //CompLibrary lib = compdata.library;
            //int libindex = lib.Components.IndexOf(compdata);
            ////Sim_Component.Components_Data.Remove(compdata);
            //lib.Components.RemoveAt(libindex);
        }
        public void RenameLib(object sender)
        {
            UI_StringButton pressedElement = sender as UI_StringButton;
            UI_Categorie<UI_Component> curUIlib = Libraries.ui_elements[0].ui_elements.Find(x => x.cat.ID_Name == pressedElement.parent.ID_Name);

            RenameBox.pos = new Pos(Libraries.pos.X, Libraries.pos.Y, ORIGIN.DEFAULT, ORIGIN.DEFAULT, this);
            RenameBox.size = curUIlib.cat.size;
            RenameBox.value = pressedElement.parent.ID_Name;
            RenameBox.ID_Name = pressedElement.parent.ID_Name;
            RenameBox.GetsUpdated = RenameBox.GetsDrawn = true;
        }

        public void Rename_Finish(object sender)
        {
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibrarys.Find(x => x.name == RenameBox.ID_Name);
            if(RenameBox.value.Length > 0)
                curlib.name = RenameBox.value;
            RenameBox.GetsUpdated = RenameBox.GetsDrawn = false;
            Reload_UI();

        }

        public void DeleteLib(object sender)
        {
            IsChange = true;
            UI_Element lib = sender as UI_Element;
            //Libraries.ui_elements[0].ui_elements.Remove(lib.parent as UI_Categorie<UI_Component>);
            CompLibrary.LibraryWindow_LoadedLibrarys.RemoveAll(x => x.name == lib.parent.ID_Name);
            Reload_UI();
        }
        public void LibFolded(object sender)
        {
            UI_Categorie<UI_Component> curUIlib = sender as UI_Categorie<UI_Component>;
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibrarys.Find(x => x.name == curUIlib.cat.ID_Name);
            curlib.IsFold = curUIlib.IsFold;
        }
        public void EditLib(object sender)
        {
            UI_Component curUIlib = sender as UI_Component;
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibrarys[curUIlib.ID];
            UI_Handler.EditLib.ID_Name = curlib.name;
            UI_Handler.EditLib.GetsUpdated = UI_Handler.EditLib.GetsDrawn = true;
            UI_Handler.EditLib.pos.pos = Game1.mo_states.New.Position + new Point(5,5);
            //currlibID = curUIlib.ID;
        }

        protected override void UpdateSpecific()
        {
            //if(FileHandler.IsUpToDate)
            //{
            //    Libraries.GetsUpdated = AddButton.GetsUpdated = Open.GetsUpdated = SaveAll.GetsUpdated = Reload.GetsUpdated = true;
            //    Libraries.GetsDrawn = AddButton.GetsDrawn = Open.GetsDrawn = SaveAll.GetsDrawn = Reload.GetsDrawn = true;
            //    NeedSavingString.GetsUpdated = NeedSavingString.GetsDrawn = false;
            //}
            //else
            //{
            //    Libraries.GetsUpdated = AddButton.GetsUpdated = Open.GetsUpdated = SaveAll.GetsUpdated = Reload.GetsUpdated = false;
            //    Libraries.GetsDrawn = AddButton.GetsDrawn = Open.GetsDrawn = SaveAll.GetsDrawn = Reload.GetsDrawn = false;
            //    NeedSavingString.GetsUpdated = NeedSavingString.GetsDrawn = true;
            //}

            
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
