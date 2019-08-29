using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public UI_Window(Point pos, Point size, string Title) : base(pos, size)
        {
            this.Title = Title;
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("UI\\Window_SM");
            Add_UI_Element(new Button(new Point(-18, 2), new Point(16), new Point(0), tex, 2)); //X Button
           
            BackgroundColor = new Color(new Vector3(0.1f)); 
            BorderColor = new Color(new Vector3(0.25f));
        }




        protected override void UpdateSpecific()
        {
            if(((Button)ui_elements[0]).IsActivated)
                GetsUpdated = GetsDrawn = false;

            base.UpdateSpecific();
        }
        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);

            base.DrawSpecific(spritebatch);
        }
    }
}
