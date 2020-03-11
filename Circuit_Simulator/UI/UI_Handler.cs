

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.COMP;
using Circuit_Simulator.UI;
using Circuit_Simulator.UI.Specific;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator
{
    public class UI_Handler
    {
        public const int UI_Active_Main = 1;
        public const int UI_Active_CompDrag = 2;

        public ContentManager Content;
        public static UI_Element ZaWarudo;  //JoJo Reference
	    public static Texture2D Button_tex;
        public static bool IsInScrollable = false;
        public static Rectangle IsInScrollable_Bounds;
        public static bool UI_AlreadyActivated, UI_IsWindowHide;
        public static int UI_Active_State;
        public static UI_Drag_Comp dragcomp = new UI_Drag_Comp();
        public static int buttonheight = 25;
        public static int buttonwidth = 67;
        public static int sqarebuttonwidth = 25;
        public static Color main_BG_Col = new Color(new Vector3(0.15f));
        public static Color main_Hover_Col = Color.White * 0.1f;
        public static Color BackgroundColor = new Color(new Vector3(0.15f));
        public static Color HoverColor = new Color(new Vector3(0.3f));
        public static Color ActivColor = Color.White * 0.2f;
        public static Color ActivHoverColor = Color.Black;
        public static Color BorderColor = new Color(new Vector3(0.45f));
        public static Color[] layer_colors;
        private UI_MultiElement<UI_Element> Toolbar;
        private UI_MultiElement<UI_Element> ButtonMenu_File, ButtonMenu_View, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help;
        public static UI_InfoBox info, GridInfo;
        public UI_Window input;
        public static UI_ParameterWindow parameterWindow;
        public static UI_LibraryEdit_Window LibraryEditWindow;
        public static UI_ProjectLibrary_Window ProjectLibWindow;
        public static UI_EditComp_Window editcompwindow;
        public static UI_Box<UI_Element> GeneralInfoBox;
        public static UI_Box<UI_StringButton> EditLib, EditProjectLib;
        public static UI_Box<UI_StringButton> EditComp;
        public static UI_QuickHBElement<UI_Element> QuickHotbar;
        public static UI_QuickHBElement<UI_Element> LayerSelectHotbar;
        public static UI_QuickHBElement<UI_TexButton> WireMaskHotbar;
        public static UI_Window SignalAnalyze;
        public static UI_SignalAnalyze signal;
        UI_Element[] toolbar_menus;
        public static UI_ComponentBox ComponentBox;
        public static UI_List<UI_TexButton> wire_ddbl;
        public static Generic_Conf componentconf,genbutconf, gen_conf;
        public static Generic_Conf cat_conf, toolbarbuttonconf, toolbarddconf1, toolbarddconf2, behave1conf, behave2conf, gridpaintbuttonconf;

        public static UI_ValueInput netbox;
        public static int netboxval = 70;

        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

        public void Initialize(SpriteBatch spriteBatch)
        {
            App.GraphicsChanged += Window_Graphics_Changed;
            Button_tex = Content.Load<Texture2D>("UI\\Project Spritemap");
            SpriteFont toolbarfont = Content.Load<SpriteFont>("UI\\TB_font");
            SpriteFont componentfont = Content.Load<SpriteFont>("UI\\component_font");
            SpriteFont dropdownfont = Content.Load<SpriteFont>("UI\\TB_Dropdown_font");
            SpriteFont catfont = Content.Load<SpriteFont>("UI\\cat_font");

            // CONFIGS
            gen_conf = new Generic_Conf(font_color: Color.White, behav: 2, BGColor: BackgroundColor, HoverColor: HoverColor, ActiveColor: ActivColor, ActiveHoverColor: ActivHoverColor, tex_color: Color.White, font: componentfont);
            cat_conf = new Generic_Conf(gen_conf);
            cat_conf.font = catfont;
            toolbarbuttonconf = new Generic_Conf(gen_conf);
            toolbarbuttonconf.font = toolbarfont;
            toolbarbuttonconf.behav = 1;
            toolbarddconf1 = new Generic_Conf(gen_conf);
            toolbarddconf1.font = dropdownfont;
            toolbarddconf1.behav = 1;
            toolbarddconf2 = new Generic_Conf(toolbarddconf1);
            toolbarddconf2.behav = 2;
            componentconf = new Generic_Conf(gen_conf);
            componentconf.font = componentfont;
            behave1conf = new Generic_Conf(gen_conf);
            behave1conf.behav = 1;
            behave2conf = new Generic_Conf(gen_conf);
            behave2conf.behav = 2;
            genbutconf = new Generic_Conf(gen_conf);
            genbutconf.BorderColor = BorderColor;
            genbutconf.font = toolbarfont;
            gridpaintbuttonconf = new Generic_Conf(behave1conf);
            gridpaintbuttonconf.BorderColor = BorderColor;

            //Toolbar
            Toolbar = new UI_MultiElement<UI_Element>(new Pos(0, 0));
            string[] TB_Names = new string[] { "File", "Config", "View", "Tools", "Help" };
            for(int i = 0; i < TB_Names.Length; ++i)
                Toolbar.Add_UI_Elements(new UI_StringButton(new Pos(buttonwidth * i, 0), new Point(buttonwidth, buttonheight), TB_Names[i], false, toolbarbuttonconf));

            // Initializing Menus for Toolbar
            ButtonMenu_File = new UI_TB_Dropdown(new Pos(0, 25));
            string[] FileButton_Names = new string[] { "Save", "Save As", "Open"};
            for(int i = 0; i < FileButton_Names.Length; ++i)
                ButtonMenu_File.Add_UI_Elements(new UI_Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), FileButton_Names[i], toolbarddconf2));

            ButtonMenu_Config = new UI_TB_Dropdown(Toolbar.ui_elements[1].pos + new Pos(0, 25));
            string[] ConfigButton_Names = new string[] { "Tiny Grid", "Small Grid", "Medium Grid", "Big Grid", "Large Grid" };
            for (int i = 0; i < ConfigButton_Names.Length; ++i)
                ButtonMenu_Config.Add_UI_Elements(new UI_Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ConfigButton_Names[i], toolbarddconf2));

            ButtonMenu_View = new UI_TB_Dropdown(Toolbar.ui_elements[2].pos + new Pos(0, 25));
            string[] ViewButton_Names = new string[] { "Component Box", "Icon Hotbar", "Layer Hotbar" };
            for (int i = 0; i < ViewButton_Names.Length; ++i)
            {
                ButtonMenu_View.Add_UI_Elements(new UI_Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ViewButton_Names[i], toolbarddconf1));
            }
            ButtonMenu_Tools = new UI_TB_Dropdown(Toolbar.ui_elements[3].pos + new Pos(0, 25));
            string[] ToolsButton_Names = new string[] { "Libary Editor", "Project Libararies"};
            for (int i = 0; i < ToolsButton_Names.Length; ++i)
                ButtonMenu_Tools.Add_UI_Elements(new UI_Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ToolsButton_Names[i], toolbarddconf1));

            ButtonMenu_Help = new UI_TB_Dropdown(Toolbar.ui_elements[4].pos + new Pos(0, 25));
            string[] HelpButton_Names = new string[] { "Developer Email: a.schoenhofer.business@gmail.com", "Developer Email: marvinott20@gmail.com", "User Guide in Application Folder" };
            for (int i = 0; i < HelpButton_Names.Length; ++i)
                ButtonMenu_Help.Add_UI_Elements(new UI_Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), HelpButton_Names[i], toolbarddconf2));

            //QuickHotbar
            QuickHotbar = new UI_QuickHBElement<UI_Element>(new Pos(0, Toolbar.size.Y));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 0, 0), Button_tex, behave1conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 1 + 1, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 2 + 2, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 3 + 3, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 4 + 4, 0), Button_tex, behave1conf));         
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 5 + 5, 0), Button_tex, behave1conf));

            //Componentbox
            ComponentBox = new UI_ComponentBox(new Pos(0, 100), new Point(buttonwidth * 3, 500), "Component Box", new Point(180, 180), componentconf, true);



            //Hover Info Box
            info = new UI_InfoBox(new Pos(500,500), new Point(300, 300));
            info.values.Add_UI_Elements(new UI_String(new Pos(0, 0), new Point(0, 0), componentconf));

            //Hover GridInfo Box 
            GridInfo = new UI_InfoBox(new Pos(500, 500), new Point(300, 300));
            GridInfo.values.Add_UI_Elements(new UI_String(new Pos(0, 0), new Point(0, 0), componentconf));

            //GeneralInfo Box (Bottom Left)
            GeneralInfoBox = new UI_Box<UI_Element>(new Pos(-1, App.Screenheight - 24 + 1), new Point(App.Screenwidth + 2, 24));
            GeneralInfoBox.Add_UI_Elements(new UI_String(new Pos( 10, 2), Point.Zero, componentconf));
            GeneralInfoBox.Add_UI_Elements(new UI_StringButton(new Pos(150, 0), new Point(24, 24), "+", true, behave2conf));
            GeneralInfoBox.Add_UI_Elements(new UI_StringButton(new Pos(0, 0, ORIGIN.TR, ORIGIN.DEFAULT, GeneralInfoBox.ui_elements[1]), new Point(24, 24), "-", true, behave2conf));
            GeneralInfoBox.Add_UI_Elements(new UI_String(new Pos(0, 2, ORIGIN.TR, ORIGIN.DEFAULT, GeneralInfoBox.ui_elements[2]), Point.Zero, componentconf));

            //Assigning Colors to the layers
            Color[] all_layer_colors = new Color[7] { Color.Red, Color.Lime, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan, new Color(1, 0.5f, 0) };
            layer_colors = new Color[7];
            for (int i = 0; i < 7; i++)
            {
                layer_colors[i] = all_layer_colors[i];
            }


            //Layer Select Hotbar
            LayerSelectHotbar = new UI_QuickHBElement<UI_Element>(new Pos(0, 0, ORIGIN.DEFAULT, ORIGIN.BL, GeneralInfoBox));

            for(int i = 0; i < 7; i++)
            {
                Generic_Conf curconf = new Generic_Conf(behave1conf);
                curconf.tex_color = layer_colors[i];
                LayerSelectHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(buttonwidth, buttonheight), new Point(0, sqarebuttonwidth * 4 + 3), Button_tex, curconf));
            }
            (LayerSelectHotbar.ui_elements[0] as UI_TexButton).IsActivated = true;

            //WireMaskHotbar
            WireMaskHotbar = new UI_QuickHBElement<UI_TexButton>(new Pos(100, 0, ORIGIN.TR, ORIGIN.DEFAULT, LayerSelectHotbar));
           



            for (int i = 0; i < 7; i++)
            {
                Generic_Conf curconf = new Generic_Conf(behave1conf);
                curconf.tex_color = layer_colors[i];
                WireMaskHotbar.Add_UI_Element(new UI_TexButton(new Pos(0, 0), new Point(sqarebuttonwidth), new Point(sqarebuttonwidth * (i + 7) + i + 7, 0), Button_tex, curconf));
            }
            WireMaskHotbar.Add_UI_Element(new UI_TexButton(new Pos(0, 0), new Point(sqarebuttonwidth), new Point(sqarebuttonwidth * (7 + 7) + 7 + 7, 0), Button_tex, behave1conf));
            WireMaskHotbar.Add_UI_Element(new UI_TexButton(new Pos(0, 0), new Point(sqarebuttonwidth), new Point(sqarebuttonwidth * (7 + 1 + 7) + 7 + 1 + 7, 0), Button_tex, behave1conf));

            //EditLib options
            EditLib = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), (int)(buttonheight * 3)));
            UI_StringButton RenameLib = new UI_StringButton(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), buttonheight), "Rename", true, componentconf);
            UI_StringButton NewComp = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, RenameLib), new Point((int)(App.Screenwidth * 0.08), buttonheight), "New Component", true, componentconf);
            UI_StringButton Dellib = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, NewComp), new Point((int)(App.Screenwidth * 0.08), buttonheight), "Remove Library", true, componentconf);
            
            EditLib.Add_UI_Elements(RenameLib, NewComp, Dellib);
            EditLib.GetsUpdated = EditLib.GetsDrawn = false;

            //EditComp options
            EditComp = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), (int)(buttonheight * 2)));
            UI_StringButton RenameComp = new UI_StringButton(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), buttonheight), "Rename", true, componentconf);
            UI_StringButton DelComp = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, RenameComp), new Point((int)(App.Screenwidth * 0.08), buttonheight), "Delete Component", true, componentconf);
            EditComp.Add_UI_Elements(RenameComp, DelComp);
            EditComp.GetsUpdated = EditComp.GetsDrawn = false;

            //Libary Edit Window
            LibraryEditWindow = new UI_LibraryEdit_Window(new Pos(App.Screenwidth / 2, App.Screenheight / 2),  new Point(500, 500),"Libaries", new Point(400, 200), componentconf, true);
            LibraryEditWindow.GetsUpdated = LibraryEditWindow.GetsDrawn = false;


            //EditCompWindow
            editcompwindow = new UI_EditComp_Window(new Pos(App.Screenwidth / 3, App.Screenheight / 3), new Point((int)(App.Screenwidth * 0.5), (int)(App.Screenheight * 0.6)), "EditComponent", new Point(300, 300), componentconf, true);

            //Project Lib options
            EditProjectLib = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), (int)(buttonheight)));
            UI_StringButton RemoveLib = new UI_StringButton(new Pos(0, 0), new Point((int)(App.Screenwidth * 0.08), buttonheight), "Remove Library", true, componentconf);
            EditProjectLib.Add_UI_Elements(RemoveLib);
            EditProjectLib.GetsUpdated = EditProjectLib.GetsDrawn = false;

            //ProjectLibraryWindow
            ProjectLibWindow = new UI_ProjectLibrary_Window(new Pos(200), new Point(500, 500), "Project Libraries", new Point(300), componentconf, true);
            ProjectLibWindow.GetsUpdated = ProjectLibWindow.GetsDrawn = false;

            //ComponentparametersWindow
            parameterWindow = new UI_ParameterWindow(new Pos(400, 250), new Point(300, 250), new Point(300, 100), componentconf);
            parameterWindow.GetsUpdated = parameterWindow.GetsDrawn = false;

            //SignalAnalyze
            SignalAnalyze = new UI_Window(new Pos(App.Screenwidth / 2, App.Screenheight / 2), new Point(800, 100), "Analyze", new Point(200, 80), componentconf, true);
            signal = new UI_SignalAnalyze(new Pos(-5, UI_Window.headheight + 5, ORIGIN.TR, ORIGIN.TR, SignalAnalyze), new Point(500, 80));
            SignalAnalyze.Add_UI_Elements(signal);
            SignalAnalyze.GetsUpdated = SignalAnalyze.GetsDrawn = false;
            InitializeUISettings(spriteBatch);
            
        }

        public void netbox_ValueChange(object sender)
        {
            netboxval = int.Parse(netbox.value);
        }

        public static void InitComponents()
        {
          
            HashSet<string> categorys = new HashSet<string>();
            List<CompData> comps = Sim_Component.Components_Data;
            for (int i = 0; i < comps.Count; ++i)
            {
                categorys.Add(comps[i].catagory);
            }
            UI_Categorie<UI_Component>[] comp_cats = new UI_Categorie<UI_Component>[categorys.Count];
            for(int i = 0; i < comp_cats.Length; ++i)
            {
                comp_cats[i] = new UI_Categorie<UI_Component>(categorys.ElementAt(i), cat_conf);
            }
            List<string> categorys_list = categorys.ToList();
            for(int i = 0; i < comps.Count; ++i)
            {

                comp_cats[categorys_list.IndexOf(comps[i].catagory)].AddComponents(new UI_Component(new Pos(0), new Point(0, 20), comps[i].name, i, 20, componentconf));
            }
            ComponentBox.Catagories.ui_elements[0].ui_elements.Clear();
            ComponentBox.Add_Categories(comp_cats);
        }

        public void InitializeUISettings(SpriteBatch spritebatch)
        {
            // Play Button Config
            QuickHotbar.ui_elements[0].UpdateFunctions.Add(delegate ()
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[0]).IsActivated != Simulator.IsSimulating)
                {
                    App.simulator.SetSimulationState(((UI_TexButton)QuickHotbar.ui_elements[0]).IsActivated);
                }
            });

            //Config Button Configs
            ((UI_Button_Menu)ButtonMenu_Config.ui_elements[0]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.ProjectSizeX = 2048;
                Simulator.ProjectSizeY = 2048;
            };

            ((UI_Button_Menu)ButtonMenu_Config.ui_elements[1]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.ProjectSizeX = 4096;
                Simulator.ProjectSizeY = 4096; 
            };
            ((UI_Button_Menu)ButtonMenu_Config.ui_elements[2]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.ProjectSizeX = 6144;
                Simulator.ProjectSizeY = 6144;
            };
            ((UI_Button_Menu)ButtonMenu_Config.ui_elements[3]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.ProjectSizeX = 10240;
                Simulator.ProjectSizeY = 10240;
            };
            ((UI_Button_Menu)ButtonMenu_Config.ui_elements[4]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.ProjectSizeX = 16384;
                Simulator.ProjectSizeY = 16384;
            };

            //Reset Button Config
            ((UI_TexButton)QuickHotbar.ui_elements[1]).GotActivatedLeft += delegate (object sender)
            {
                Simulator.cursimframe = 0;
                ((UI_TexButton)QuickHotbar.ui_elements[0]).IsActivated = false;
                Simulator.IsSimulating = false;
            };

            // ComponentBox UI Toggle
            ButtonMenu_View.ui_elements[0].UpdateFunctions.Add(delegate () 
            {
                UI_Button_Menu current = (UI_Button_Menu)ButtonMenu_View.ui_elements[0];
                if (current.IsToggle)
                    ComponentBox.GetsUpdated = ComponentBox.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = ComponentBox.GetsUpdated;
            });
            // Library Window UI Toggle
            ButtonMenu_Tools.ui_elements[0].UpdateFunctions.Add(delegate ()
            {
                UI_Button_Menu current = (UI_Button_Menu)ButtonMenu_Tools.ui_elements[0];
                if (current.IsToggle)
                    LibraryEditWindow.GetsUpdated = LibraryEditWindow.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = LibraryEditWindow.GetsUpdated;
            });
            ButtonMenu_Tools.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                UI_Button_Menu current = (UI_Button_Menu)ButtonMenu_Tools.ui_elements[1];
                if (current.IsToggle)
                    ProjectLibWindow.GetsUpdated = ProjectLibWindow.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = ProjectLibWindow.GetsUpdated;
            });
            // QuickHotbar UI Toggle
            ButtonMenu_View.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                UI_Button_Menu current = (UI_Button_Menu)ButtonMenu_View.ui_elements[1];
                if (current.IsToggle)
                    QuickHotbar.GetsUpdated = QuickHotbar.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = QuickHotbar.GetsUpdated;
            });
            // Layer Select UI Toggle
            ButtonMenu_View.ui_elements[2].UpdateFunctions.Add(delegate ()
            {
                UI_Button_Menu current = (UI_Button_Menu)ButtonMenu_View.ui_elements[2];
                if (current.IsToggle)
                    LayerSelectHotbar.GetsUpdated = LayerSelectHotbar.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = LayerSelectHotbar.GetsUpdated;
            });

            //Configs for Main Toolbar Buttons
            toolbar_menus = new UI_Element[] { ButtonMenu_File, ButtonMenu_Config, ButtonMenu_View, ButtonMenu_Tools, ButtonMenu_Help};
           
            for (int i = 0; i < 5; ++i)
            {
                int ii = i;
                toolbar_menus[i].UpdateFunctions.Add(delegate ()
                {
                    UI_Button cur = (UI_Button)Toolbar.ui_elements[ii];
                    toolbar_menus[ii].GetsDrawn = toolbar_menus[ii].GetsUpdated = cur.IsActivated;

                    // Deactivate current active button when something else got pressed
                    bool IsInOther =  new Rectangle(cur.absolutpos, cur.size).Contains(App.mo_states.New.Position);
                    IsInOther |= new Rectangle(toolbar_menus[ii].absolutpos, toolbar_menus[ii].size).Contains(App.mo_states.New.Position);
                    if (cur.IsActivated && (App.mo_states.IsLeftButtonToggleOff() || App.mo_states.IsLeftButtonToggleOn()) && !IsInOther)
                        cur.IsActivated = false;
                });
            }

            EditLib.UpdateFunctions.Add(delegate ()
            {
                if(App.mo_states.IsLeftButtonToggleOff())
                {
                    
                    EditLib.GetsUpdated = EditLib.GetsDrawn = false;
                }
            
            });
            EditProjectLib.UpdateFunctions.Add(delegate ()
            {
                if (App.mo_states.IsLeftButtonToggleOff())
                {

                    EditProjectLib.GetsUpdated = EditProjectLib.GetsDrawn = false;
                }

            });
            EditComp.UpdateFunctions.Add(delegate ()
            {
                if (App.mo_states.IsLeftButtonToggleOff())
                {

                    EditComp.GetsUpdated = EditComp.GetsDrawn = false;
                }

            });
            //UI Layer Select Hotbar change
            for(int i = 0; i < LayerSelectHotbar.ui_elements.Count; ++i)
            {
                (LayerSelectHotbar.ui_elements[i] as UI_TexButton).GotToggledLeft += LayerHotBarButton_Pressed;
            }
            for (int i = 0; i < WireMaskHotbar.ui_elements.Count; ++i)
            {
                (WireMaskHotbar.ui_elements[i]).GotToggledLeft += WireMaskBar_Pressed;
                (WireMaskHotbar.ui_elements[i]).GetsHovered += WireMaskBar_Hovered;
            }



            //Changin Simspeed int the UI
            (GeneralInfoBox.ui_elements[1] as UI_StringButton).GotToggledLeft += inreaseSimSpeed;
            (GeneralInfoBox.ui_elements[2] as UI_StringButton).GotToggledLeft += decreaseSimSpeed;


            ((UI_TexButton)QuickHotbar.ui_elements[5]).GotActivatedLeft += delegate (object sender)
            {
                ((UI_TexButton)QuickHotbar.ui_elements[5]).IsActivated = true;
                App.simulator.ChangeToolmode(Simulator.TOOL_WIRE);
                ((UI_TexButton)QuickHotbar.ui_elements[4]).IsActivated = false;
            };

            ((UI_TexButton)QuickHotbar.ui_elements[4]).GotToggledLeft += delegate (object sender)
            {
                ((UI_TexButton)QuickHotbar.ui_elements[4]).IsActivated = true;
                App.simulator.ChangeToolmode(Simulator.TOOL_SELECT);
                ((UI_TexButton)QuickHotbar.ui_elements[5]).IsActivated = false;
            };

            ((UI_TexButton)QuickHotbar.ui_elements[2]).GotActivatedLeft += delegate (object sender)
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[2]).IsActivated)
                {
                    FileHandler.Save();
                }
            };

            ((UI_TexButton)QuickHotbar.ui_elements[3]).GotActivatedLeft += delegate (object sender)
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[3]).IsActivated)
                {
                    FileHandler.Open();
                }
            };
        }
        public void inreaseSimSpeed(object sender)
        {
            Simulator.simspeed ++;
        }
        public void decreaseSimSpeed(object sender)
        {
            Simulator.simspeed --;
        }
        public void LayerHotBarButton_Pressed(object sender)
        {
            UI_TexButton cur = sender as UI_TexButton;
            Simulator.currentlayer = LayerSelectHotbar.ui_elements.IndexOf(cur);
            Simulator.sim_effect.Parameters["currentlayer"].SetValue(Simulator.currentlayer);
            if (cur.IsActivated == false)
                cur.IsActivated = true;
            else
            {
                for (int i = 0; i < LayerSelectHotbar.ui_elements.Count; ++i)
                {
                    UI_TexButton curbut = (LayerSelectHotbar.ui_elements[i] as UI_TexButton);
                    if (curbut != cur)
                        curbut.IsActivated = false;
                }
            }
        }
        public void WireMaskBar_Pressed(object sender)
        {
            UI_TexButton cur = sender as UI_TexButton;
            if (cur.IsActivated == false)
            {
                if (App.kb_states.New.IsKeyUp(Keys.LeftShift))
                    cur.IsActivated = true;
            }

            if (App.kb_states.New.IsKeyUp(Keys.LeftShift))
            {
                for (int i = 0; i < WireMaskHotbar.ui_elements.Count; ++i)
                {
                    UI_TexButton curbut = (WireMaskHotbar.ui_elements[i] as UI_TexButton);
                    if (curbut != cur)
                        curbut.IsActivated = false;
                }
            }

        }

        public void WireMaskBar_Hovered(object sender)
        {
            UI_TexButton cur = sender as UI_TexButton;
            if ((App.mo_states.New.LeftButton == ButtonState.Pressed || App.mo_states.IsLeftButtonToggleOff()) && App.kb_states.New.IsKeyDown(Keys.LeftShift))
                cur.IsActivated = true;
        }

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            GeneralInfoBox.pos.pos = new Point(-1,  App.Screenheight - 25 + 1);
            GeneralInfoBox.size = new Point(App.Screenwidth + 2, 25);
        }

	    public void Update()
	    {
            
            UI_AlreadyActivated = false;
            UI_Active_State = 0;
            if (ZaWarudo != null)
            {
                ZaWarudo.UpdateMain();
            }


            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].UpdateMain();
            EditLib.UpdateMain();
            EditProjectLib.UpdateMain();
            EditComp.UpdateMain();
            UI_Window.All_Update();
            GridInfo.UpdateMain();
            info.UpdateMain();
            QuickHotbar.UpdateMain();
            Toolbar.UpdateMain();
            GeneralInfoBox.UpdateMain();
            LayerSelectHotbar.UpdateMain();
            WireMaskHotbar.UpdateMain();



        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            WireMaskHotbar.Draw(spritebatch);
            LayerSelectHotbar.Draw(spritebatch);
            GeneralInfoBox.Draw(spritebatch);
            Toolbar.Draw(spritebatch);
            QuickHotbar.Draw(spritebatch);
            UI_Window.All_Draw(spritebatch);
            info.Draw(spritebatch);
            GridInfo.Draw(spritebatch);
            EditComp.Draw(spritebatch);
            EditProjectLib.Draw(spritebatch);
            EditLib.Draw(spritebatch);

            
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].Draw(spritebatch);


            dragcomp.Draw(spritebatch);

        }
    }
}
