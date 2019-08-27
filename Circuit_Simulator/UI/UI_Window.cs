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

        public UI_Window(Point pos, Point size, string Title) : base(pos, size)
        {
            this.Title = Title;
            BackgroundColor = new Color(new Vector3(0.1f));
            BorderColor = new Color(new Vector3(0.25f));
        }




        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
            base.DrawSpecific(spritebatch);
        }
    }
}
