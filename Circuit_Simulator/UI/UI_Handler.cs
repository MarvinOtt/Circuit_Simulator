using System;
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

	    public UI_Handler(ContentManager Content)
	    {
		    this.Content = Content;
	    }

	    public void Initialize()
	    {
		    Button_tex = Content.Load<Texture2D>("UI\\Project Spritemap");

			//Toolbar
			Toolbar = new UI_MultiElement(new Vector2(500, 0));
			Toolbar.Add_UI_Element(new Button(new Vector2(0, 0), new Point(67, 25), new Point(0, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67, 0), new Point(67, 25), new Point(67, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67*2, 0), new Point(67, 25), new Point(67*2, 0), Button_tex, 1));
		    Toolbar.Add_UI_Element(new Button(new Vector2(67*3, 0), new Point(67, 25), new Point(67*3, 0), Button_tex, 1));
			Toolbar.UpdateFunctions.Add(delegate ()
		    {
			    foreach (var element in Toolbar.ui_elements)
			    {
				    if (((Button) element).IsActivated)
				    {
					    int breaki = 2;
				    }
			    }
		    });
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
