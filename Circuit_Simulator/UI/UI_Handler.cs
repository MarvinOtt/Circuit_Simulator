﻿

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

namespace Circuit_Simulator
{
    public class UI_Handler
    {
        public const int UI_Active_Main = 1;
        public const int UI_Active_CompDrag = 2;

        public ContentManager Content;
        public static UI_Element ZaWarudo;  //JoJo Reference
	    private Texture2D Button_tex;
        public static bool UI_Element_Pressed, UI_IsWindowHide;
        public static int UI_Active_State;
        public static UI_Drag_Comp dragcomp = new UI_Drag_Comp();
        static int buttonheight = 25;
        static int buttonwidth = 67;
        static int sqarebuttonwidth = 25;
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
        public UI_Window LibaryWindow;
        public static UI_Box<UI_String> GeneralInfoBox;
        public static UI_QuickHBElement QuickHotbar;
        UI_Element[] toolbar_menus;
        public static UI_ComponentBox ComponentBox;
        public static UI_List<UI_Dropdown_Button> wire_ddbl;
        public static Generic_Conf componentconf, gen_conf;
        public static Generic_Conf cat_conf, toolbarbuttonconf, toolbarddconf1, toolbarddconf2, behave1conf, behave2conf;

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
            gen_conf = new Generic_Conf(font_color: Color.White, behav: 2, BGColor: BackgroundColor, HoverColor: HoverColor, ActiveColor: ActivColor, ActiveHoverColor: ActivHoverColor, tex_color: Color.White);
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

            //Toolbar
            Toolbar = new UI_MultiElement<UI_Element>(new Point(0, 0));
            string[] TB_Names = new string[] { "File", "Config", "View", "Tools", "Help" };
            for(int i = 0; i < TB_Names.Length; ++i)
                Toolbar.Add_UI_Elements(new UI_StringButton(new Point(buttonwidth * i, 0), new Point(buttonwidth, buttonheight), TB_Names[i], toolbarbuttonconf));

            // Initializing Menus for Toolbar
            ButtonMenu_File = new UI_TB_Dropdown(new Point(0, 25));
            string[] FileButton_Names = new string[] { "Save", "Save As", "Open", "Open Recent" };
            for(int i = 0; i < FileButton_Names.Length; ++i)
                ButtonMenu_File.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), FileButton_Names[i], toolbarddconf2));

            ButtonMenu_Config = new UI_TB_Dropdown(Toolbar.ui_elements[1].pos + new Point(0, 25));
            string[] ConfigButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < ConfigButton_Names.Length; ++i)
                ButtonMenu_Config.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ConfigButton_Names[i], toolbarddconf2));

            ButtonMenu_View = new UI_TB_Dropdown(Toolbar.ui_elements[2].pos + new Point(0, 25));
            string[] ViewButton_Names = new string[] { "Component Box", "Icon Hotbar", "Test" };
            for (int i = 0; i < ViewButton_Names.Length; ++i)
            {
                ButtonMenu_View.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ViewButton_Names[i], toolbarddconf1));
                //((Button_Menu)ButtonMenu_View.ui_elements[i]).conf.behav = 2;
            }
            ButtonMenu_Tools = new UI_TB_Dropdown(Toolbar.ui_elements[3].pos + new Point(0, 25));
            string[] ToolsButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < ToolsButton_Names.Length; ++i)
                ButtonMenu_Tools.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ToolsButton_Names[i], toolbarddconf2));

            ButtonMenu_Help = new UI_TB_Dropdown(Toolbar.ui_elements[4].pos + new Point(0, 25));
            string[] HelpButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < HelpButton_Names.Length; ++i)
                ButtonMenu_Help.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), HelpButton_Names[i], toolbarddconf2));

            //QuickHotbar
            QuickHotbar = new UI_QuickHBElement(new Point(0, Toolbar.size.Y));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 0, 0), Button_tex, behave1conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 1 + 1, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 2 + 2, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 3 + 3, 0), Button_tex, behave2conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 4 + 4, 0), Button_tex, behave1conf));         
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 5 + 5, 0), Button_tex, behave1conf));
            QuickHotbar.Add_UI_Element(new UI_TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 6 + 6, 0), Button_tex, behave1conf));

            Color[] all_layer_colors = new Color[7] { Color.Red, Color.Lime, Color.Blue, Color.Yellow, Color.Magenta, Color.Cyan, new Color(1, 0.5f, 0) };
            layer_colors = new Color[Simulator.LAYER_NUM];
            for(int i = 0; i < Simulator.LAYER_NUM; i++)
            {
                layer_colors[i] = all_layer_colors[i];
            }
            wire_ddbl = new UI_List<UI_Dropdown_Button>(new Point(0, sqarebuttonwidth), false); 
            QuickHotbar.ui_elements[6].child = wire_ddbl;
            wire_ddbl.parent = QuickHotbar.ui_elements[6];
            wire_ddbl.GetsUpdated = wire_ddbl.GetsDrawn = false;
            
            for(int i = 0; i < Simulator.LAYER_NUM; i++)
            {
                Generic_Conf curconf = new Generic_Conf(gen_conf);
                curconf.tex_color = layer_colors[i];
                wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Point(0,0),new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (i+7) + i+7, 0), Button_tex, curconf));
            }
            wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Point(0, 0), new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (Simulator.LAYER_NUM + 7) + Simulator.LAYER_NUM + 7, 0), Button_tex, gen_conf));
            wire_ddbl.Add_UI_Elements(new UI_Dropdown_Button(new Point(0, 0), new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * (Simulator.LAYER_NUM + 1 + 7) + Simulator.LAYER_NUM + 1 + 7, 0), Button_tex, gen_conf));

            //Componentbox
            ComponentBox = new UI_ComponentBox(new Point(0, 100), new Point(buttonwidth * 3, 500), "Component Box", new Point(120, 20), componentconf, true);



            //Wire Info Box
            info = new UI_InfoBox(new Point(500,500), new Point(300, 300));
            info.values.Add_UI_Elements(new UI_String(new Point(0, 0), new Point(0, 0), componentconf));

            //input Box
            input = new UI_Window(new Point(Game1.Screenwidth / 2, Game1.Screenheight / 2), new Point((int)(Game1.Screenwidth * 0.2), (int)(Game1.Screenheight * 0.10)), "Value", new Point((int)(Game1.Screenwidth * 0.2), (int)(Game1.Screenheight * 0.1)), componentconf, false);
            input.Add_UI_Elements(new UI_ValueInput(new Point(input.size.X / 2 - input.size.X / 4, 20 + input.size.Y / 2 - input.size.Y / 4), new Point(input.size.X / 2, input.size.Y / 2 -20 -1), componentconf, 1));
            input.GetsDrawn = input.GetsUpdated = false;
            InitializeUISettings(spriteBatch);

            //GeneralInfo Box
            GeneralInfoBox = new UI_Box<UI_String>(new Point(-1, Game1.Screenheight - 25 + 1), new Point(Game1.Screenwidth + 2, 25));
            GeneralInfoBox.Add_UI_Elements(new UI_String(Point.Zero, Point.Zero, componentconf));
            GeneralInfoBox.Add_UI_Elements(new UI_String(new Point(150, 0), Point.Zero, componentconf));

            //Libary Window
            LibaryWindow = new UI_Window(new Point(Game1.Screenwidth / 2, Game1.Screenheight / 2),  new Point(200, 500),"Libaries", new Point(200, 500), componentconf, true);

        }

        public static void InitComponents4CompBox()
        {
            HashSet<string> categorys = new HashSet<string>();
            List<CompData> comps = Sim_Component.Components_Data;
            for (int i = 0; i < comps.Count; ++i)
            {
                categorys.Add(comps[i].catagory);
            }
            UI_Comp_Cat[] comp_cats = new UI_Comp_Cat[categorys.Count];
            for(int i = 0; i < comp_cats.Length; ++i)
            {
                comp_cats[i] = new UI_Comp_Cat(categorys.ElementAt(i), cat_conf);
            }
            List<string> categorys_list = categorys.ToList();
            for(int i = 0; i < comps.Count; ++i)
            {
                comp_cats[categorys_list.IndexOf(comps[i].catagory)].AddComponents(new UI_Component(comps[i].name, i, componentconf));
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

            // Config for opening ComponentBox
            ButtonMenu_View.ui_elements[0].UpdateFunctions.Add(delegate () 
            {
                Button_Menu current = (Button_Menu)ButtonMenu_View.ui_elements[0];
                if (current.IsToggle)
                    ComponentBox.GetsUpdated = ComponentBox.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = ComponentBox.GetsUpdated;
            });
            ButtonMenu_View.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                Button_Menu current = (Button_Menu)ButtonMenu_View.ui_elements[1];
                if (current.IsToggle)
                    QuickHotbar.GetsUpdated = QuickHotbar.GetsDrawn = current.IsActivated;
                else
                    current.IsActivated = QuickHotbar.GetsUpdated;
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
                    if (cur.IsActivated && Game1.mo_states.IsLeftButtonToggleOff() && !IsInOther)
                        cur.IsActivated = false;
                });
            }
           

            // Wire MaskButton
            QuickHotbar.ui_elements[6].UpdateFunctions.Add(delegate ()
            {
                UI_TexButton current = (UI_TexButton)QuickHotbar.ui_elements[6];
                current.child.GetsUpdated = current.child.GetsDrawn = current.IsActivated;
            });
            ((UI_TexButton)QuickHotbar.ui_elements[5]).GotActivated += delegate ()
            {
                ((UI_TexButton)QuickHotbar.ui_elements[4]).IsActivated = false;
            };
            ((UI_TexButton)QuickHotbar.ui_elements[4]).GotActivated += delegate ()
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

            QuickHotbar.ui_elements[2].UpdateFunctions.Add(delegate ()
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[2]).IsActivated)
                {
                    FileHandler.Save();
                }
            });

            QuickHotbar.ui_elements[3].UpdateFunctions.Add(delegate ()
            {
                if (((UI_TexButton)QuickHotbar.ui_elements[3]).IsActivated)
                {
                    FileHandler.Open();
                }
            });
        }

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            GeneralInfoBox.pos = new Point(-1,  Game1.Screenheight - 25 + 1);
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

            input.UpdateMain();
            LibaryWindow.UpdateMain();
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].UpdateMain();
            wire_ddbl.UpdateMain();
            info.UpdateMain();
            ComponentBox.UpdateMain();
            QuickHotbar.UpdateMain();
            Toolbar.UpdateMain();
            GeneralInfoBox.UpdateMain();



        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            GeneralInfoBox.Draw(spritebatch);
            Toolbar.Draw(spritebatch);
            QuickHotbar.Draw(spritebatch);
            ComponentBox.Draw(spritebatch);
            info.Draw(spritebatch);
            wire_ddbl.Draw(spritebatch);
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].Draw(spritebatch);
            LibaryWindow.Draw(spritebatch);
            input.Draw(spritebatch);
            dragcomp.Draw(spritebatch);
        }
    }
}
