﻿using System;
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
    public class UI_LibraryEdit_Window : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Categorie<UI_Component>>> Libraries;
        public static Rectangle libhitbox;
        UI_StringButton AddButton, Open, SaveAll;
        UI_ValueInput RenameBox1, RenameBox2;

        public UI_LibraryEdit_Window(Pos pos, Point size, string Title, Point minsize, Generic_Conf conf, bool IsResizeable = true) : base(pos, size, Title, minsize, conf, IsResizeable)
        {
            Libraries = new UI_Scrollable<UI_List<UI_Categorie<UI_Component>>>(new Pos(bezelsize, 50), Point.Zero);
            Libraries.Add_UI_Elements(new UI_List<UI_Categorie<UI_Component>>(Pos.Zero, false));
            AddButton = new UI_StringButton(new Pos(-bezelsize, ORIGIN.BR, ORIGIN.BR), new Point(UI_Handler.buttonwidth, UI_Handler.buttonheight), "Add", true, UI_Handler.genbutconf);
            Open = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, AddButton), new Point((int)(UI_Handler.buttonwidth), UI_Handler.buttonheight), "Open", true, UI_Handler.genbutconf);
            SaveAll = new UI_StringButton(new Pos(-bezelsize, 0, ORIGIN.DEFAULT, ORIGIN.TR, Open), new Point((int)(UI_Handler.buttonwidth * 1.5), UI_Handler.buttonheight), "Save All", true, UI_Handler.genbutconf);
            RenameBox1 = new UI_ValueInput(new Pos(0, 0), Point.Zero, UI_Handler.genbutconf, 3);
            RenameBox2 = new UI_ValueInput(new Pos(0, 0), Point.Zero, UI_Handler.componentconf, 3);


            Add_UI_Elements(RenameBox1, RenameBox2, AddButton, Open, SaveAll);
            Add_UI_Elements(Libraries);

           
            UI_Handler.EditLib.ui_elements[0].GotActivatedLeft += RenameLib;
            UI_Handler.EditLib.ui_elements[1].GotActivatedLeft += AddComp;
            UI_Handler.EditLib.ui_elements[2].GotActivatedLeft += DeleteLib;

            UI_Handler.EditComp.ui_elements[0].GotActivatedLeft += RenameComp;
            UI_Handler.EditComp.ui_elements[1].GotActivatedLeft += DeleteComp;
           
            RenameBox1.ValueChanged += RenameLib_Finish;
            RenameBox2.ValueChanged += RenameComp_Finish;
            Open.GotActivatedLeft += OpenLib;
            AddButton.GotActivatedLeft += NewLib;
            SaveAll.GotActivatedLeft += SaveAllChanges;


        }
       
        protected override void Resize()
        {
            Libraries.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            if (RenameBox1.GetsDrawn)
                RenameBox1.size.X = Libraries.size.X;
            if (RenameBox2.GetsDrawn)
                RenameBox2.size.X = Libraries.size.X;
        }

        public void Add_Library(CompLibrary libs)
        {
            UI_Categorie<UI_Component> newlib = new UI_Categorie<UI_Component>(libs.name, UI_Handler.cat_conf);
            newlib.cat.ID = CompLibrary.LibraryWindow_LoadedLibraries.IndexOf(libs);
            newlib.cat.ID_Name = libs.name;
            newlib.cat.GotActivatedRight += EditLib; 
            newlib.GotFolded += LibFolded;
            newlib.Fold(libs.IsFold);
            for(int i = 0; i < libs.Components.Count; i++)
            {
                int ID = i;
                UI_Component cur_comp = new UI_Component(new Pos(0), new Point(20, 20), libs.Components[i].name, ID, 20, UI_Handler.componentconf);
                cur_comp.ID_Name = libs.name + "|" + libs.Components[i].name;
                cur_comp.Sort_Name = libs.Components[i].catagory + "|" + libs.Components[i].name;
                cur_comp.GotActivatedLeft += EditCompWindow;
                cur_comp.GotActivatedRight += EditComp;

                newlib.AddComponents(cur_comp);
            }
            newlib.SetXSize(size.X - bezelsize * 2);
            Libraries.ui_elements[0].Add_UI_Elements(newlib);
   
        }

        public override void ChangedUpdate2True()
        {
            for(int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
            {
                int index = CompLibrary.LibraryWindow_LoadedLibraries.FindIndex(x => x.name == CompLibrary.AllUsedLibraries[i].name);
                if(index == -1)
                {
                    CompLibrary newlib = new CompLibrary(null, CompLibrary.AllUsedLibraries[i].SaveFile, false);
                    newlib.LoadFromPath();
                }
            }

            Reload_UI();

            base.ChangedUpdate2True();
        }

        public void Reload_UI()
        {
            Libraries.ui_elements[0].ui_elements.Clear();
            for (int i = 0; i < CompLibrary.LibraryWindow_LoadedLibraries.Count; ++i)
            {
                Add_Library(CompLibrary.LibraryWindow_LoadedLibraries[i]);
            }
        }

       
        public void SaveAllChanges( object sender)
        {
            for (int i = 0; i < CompLibrary.LibraryWindow_LoadedLibraries.Count; i++)
                CompLibrary.LibraryWindow_LoadedLibraries[i].Save();
        }
        public void OpenLib(object sender)
        {
            CompLibrary.LoadFrom(false);
            CompLibrary.LibraryWindow_LoadedLibraries.RemoveAll(x => x.STATE == CompLibrary.LOAD_FAILED);
            Reload_UI();
        }
        public void NewLib(object sender)
        {

            string startname = "New Library";
            string finalname = "";
            for(int i = 1; ; ++i)
            {
                bool state = CompLibrary.LibraryWindow_LoadedLibraries.Exists(x => x.name == startname + i.ToString());
                if(!state)
                {
                    finalname = startname + i.ToString();
                    break;
                }

            }
            CompLibrary newLib = new CompLibrary(finalname, null, false);
            Reload_UI();
            Libraries.ui_elements.ForEach(x => { if (x.pos.parent == Libraries) { x.pos.Y -= 1000000; } });
            Libraries.UpdatePos();
            Libraries.UpdateSpecific();
            UpdatePos();
            UI_Component curUIcomp = Libraries.ui_elements[0].ui_elements.Last().cat;

            RenameBox1.pos = new Pos(Libraries.pos.X, (curUIcomp.absolutpos.Y - this.pos.Y), ORIGIN.DEFAULT, ORIGIN.DEFAULT, this);
            RenameBox1.size = curUIcomp.size;
            RenameBox1.value = finalname;
            RenameBox1.ID_Name = finalname;
            RenameBox1.GetsUpdated = RenameBox1.GetsDrawn = true;
            RenameBox1.Set2Typing();
        }

        public void EditLib(object sender)
        {
            UI_Component curUIlib = sender as UI_Component;
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibraries[curUIlib.ID];
            UI_Handler.EditLib.ID_Name = curlib.name;
            UI_Handler.EditLib.GetsUpdated = UI_Handler.EditLib.GetsDrawn = true;
            UI_Handler.EditLib.pos.pos = App.mo_states.New.Position + new Point(5, 5);
            UI_Handler.EditLib.UpdatePos();
        }

        public void AddComp(object sender)
        {
            UI_StringButton pressedElement = sender as UI_StringButton;
            string startname = "New Component";
            string finalname = "";
            for (int y = 1; ; y++)
            {
                bool DoesExist = false;
                for (int i = 0; i < CompLibrary.LibraryWindow_LoadedLibraries.Count; ++i)
                {
                    bool state = CompLibrary.LibraryWindow_LoadedLibraries[i].Components.Exists(x => x.name == startname + y.ToString());
                    if (state)
                        DoesExist = true;
                }
                if (!DoesExist)
                {
                    finalname = startname + y.ToString();
                    break;
                }
            }
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == pressedElement.parent.ID_Name);
            CompData newComp = new CompData(finalname, "Other", false, false);
            curlib.AddComponent(newComp);


            Reload_UI();
            Libraries.ui_elements.ForEach(x => { if (x.pos.parent == Libraries) { x.pos.Y -= 1000000; } });
            Libraries.UpdatePos();
            Libraries.UpdateSpecific();
            UpdatePos();
            UI_Component curUIcomp;
            int libindex_UI = Libraries.ui_elements[0].ui_elements.FindIndex(x => x.cat.ID_Name == pressedElement.parent.ID_Name);

            curUIcomp = Libraries.ui_elements[0].ui_elements[libindex_UI].Components.ui_elements.Last();
            RenameBox2.pos = new Pos(Libraries.pos.X, (curUIcomp.absolutpos.Y - this.pos.Y), ORIGIN.DEFAULT, ORIGIN.DEFAULT, this);
            RenameBox2.size = curUIcomp.size;
            RenameBox2.value = finalname;
            RenameBox2.ID_Name = finalname;
            RenameBox2.GetsUpdated = RenameBox2.GetsDrawn = true;
            RenameBox2.Set2Typing();
            UI_Handler.EditComp.ID_Name = curlib.name + "|" + finalname;
        }
        public void EditCompWindow(object sender)
        {
            UI_Component comp = sender as UI_Component;
            UI_Handler.editcompwindow.GetsUpdated = UI_Handler.editcompwindow.GetsDrawn = true;
            UI_Window.All_Highlight(UI_Handler.editcompwindow);
            string[] names = comp.ID_Name.Split('|');
            CompData compdata = CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == names[0]).Components.Find(x => x.name == names[1]);
            UI_Handler.editcompwindow.SetRootComp(compdata);

        }
        
        public void RenameLib(object sender)
        {
            UI_StringButton pressedElement = sender as UI_StringButton;
            UI_Categorie<UI_Component> curUIlib= Libraries.ui_elements[0].ui_elements.Find(x => x.cat.ID_Name == pressedElement.pos.parent.ID_Name);

            RenameBox1.pos = new Pos(Libraries.pos.X, (curUIlib.absolutpos.Y - this.pos.Y), ORIGIN.DEFAULT, ORIGIN.DEFAULT, this);
            RenameBox1.size = curUIlib.cat.size;
            RenameBox1.value = pressedElement.pos.parent.ID_Name;
            RenameBox1.ID_Name = pressedElement.pos.parent.ID_Name;
            RenameBox1.GetsUpdated = RenameBox1.GetsDrawn = true;
            RenameBox1.Set2Typing();
        }

        public void RenameLib_Finish(object sender)
        {
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == RenameBox1.ID_Name);
            if (Libraries.ui_elements[0].ui_elements.Exists(x => x.cat.ID_Name == RenameBox1.value) && curlib.name != RenameBox1.value)
            {
                RenameBox1.IsTyping = true;
                return;
            }
            if(RenameBox1.value.Length > 0)
                curlib.name = RenameBox1.value;
            RenameBox1.GetsUpdated = RenameBox1.GetsDrawn = false;
            Reload_UI();

        }

        public void DeleteLib(object sender)
        {
            UI_Element lib = sender as UI_Element;
            CompLibrary.LibraryWindow_LoadedLibraries.RemoveAll(x => x.name == lib.parent.ID_Name);
            Reload_UI();
        }
        public void LibFolded(object sender)
        {
            UI_Categorie<UI_Component> curUIlib = sender as UI_Categorie<UI_Component>;
            CompLibrary curlib = CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == curUIlib.cat.ID_Name);
            curlib.IsFold = curUIlib.IsFold;
        }
       

        public void EditComp(object sender)
        {
            UI_Component curUIcomp = sender as UI_Component;
            UI_Handler.EditComp.ID_Name = curUIcomp.ID_Name;
            UI_Handler.EditComp.GetsUpdated = UI_Handler.EditComp.GetsDrawn = true;
            UI_Handler.EditComp.pos.pos = App.mo_states.New.Position + new Point(5, 5);
        }

        public void RenameComp(object sender)
        {
            UI_StringButton pressedElement = sender as UI_StringButton;
            string[] names = pressedElement.parent.ID_Name.Split('|');
            UI_Component curUIcomp = Libraries.ui_elements[0].ui_elements.Find(x => x.cat.ID_Name == names[0]).Components.ui_elements.Find(x => x.ID_Name == pressedElement.parent.ID_Name);

            RenameBox2.pos = new Pos(Libraries.pos.X, (curUIcomp.absolutpos.Y - this.pos.Y), ORIGIN.DEFAULT, ORIGIN.DEFAULT, this);
            RenameBox2.size = curUIcomp.size;
            RenameBox2.value = names[1];
            RenameBox2.ID_Name = pressedElement.pos.parent.ID_Name;
            RenameBox2.GetsUpdated = RenameBox2.GetsDrawn = true;
            RenameBox2.Set2Typing();
        }

        public void RenameComp_Finish(object sender)
        {
            string[] names = UI_Handler.EditComp.ID_Name.Split('|');
            CompData comp = CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == names[0]).Components.Find(x => x.name == names[1]);
            if (RenameBox2.value.Length > 0)
                comp.name = RenameBox2.value;
            RenameBox2.GetsUpdated = RenameBox2.GetsDrawn = false;
            Reload_UI();

        }

        public void DeleteComp(object sender)
        {
            UI_StringButton comp = sender as UI_StringButton;
            string[] names = comp.parent.ID_Name.Split('|');
            CompLibrary.LibraryWindow_LoadedLibraries.Find(x => x.name == names[0]).Components.RemoveAll(x => x.name == names[1]);
            Reload_UI();
   
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
