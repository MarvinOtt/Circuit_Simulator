

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;



namespace Circuit_Simulator
{
    public class UI_Handler
    {
	    public ContentManager Content;

	    private Texture2D Button_tex;
        public static bool UI_Element_Pressed;
        static int buttonheight = 25;
        static int buttonwidth = 67;
        static int sqarebuttonwidth = 25;

	    private UI_MultiElement Toolbar;
        private UI_MultiElement ButtonMenu_File, ButtonMenu_View, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help;
        private UI_MultiElement ComponentBox;
        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

        public void Initialize()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
            Button_tex = Content.Load<Texture2D>("UI\\Project Spritemap");



            //Toolbar
            Toolbar = new UI_MultiElement(new Point(0, 0));
            Toolbar.size = new Point(buttonwidth * 4, buttonheight);
            Toolbar.Add_UI_Element(new Button(new Point(0, 0), new Point(buttonwidth, buttonheight), new Point(0, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(buttonwidth * 1, 0), new Point(buttonwidth, buttonheight), new Point(318, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(buttonwidth * 2, 0), new Point(buttonwidth, buttonheight), new Point(buttonwidth, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(buttonwidth * 3, 0), new Point(buttonwidth, buttonheight), new Point(buttonwidth * 2, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(buttonwidth * 4, 0), new Point(buttonwidth, buttonheight), new Point(buttonwidth * 3, 0), Button_tex, 1));

            Toolbar.Add_UI_Element(new Button(new Point(buttonwidth * 5, 0), new Point(sqarebuttonwidth, sqarebuttonwidth), new Point(buttonwidth * 4, 0), Button_tex, 1));

            //ButtonMenu1
            ButtonMenu_File = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_File.parent = Toolbar.ui_elements[0];
            Toolbar.ui_elements[0].child = ButtonMenu_File;
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(buttonwidth, buttonheight), 2));

            ButtonMenu_View = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_View.parent = Toolbar.ui_elements[1];
            Toolbar.ui_elements[1].child = ButtonMenu_View;
            ButtonMenu_View.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_View.ui_elements[0].UpdateFunctions.Add(delegate (){
                if (((Button_Menu)ButtonMenu_View.ui_elements[0]).IsActivated) ComponentBox.GetsUpdated = ComponentBox.GetsDrawn = true;
            });
            ButtonMenu_View.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_View.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(buttonwidth, buttonheight), 2));

            ButtonMenu_Config = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Config.parent = Toolbar.ui_elements[2];
            Toolbar.ui_elements[2].child = ButtonMenu_Config;
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(buttonwidth, buttonheight), 2));

            ButtonMenu_Tools = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Tools.parent = Toolbar.ui_elements[3];
            Toolbar.ui_elements[3].child = ButtonMenu_Tools;
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(buttonwidth, buttonheight), 2));

            ButtonMenu_Help = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Help.parent = Toolbar.ui_elements[4];
            Toolbar.ui_elements[4].child = ButtonMenu_Help;
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(buttonwidth, buttonheight), 2));
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(buttonwidth, buttonheight), 2));

            //Componentbox
            ComponentBox = new UI_Window(new Point(0, 25), new Point(buttonwidth * 3, 500), "ComponentBox");

            // Play Button Config
            Toolbar.ui_elements[4].UpdateFunctions.Add(delegate ()
            {
                Game1.IsSimulating = ((Button)Toolbar.ui_elements[4]).IsActivated;
            });

            //Configs for Main Toolbar Buttons
            UI_MultiElement[] toolbar_menus = new UI_MultiElement[] { ButtonMenu_File, ButtonMenu_View, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help };
            for (int i = 0; i < 5; ++i)
            {
                int ii = i;
                Toolbar.ui_elements[i].UpdateFunctions.Add(delegate ()
                {
                    Button cur = (Button)Toolbar.ui_elements[ii];
                    toolbar_menus[ii].GetsDrawn = toolbar_menus[ii].GetsUpdated = cur.IsActivated;
                    // Deactivate current active button when something else got pressed
                    if (cur.IsActivated && Game1.mo_states.IsLeftButtonToggleOn() && !(new Rectangle(cur.absolutpos, new Point(cur.size.X, cur.size.Y + ((UI_MultiElement)cur.child).ui_elements.Sum(x => ((Button_Menu)x).size.Y))).Contains(Game1.mo_states.New.Position)))
                        cur.IsActivated = false;
                });
            }
            


        }

        public void DisableAllOtherToolbarButtons(int id)
        {
            for(int i = 0; i < 4; ++i)
            {
                if(i != id)
                {
                    ((Button)Toolbar.ui_elements[i]).IsActivated = false;
                }
            }
        }

        // Gets called when something of the Window or Graphics got changed
        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            //Toolbar.pos = new Vector2(0, Game1.Screenheight - 25);
        }

	    public void Update()
	    {
            UI_Element_Pressed = false;
            Toolbar.Update();
            ComponentBox.Update();
        }

	    public void Draw(SpriteBatch spritebatch)
	    {
            ComponentBox.Draw(spritebatch);
            Toolbar.Draw(spritebatch);
        }
    }
}
