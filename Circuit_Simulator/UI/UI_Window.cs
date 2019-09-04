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
        Color BackgroundColor;
        Color BorderColor;
        string Title;
        static Texture2D tex;
        bool IsGrab;

        public UI_Window(Point pos, Point size, string Title) : base(pos, size)
        {
            this.Title = Title;
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("UI\\Window_SM");
            Add_UI_Element(new Button(new Point(-18, 2), new Point(16), new Point(0), tex, 2)); //X Button
           
            BackgroundColor = new Color(new Vector3(0.15f)); 
            BorderColor = new Color(new Vector3(0.45f));
        }




        protected override void UpdateSpecific()
        {
            if(((Button)ui_elements[0]).IsActivated)
                GetsUpdated = GetsDrawn = false;
            Rectangle Grabbox = new Rectangle(absolutpos, new Point(size.X - ui_elements[0].size.X - 4, ui_elements[0].size.Y + 4));
            if (Grabbox.Contains(Game1.mo_states.Old.Position) && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsGrab = true;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            }
            else if (Grabbox.Contains(Game1.mo_states.Old.Position) && Game1.mo_states.IsLeftButtonToggleOff())
            {
                IsGrab = false;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            if(IsGrab)
            {
                Point dif = Game1.mo_states.New.Position - Game1.mo_states.Old.Position;
                pos += dif;
                absolutpos += dif;
            }
            



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
