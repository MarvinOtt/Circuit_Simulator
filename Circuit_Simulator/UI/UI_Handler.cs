

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	    public ContentManager Content;

	    private Texture2D Button_tex;
        public static bool UI_Element_Pressed;
        public static UI_Element ZaWarudo;      //JoJo Reference
        static int buttonheight = 25;
        static int buttonwidth = 67;
        static int sqarebuttonwidth = 25;
        public static Color main_BG_Col = new Color(new Vector3(0.075f));
        public static Color main_Hover_Col = new Color(new Vector3(0.175f));
        public static Color BackgroundColor = new Color(new Vector3(0.15f));
        public static Color HoverColor = new Color(new Vector3(0.3f));
        public static Color ActivColor = Color.Black;
        public static Color ActivHoverColor = Color.Black;
        public static Color BorderColor = new Color(new Vector3(0.45f));
        public static TexButton_Conf TexButton_baseconf = new TexButton_Conf(1);

        private UI_MultiElement<UI_Element> Toolbar;
        private UI_MultiElement<UI_Element> ButtonMenu_File, ButtonMenu_View, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help;
        private UI_QuickHBElement QuickHotbar;
        UI_MultiElement<UI_Element>[] toolbar_menus;
        private UI_ComponentBox ComponentBox;
        UI_Comp_Cat Cat_Gates, Cat_ShiftRegisters, Cat_Counters, Cat_Decoders, Cat_FlipFlops;
        UI_Component AND, OR, XOR, NAND, NOR, XNOR;
        UI_Component FF_RS, FF_D, FF_JK, FF_T;
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
            SpriteFont catfont = Content.Load<SpriteFont>("UI\\cat_font");

            // CONFIGS
            Button_Conf cat_conf;
            cat_conf = new Button_Conf(Color.White, catfont, 2, BackgroundColor, HoverColor, ActivColor, ActivHoverColor);
            Button_Conf toolbarbuttonconf;
            toolbarbuttonconf = new Button_Conf(Color.White, toolbarfont, 1, BackgroundColor, HoverColor, ActivColor, ActivHoverColor);
            Button_Conf componentconf;
            componentconf = new Button_Conf(Color.White, componentfont, 2, BackgroundColor, HoverColor, ActivColor, ActivHoverColor);

            TexButton_Conf quickbarconf_1 = new TexButton_Conf(1, Color.White * 0.1f);
            TexButton_Conf quickbarconf_2 = new TexButton_Conf(2, Color.White * 0.1f);

            //Toolbar
            Toolbar = new UI_MultiElement<UI_Element>(new Point(0, 0));
            string[] TB_Names = new string[] { "File", "Config", "View", "Tools", "Help" };
            for(int i = 0; i < TB_Names.Length; ++i)
                Toolbar.Add_UI_Elements(new UI_Button(new Point(buttonwidth * i, 0), new Point(buttonwidth, buttonheight), TB_Names[i], toolbarbuttonconf));

            // Initializing Menus for Toolbar
            ButtonMenu_File = new UI_TB_Dropdown(new Point(0, 25));
            string[] FileButton_Names = new string[] { "Save", "Save As", "Open", "Open Recent" };
            for(int i = 0; i < FileButton_Names.Length; ++i)
                ButtonMenu_File.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), FileButton_Names[i]));

            ButtonMenu_View = new UI_TB_Dropdown(Toolbar.ui_elements[1].pos + new Point(0, 25));
            string[] ViewButton_Names = new string[] { "Component Box", "Icon Hotbar", "Test" };
            for (int i = 0; i < ViewButton_Names.Length; ++i)
                ButtonMenu_View.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ViewButton_Names[i]));

            ButtonMenu_Config = new UI_TB_Dropdown(Toolbar.ui_elements[2].pos + new Point(0, 25));
            string[] ConfigButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < ConfigButton_Names.Length; ++i)
                ButtonMenu_Config.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ConfigButton_Names[i]));

            ButtonMenu_Tools = new UI_TB_Dropdown(Toolbar.ui_elements[3].pos + new Point(0, 25));
            string[] ToolsButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < ToolsButton_Names.Length; ++i)
                ButtonMenu_Tools.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), ToolsButton_Names[i]));

            ButtonMenu_Help = new UI_TB_Dropdown(Toolbar.ui_elements[4].pos + new Point(0, 25));
            string[] HelpButton_Names = new string[] { "Test", "Test", "Test" };
            for (int i = 0; i < HelpButton_Names.Length; ++i)
                ButtonMenu_Help.Add_UI_Elements(new Button_Menu(new Point(0, i * 25), new Point(buttonwidth, buttonheight), HelpButton_Names[i]));

            //QuickHotbar
            QuickHotbar = new UI_QuickHBElement(new Point(0, Toolbar.size.Y));
            QuickHotbar.Add_UI_Element(new TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 0, 0), Button_tex, quickbarconf_1));
            QuickHotbar.Add_UI_Element(new TexButton(Point.Zero, new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(sqarebuttonwidth * 1 + 1, 0), Button_tex, quickbarconf_2));

            //Componentbox
            ComponentBox = new UI_ComponentBox(new Point(0, 100), new Point(buttonwidth * 3, 500), "ComponentBox");

            // Sample Components
            AND = new UI_Component("AND", componentconf);
            OR = new UI_Component("OR", componentconf);
            XOR = new UI_Component("XOR", componentconf);
            NAND = new UI_Component("NAND", componentconf);
            NOR = new UI_Component("NOR", componentconf);
            XNOR = new UI_Component("XNOR", componentconf);

            FF_RS = new UI_Component("RS", componentconf);
            FF_JK = new UI_Component("JK", componentconf);
            FF_D = new UI_Component("Data", componentconf);
            FF_T = new UI_Component("Toggle", componentconf);



            //Catagories
            Cat_Gates = new UI_Comp_Cat("Gates", cat_conf);
            Cat_FlipFlops = new UI_Comp_Cat("Flip Flops", cat_conf);

            Cat_Gates.AddComponents(AND, NAND, OR, NOR, XOR, XNOR);
            Cat_FlipFlops.AddComponents(FF_RS, FF_JK, FF_D, FF_T);

            ComponentBox.Add_Categories(Cat_Gates, Cat_FlipFlops);

            InitializeUISettings(spriteBatch);
        }

        //
        public void InitializeUISettings(SpriteBatch spritebatch)
        {
            // Play Button Config
            Toolbar.ui_elements[4].UpdateFunctions.Add(delegate ()
            {
                Game1.IsSimulating = ((UI_Button)Toolbar.ui_elements[4]).IsActivated;
            });

            // Config for opening ComponentBox
            ButtonMenu_View.ui_elements[0].UpdateFunctions.Add(delegate () {
                if (((Button_Menu)ButtonMenu_View.ui_elements[0]).IsActivated)
                    ComponentBox.GetsUpdated = ComponentBox.GetsDrawn = true;
            });

            //Configs for Main Toolbar Buttons
            toolbar_menus = new UI_MultiElement<UI_Element>[] { ButtonMenu_File, ButtonMenu_View, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help };
            for (int i = 0; i < 5; ++i)
            {
                int ii = i;
                toolbar_menus[i].UpdateFunctions.Add(delegate ()
                {
                    UI_Button cur = (UI_Button)Toolbar.ui_elements[ii];
                    toolbar_menus[ii].GetsDrawn = toolbar_menus[ii].GetsUpdated = cur.IsActivated;
                    // Deactivate current active button when something else got pressed
                    bool IsInOther = new Rectangle(cur.absolutpos, cur.size).Contains(Game1.mo_states.New.Position);
                    IsInOther |= new Rectangle(toolbar_menus[ii].absolutpos, toolbar_menus[ii].size).Contains(Game1.mo_states.New.Position);
                    if (cur.IsActivated && Game1.mo_states.IsLeftButtonToggleOn() && !IsInOther)
                        cur.IsActivated = false;
                });
            }
            // DragDraw
            ComponentBox.Catagories.DrawFunctions.Add(delegate ()
            {
                ComponentBox.Catagories.ui_elements.ForEach(cat => cat.Components.ui_elements.ForEach(c =>
                {
                    if (c.IsDrag)
                        spritebatch.DrawString(c.conf.font, c.name, Game1.mo_states.New.Position.ToVector2() + new Vector2(16, 0), c.conf.fontcol);
                }));
            });
        }

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            //Toolbar.pos = new Vector2(0, Game1.Screenheight - 25);
        }

	    public void Update()
	    {
            UI_Element_Pressed = false;

            if(ZaWarudo != null)
            {
                ZaWarudo.Update();
                return;
            }

            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].Update();
            ComponentBox.Update();
            QuickHotbar.Update();
            Toolbar.Update();
            
        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            Toolbar.Draw(spritebatch);
            QuickHotbar.Draw(spritebatch);
            ComponentBox.Draw(spritebatch);
            for (int i = 0; i < toolbar_menus.Length; ++i)
                toolbar_menus[i].Draw(spritebatch);
        }
    }
}
