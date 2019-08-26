using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Circuit_Simulator
{
    public class UI_Handler
    {
	    private Button button1;
	    public ContentManager Content;

	    private Texture2D Button_tex;

	    private UI_MultiElement Toolbar;
        private UI_MultiElement ButtonMenu_File, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help;
        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

        public void Initialize()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
            Button_tex = Content.Load<Texture2D>("UI\\Project Spritemap");


            //Toolbar
            Toolbar = new UI_MultiElement(new Point(50, 50));
            Toolbar.Add_UI_Element(new Button(new Point(0, 0), new Point(67, 25), new Point(0, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(67, 0), new Point(67, 25), new Point(67, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(67 * 2, 0), new Point(67, 25), new Point(67 * 2, 0), Button_tex, 1));
            Toolbar.Add_UI_Element(new Button(new Point(67 * 3, 0), new Point(67, 25), new Point(67 * 3, 0), Button_tex, 1));

            Toolbar.Add_UI_Element(new Button(new Point(67 * 4, 0), new Point(25, 25), new Point(67 * 4, 0), Button_tex, 1));

            //ButtonMenu1
            ButtonMenu_File = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_File.parent = Toolbar.ui_elements[0];
            Toolbar.ui_elements[0].child = ButtonMenu_File;
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(67, 25), 2));
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(67, 25), 2));
            ButtonMenu_File.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(67, 25), 2));

            ButtonMenu_Config = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Config.parent = Toolbar.ui_elements[1];
            Toolbar.ui_elements[1].child = ButtonMenu_Config;
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(67, 25), 2));
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(67, 25), 2));
            ButtonMenu_Config.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(67, 25), 2));

            ButtonMenu_Tools = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Tools.parent = Toolbar.ui_elements[2];
            Toolbar.ui_elements[2].child = ButtonMenu_Tools;
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(67, 25), 2));
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(67, 25), 2));
            ButtonMenu_Tools.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(67, 25), 2));

            ButtonMenu_Help = new UI_MultiElement(new Point(0, 25));
            ButtonMenu_Help.parent = Toolbar.ui_elements[3];
            Toolbar.ui_elements[3].child = ButtonMenu_Help;
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 0), new Point(67, 25), 2));
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 25), new Point(67, 25), 2));
            ButtonMenu_Help.Add_UI_Element(new Button_Menu(new Point(0, 25 * 2), new Point(67, 25), 2));


            Toolbar.ui_elements[4].UpdateFunctions.Add(delegate ()
            {
                Game1.IsSimulating = ((Button)Toolbar.ui_elements[4]).IsActivated;
            });

            //Configs for Main Toolbar Buttons
            //UI_MultiElement[] toolbar_menus = new UI_MultiElement[] { ButtonMenu_File, ButtonMenu_Config, ButtonMenu_Tools, ButtonMenu_Help };
            //for(int i = 0; i < 3; ++i)
            //{
            //    Toolbar.ui_elements[i].UpdateFunctions.Add(delegate ()
            //    {
            //        Button cur = (Button)Toolbar.ui_elements[i];
            //        toolbar_menus[i].GetsDrawn = toolbar_menus[i].GetsUpdated = cur.IsActivated;
            //        if (cur.GotActivated)
            //            DisableAllOtherToolbarButtons(i);
            //    });
            //}
            Rectangle[] values = new Rectangle[] { Rectangle.Empty, Rectangle.Empty , Rectangle.Empty , Rectangle.Empty , Rectangle.Empty };
            int sum = values.Sum(x => x.Size.X);

            Toolbar.ui_elements[0].UpdateFunctions.Add(delegate ()
            {
                Button cur = (Button)Toolbar.ui_elements[0];
                ButtonMenu_File.GetsDrawn = ButtonMenu_File.GetsUpdated = cur.IsActivated;
                if (cur.GotActivated)
                    DisableAllOtherToolbarButtons(0);
                if (cur.IsActivated && Game1.mo_states.IsLeftButtonToggleOn() && !(new Rectangle(cur.absolutpos, new Point(cur.size.X, cur.size.Y + ((UI_MultiElement)cur.child).ui_elements.Sum(x => ((Button_Menu)x).size.Y))).Contains(Game1.mo_states.New.Position)))
                    cur.IsActivated = false;
            });
            Toolbar.ui_elements[1].UpdateFunctions.Add(delegate ()
            {
                Button cur = (Button)Toolbar.ui_elements[1];
                ButtonMenu_Config.GetsDrawn = ButtonMenu_Config.GetsUpdated = cur.IsActivated;
                if (cur.GotActivated)
                    DisableAllOtherToolbarButtons(1);
            });
            Toolbar.ui_elements[2].UpdateFunctions.Add(delegate ()
            {
                Button cur = (Button)Toolbar.ui_elements[2];
                ButtonMenu_Tools.GetsDrawn = ButtonMenu_Tools.GetsUpdated = cur.IsActivated;
                if (cur.GotActivated)
                    DisableAllOtherToolbarButtons(2);
            });
            Toolbar.ui_elements[3].UpdateFunctions.Add(delegate ()
            {
                Button cur = (Button)Toolbar.ui_elements[3];
                ButtonMenu_Help.GetsDrawn = ButtonMenu_Help.GetsUpdated = cur.IsActivated;
                if (cur.GotActivated)
                    DisableAllOtherToolbarButtons(3);
            });
            ButtonMenu_File.ui_elements[0].UpdateFunctions.Add(delegate ()
            {
                if (((Button_Menu)ButtonMenu_File.ui_elements[0]).IsActivated)
                {
                    int breaki = 3;
                }
            });
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
			Toolbar.Update();
        }

	    public void Draw(SpriteBatch spritebatch)
	    {
			Toolbar.Draw(spritebatch);
	    }
    }
}
