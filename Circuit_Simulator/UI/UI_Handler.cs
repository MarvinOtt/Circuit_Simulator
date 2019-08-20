﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator
{
    public class UI_Handler
    {
	    private Button button1;
	    public ContentManager Content;

	    private Texture2D Button_tex;

	    private UI_MultiElement Toolbar;
        private UI_MultiElement ButtonMenu1;
        private UI_MultiElement ButtonMenu2;
        public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

	    public void Initialize()
	    {
		    Button_tex = Content.Load<Texture2D>("UI\\Project Spritemap");


			//Toolbar
			Toolbar = new UI_MultiElement(new Vector2(0, 0));
			Toolbar.Add_UI_Element(new Button(new Vector2(0, 0), new Point(67, 25), new Point(0, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67, 0), new Point(67, 25), new Point(67, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67*2, 0), new Point(67, 25), new Point(67*2, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67*3, 0), new Point(67, 25), new Point(67*3, 0), Button_tex, 1));

            Toolbar.Add_UI_Element(new Button(new Vector2(67*4, 0), new Point(26, 25), new Point(67*4, 0), Button_tex, 1));
            Toolbar.UpdateFunctions.Add(delegate ()
		    {
                Game1.IsSimulating = ((Button)Toolbar.ui_elements[4]).IsActivated;
		    });

            //ButtonMenu1
            ButtonMenu1 = new UI_MultiElement(Toolbar.ui_elements[0].pos);
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[0].pos + new Vector2(0, 25), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[0], Game1.pixel, 1));
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[0].pos + new Vector2(0, 25*2), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[0], Game1.pixel, 1));
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[0].pos + new Vector2(0, 25*3), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[0], Game1.pixel, 1));

            ButtonMenu2 = new UI_MultiElement(Toolbar.ui_elements[1].pos);
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[1].pos + new Vector2(0, 25), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[1], Game1.pixel, 1));
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[1].pos + new Vector2(0, 25*2), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[1], Game1.pixel, 1));
            ButtonMenu1.Add_UI_Element(new Button(Toolbar.ui_elements[1].pos + new Vector2(0, 25*3), new Point(67, 25), new Point(0, 0), Toolbar.ui_elements[1], Game1.pixel, 1));



        }

	    public void Update()
	    {
			Toolbar.Update();
            ButtonMenu1.Update();
            
	    }

	    public void Draw(SpriteBatch spritebatch)
	    {
			Toolbar.Draw(spritebatch);
            ButtonMenu1.Draw(spritebatch);
	    }
    }
}
