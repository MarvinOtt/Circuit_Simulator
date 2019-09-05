using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    class UI_Window : UI_MultiElement
    {
        static Color BackgroundColor = new Color(new Vector3(0.15f));
        static Color BorderColor = new Color(new Vector3(0.45f));
        string Title;
        static Texture2D tex;
        static int headheight = 20;
        bool IsGrab;
        Point Grabpos;

        public UI_Window(Point pos, Point size, string Title) : base(pos, size)
        {
            this.Title = Title;
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("UI\\Window_SM");
            Add_UI_Element(new Button(new Point(-18, 2), new Point(16), new Point(0), tex, 2)); //X Button
           
            
          
        }




        protected override void UpdateSpecific()
        {
            if(((Button)ui_elements[0]).IsActivated)
                GetsUpdated = GetsDrawn = false;
            // Handling Draging of Window
            Rectangle Grabbox = new Rectangle(absolutpos, new Point(size.X - ui_elements[0].size.X - 4, ui_elements[0].size.Y + 4));
            if (Grabbox.Contains(Game1.mo_states.New.Position) && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsGrab = true;
                Grabpos = Game1.mo_states.New.Position - pos;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            }
            if (IsGrab && Game1.mo_states.IsLeftButtonToggleOff())
            {
                IsGrab = false;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            if(IsGrab)
            {
                pos = Game1.mo_states.New.Position - Grabpos;
            }
            if (pos.X < 0)
                pos.X = 0;
            if (pos.X + size.X > Game1.Screenwidth)
                pos.X = Game1.Screenwidth - size.X;
            if (pos.Y < 0)
                pos.Y = 0;
            if (pos.Y + headheight > Game1.Screenheight)
                pos.Y = Game1.Screenheight - headheight;
            absolutpos = pos;


            base.UpdateSpecific();
        }
        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, new Point(size.X, ui_elements[0].size.Y + 4)), BorderColor); //Chartreuse Best Color
            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);

            base.DrawSpecific(spritebatch);
        }
    }
}
