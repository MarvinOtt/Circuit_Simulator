

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
        public static bool UI_Element_Pressed, UI_IsWindowHide;
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
        public static UI_InfoBox info;
        public UI_Window input;
        public static UI_LibraryEdit_Window LibraryEditWindow;
        public static UI_ProjectLibrary_Window ProjectLibWindow;
        public static UI_EditComp_Window editcompwindow;
        public static UI_Box<UI_Element> GeneralInfoBox;
        public static UI_Box<UI_StringButton> EditLib, EditProjectLib;
        public static UI_Box<UI_StringButton> EditComp;
        public static UI_QuickHBElement QuickHotbar;
        public static UI_QuickHBElement LayerSelectHotbar;
        UI_Element[] toolbar_menus;
        public static UI_ComponentBox ComponentBox;
        public static UI_List<UI_Dropdown_Button> wire_ddbl;
        public static Generic_Conf componentconf,genbutconf, gen_conf;
        public static Generic_Conf cat_conf, toolbarbuttonconf, toolbarddconf1, toolbarddconf2, behave1conf, behave2conf, gridpaintbuttonconf;

        public static UI_ValueInput netbox;
        public static int netboxval = 70;

        UI_GridPaint gridpaint;

        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

        public void Initialize(SpriteBatch spriteBatch)
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
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
            //gridpaint = new UI_GridPaint(new Pos(100), new Point(200), 200, new Point(25));

            //Toolbar
            Toolbar = new UI_MultiElement<UI_Element>(new Pos(0, 0));
            string[] TB_Names = new string[] { "File", "Config", "View", "Tools", "Help" };
            for(int i = 0; i < TB_Names.Length; ++i)
                Toolbar.Add_UI_Elements(new UI_StringButton(new Pos(buttonwidth * i, 0), new Point(buttonwidth, buttonheight), TB_Names[i], false, toolbarbuttonconf));

            // Initializing Menus for Toolbar
            ButtonMenu_File = new UI_TB_Dropdown(new Pos(0, 25));
            string[] FileButton_Names = new string[] { "Save", "Save As", "Open", "Open Recent" };
            for(int i = 0; i < FileButton_Names.Length; ++i)
                ButtonMenu_File.Add_UI_Elements(new Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), FileButton_Names[i], toolbarddconf2));

            ButtonMenu_Config = new UI_TB_Dropdown(Toolbar.ui_elements[1].pos + new Pos(0, 25));
            string[] ConfigButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < ConfigButton_Names.Length; ++i)
                ButtonMenu_Config.Add_UI_Elements(new Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ConfigButton_Names[i], toolbarddconf2));

            ButtonMenu_View = new UI_TB_Dropdown(Toolbar.ui_elements[2].pos + new Pos(0, 25));
            string[] ViewButton_Names = new string[] { "Component Box", "Icon Hotbar", "Layer Hotbar" };
            for (int i = 0; i < ViewButton_Names.Length; ++i)
            {
                ButtonMenu_View.Add_UI_Elements(new Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ViewButton_Names[i], toolbarddconf1));
                //((Button_Menu)ButtonMenu_View.ui_elements[i]).conf.behav = 2;
            }
            ButtonMenu_Tools = new UI_TB_Dropdown(Toolbar.ui_elements[3].pos + new Pos(0, 25));
            string[] ToolsButton_Names = new string[] { "Libary Editor", "Project Libararies", "Test" };
            for (int i = 0; i < ToolsButton_Names.Length; ++i)
                ButtonMenu_Tools.Add_UI_Elements(new Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), ToolsButton_Names[i], toolbarddconf1));

            ButtonMenu_Help = new UI_TB_Dropdown(Toolbar.ui_elements[4].pos + new Pos(0, 25));
            string[] HelpButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < HelpButton_Names.Length; ++i)
                ButtonMenu_Help.Add_UI_Elements(new Button_Menu(new Pos(0, i * 25), new Point(buttonwidth, buttonheight), HelpButton_Names[i], toolbarddconf2));

            //QuickHotbar
            QuickHotbar = new UI_QuickHBElement(new Pos(0, Toolbar.size.Y));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 0, 0), Button_tex, behave1conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 1 + 1, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 2 + 2, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 3 + 3, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 4 + 4, 0), Button_tex, behave1conf));         
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 5 + 5, 0), Button_tex, behave1conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 6 + 6, 0), Button_tex, behave1conf));

            //Assigning Colors to the layers
            Color[] all_layer_colors = new Color[7] { Color.Red, Color.Lime, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan, new Color(1, 0.5f, 0) };
            layer_colors = new Color[7];
            for(int i = 0; i < 7; i++)
            {
                layer_colors[i] = all_layer_colors[i];
            }
            wire_ddbl = new UI_List<UI_Dropdown_Button>(new Pos(0, sqarebuttonwidth), false); 
            QuickHotbar.ui_elements[6].child = wire_ddbl;
            wire_ddbl.parent = QuickHotbar.ui_elements[6];
            wire_ddbl.GetsUpdated = wire_ddbl.GetsDrawn = false;
            
            for(int i = 0; i < 7; i++)
            {
                Generic_Conf curconf = new Generic_Conf(behave1conf);
                curconf.tex_color = layer_colors[i];
                wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Pos(0,0),new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (i + 7) + i + 7, 0), Button_tex, curconf));
            }
            wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Pos(0, 0), new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (7 + 7) + 7 + 7, 0), Button_tex, behave1conf));
            wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Pos(0, 0), new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (7 + 1 + 7) + 7 + 1 + 7, 0), Button_tex, behave1conf));

            //Componentbox
            ComponentBox = new UI_ComponentBox(new Pos(0, 100), new Point(buttonwidth * 3, 500), "Component Box", new Point(180, 180), componentconf, true);



            //Hover Info Box
            info = new UI_InfoBox(new Pos(500,500), new Point(300, 300));
            info.values.Add_UI_Elements(new UI_String(new Pos(0, 0), new Point(0, 0), componentconf));


            //input Box
            //input = new UI_Window(new Point(Game1.Screenwidth / 2, Game1.Screenheight / 2), new Point((int)(Game1.Screenwidth * 0.2), (int)(Game1.Screenheight * 0.10)), "Value", new Point((int)(Game1.Screenwidth * 0.2), (int)(Game1.Screenheight * 0.1)), componentconf, false);
            //input.Add_UI_Elements(new UI_ValueInput(new Point(input.size.X / 2 - input.size.X / 4, 20 + input.size.Y / 2 - input.size.Y / 4), new Point(input.size.X / 2, input.size.Y / 2 -20 -1), componentconf, 1));
            //input.GetsDrawn = input.GetsUpdated = false;

            //GeneralInfo Box (Bottom Left)
            GeneralInfoBox = new UI_Box<UI_Element>(new Pos(-1, Game1.Screenheight - 24 + 1), new Point(Game1.Screenwidth + 2, 24));
            GeneralInfoBox.Add_UI_Elements(new UI_String(new Pos( 10, 2), Point.Zero, componentconf));
            GeneralInfoBox.Add_UI_Elements(new UI_StringButton(new Pos(150, 0), new Point(24, 24), "+", true, behave2conf));
            GeneralInfoBox.Add_UI_Elements(new UI_StringButton(new Pos(0, 0, ORIGIN.TR, ORIGIN.DEFAULT, GeneralInfoBox.ui_elements[1]), new Point(24, 24), "-", true, behave2conf));
            GeneralInfoBox.Add_UI_Elements(new UI_String(new Pos(0, 2, ORIGIN.TR, ORIGIN.DEFAULT, GeneralInfoBox.ui_elements[2]), Point.Zero, componentconf));
            //Layer Select Hotbar
            LayerSelectHotbar = new UI_QuickHBElement(new Pos(0, 0, ORIGIN.DEFAULT, ORIGIN.BL, GeneralInfoBox));

            for(int i = 0; i < 7; i++)
            {
                Generic_Conf curconf = new Generic_Conf(behave1conf);
                curconf.tex_color = layer_colors[i];
                LayerSelectHotbar.Add_UI_Element(new UI_TexButton(Pos.Zero, new Point(buttonwidth, buttonheight), new Point(0, sqarebuttonwidth * 4 + 3), Button_tex, curconf));
            }
            (LayerSelectHotbar.ui_elements[0] as UI_TexButton).IsActivated = true;
            
            //EditLib options
            EditLib = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), (int)(buttonheight * 3)));
            UI_StringButton RenameLib = new UI_StringButton(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "Rename", true, componentconf);
            UI_StringButton NewComp = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, RenameLib), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "New Component", true, componentconf);
            UI_StringButton Dellib = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, NewComp), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "Delete Library", true, componentconf);
            
            EditLib.Add_UI_Elements(RenameLib, NewComp, Dellib);
            EditLib.GetsUpdated = EditLib.GetsDrawn = false;

            //EditComp options
            EditComp = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), (int)(buttonheight * 2)));
            UI_StringButton RenameComp = new UI_StringButton(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "Rename", true, componentconf);
            UI_StringButton DelComp = new UI_StringButton(new Pos(0, 0, ORIGIN.BL, ORIGIN.DEFAULT, RenameComp), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "Delete Component", true, componentconf);
            EditComp.Add_UI_Elements(RenameComp, DelComp);
            EditComp.GetsUpdated = EditComp.GetsDrawn = false;

            //Libary Edit Window
            LibraryEditWindow = new UI_LibraryEdit_Window(new Pos(Game1.Screenwidth / 2, Game1.Screenheight / 2),  new Point(500, 500),"Libaries", new Point(400, 200), componentconf, true);
            LibraryEditWindow.GetsUpdated = LibraryEditWindow.GetsDrawn = false;
            //LibaryWindow.Add_UI_Elements(new UI_StringButton(new Point(2, LibaryWindow.Libaries.size.Y), new Point(buttonwidth, buttonheight), "test", toolbarbuttonconf));


            //EditCompWindow
            editcompwindow = new UI_EditComp_Window(new Pos(Game1.Screenwidth / 3, Game1.Screenheight / 3), new Point((int)(Game1.Screenwidth * 0.5), (int)(Game1.Screenheight * 0.6)), "EditComponent", new Point(300, 300), componentconf, true);

            //Project Lib options
            EditProjectLib = new UI_Box<UI_StringButton>(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), (int)(buttonheight)));
            UI_StringButton RemoveLib = new UI_StringButton(new Pos(0, 0), new Point((int)(Game1.Screenwidth * 0.08), buttonheight), "Remove Library", true, componentconf);
            EditProjectLib.Add_UI_Elements(RemoveLib);
            EditProjectLib.GetsUpdated = EditProjectLib.GetsDrawn = false;

            //ProjectLibraryWindow
            ProjectLibWindow = new UI_ProjectLibrary_Window(new Pos(200), new Point(500, 500), "Project Libraries", new Point(300), componentconf, true);
            ProjectLibWindow.GetsUpdated = ProjectLibWindow.GetsDrawn = false;

            //netbox = new UI_ValueInput(new Pos(200), new Point(200, 40), gen_conf, 1);
            //netbox.ValueChanged += netbox_ValueChange;
            
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
                if(((UI_TexButton)QuickHotbar.ui_elements[0]).IsActivated != Simulator.IsSimulating)
                    Game1.simulator.SetSimulationState(((UI_TexButton)QuickHotbar.ui_elements[0]).IsActivated);
            });

            // ComponentBox UI Toggle
            ButtonMenu_View.ui_elements[0].UpdateFunctions.Add(delegate () 
            {
                Button_Menu current = (Button_Menu)ButtonMenu_View.ui_elements[0];
                if (current.IsToggle)
                    ComponentBox.GetsUpdated = ComponentBox.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = ComponentBox.GetsUpdated;
            });
            // Library Window UI Toggle
            ButtonMenu_Tools.ui_elements[0].UpdateFunctions.Add(delegate ()
            {
                Button_Menu current = (Button_Menu)ButtonMenu_Tools.ui_elements[0];
                if (current.IsToggle)
                    LibraryEditWindow.GetsUpdated = LibraryEditWindow.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = LibraryEditWindow.GetsUpdated;
            });
            ButtonMenu_Tools.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                Button_Menu current = (Button_Menu)ButtonMenu_Tools.ui_elements[1];
                if (current.IsToggle)
                    ProjectLibWindow.GetsUpdated = ProjectLibWindow.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = ProjectLibWindow.GetsUpdated;
            });
            // QuickHotbar UI Toggle
            ButtonMenu_View.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                Button_Menu current = (Button_Menu)ButtonMenu_View.ui_elements[1];
                if (current.IsToggle)
                    QuickHotbar.GetsUpdated = QuickHotbar.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = QuickHotbar.GetsUpdated;
            });
            // Layer Select UI Toggle
            ButtonMenu_View.ui_elements[2].UpdateFunctions.Add(delegate ()
            {
                Button_Menu current = (Button_Menu)ButtonMenu_View.ui_elements[2];
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
                    bool IsInOther =  new Rectangle(cur.absolutpos, cur.size).Contains(Game1.mo_states.New.Position);
                    IsInOther |= new Rectangle(toolbar_menus[ii].absolutpos, toolbar_menus[ii].size).Contains(Game1.mo_states.New.Position);
                    if (cur.IsActivated && (Game1.mo_states.IsLeftButtonToggleOff() || Game1.mo_states.IsLeftButtonToggleOn()) && !IsInOther)
                        cur.IsActivated = false;
                });
            }

            EditLib.UpdateFunctions.Add(delegate ()
            {
                if(Game1.mo_states.IsLeftButtonToggleOff())
                {
                    
                    EditLib.GetsUpdated = EditLib.GetsDrawn = false;
                }
            
            });
            EditProjectLib.UpdateFunctions.Add(delegate ()
            {
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {

                    EditProjectLib.GetsUpdated = EditProjectLib.GetsDrawn = false;
                }

            });
            EditComp.UpdateFunctions.Add(delegate ()
            {
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {

                    EditComp.GetsUpdated = EditComp.GetsDrawn = false;
                }

            });

            for(int i = 0; i < LayerSelectHotbar.ui_elements.Count; ++i)
            {
                (LayerSelectHotbar.ui_elements[i] as UI_TexButton).GotToggledLeft += LayerHotBarButton_Pressed;
            }

            //Changin Simspeed int the UI
            (GeneralInfoBox.ui_elements[1] as UI_StringButton).GotToggledLeft += inreaseSimSpeed;
            (GeneralInfoBox.ui_elements[2] as UI_StringButton).GotToggledLeft += decreaseSimSpeed;

            // Wire MaskButton
            QuickHotbar.ui_elements[6].UpdateFunctions.Add(delegate ()
            {
                UI_TexButton current = (UI_TexButton)QuickHotbar.ui_elements[6];
                current.child.GetsUpdated = current.child.GetsDrawn = current.IsActivated;
            });
            ((UI_TexButton)QuickHotbar.ui_elements[5]).GotActivatedLeft += delegate (object sender)
            {
                ((UI_TexButton)QuickHotbar.ui_elements[4]).IsActivated = false;
            };
            ((UI_TexButton)QuickHotbar.ui_elements[4]).GotActivatedLeft += delegate (object sender)
            {
                ((UI_TexButton)QuickHotbar.ui_elements[5]).IsActivated = false;
            };

            QuickHotbar.ui_elements[5].UpdateFunctions.Add(delegate ()
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[5]).IsActivated)
                    Game1.simulator.ChangeToolmode(Simulator.TOOL_WIRE);
            });

            QuickHotbar.ui_elements[4].UpdateFunctions.Add(delegate ()
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[4]).IsActivated)
                    Game1.simulator.ChangeToolmode(Simulator.TOOL_SELECT);
            });

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

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            GeneralInfoBox.pos.pos = new Point(-1,  Game1.Screenheight - 25 + 1);
            GeneralInfoBox.size = new Point(Game1.Screenwidth + 2, 25);
        }

	    public void Update()
	    {
            
            UI_Element_Pressed = false;
            UI_Active_State = 0;
            if (ZaWarudo != null)
            {
                ZaWarudo.UpdateMain();
                //return;
            }

            //netbox.UpdateMain();
            //textbox.UpdateMain();
            //gridpaint.UpdateMain();

            //input.UpdateMain();
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].UpdateMain();
            EditLib.UpdateMain();
            EditProjectLib.UpdateMain();
            EditComp.UpdateMain();
            wire_ddbl.UpdateMain();
            UI_Window.All_Update();
            info.UpdateMain();
            QuickHotbar.UpdateMain();
            Toolbar.UpdateMain();
            GeneralInfoBox.UpdateMain();
            LayerSelectHotbar.UpdateMain();



        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            LayerSelectHotbar.Draw(spritebatch);
            GeneralInfoBox.Draw(spritebatch);
            Toolbar.Draw(spritebatch);
            QuickHotbar.Draw(spritebatch);
            UI_Window.All_Draw(spritebatch);
            info.Draw(spritebatch);
            wire_ddbl.Draw(spritebatch);
            EditComp.Draw(spritebatch);
            EditProjectLib.Draw(spritebatch);
            EditLib.Draw(spritebatch);

            
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].Draw(spritebatch);

            //netbox.Draw(spritebatch);
            //gridpaint.Draw(spritebatch);
            //textbox.Draw(spritebatch);
            //input.Draw(spritebatch);
            dragcomp.Draw(spritebatch);

        }
    }
}
