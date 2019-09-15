using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator.UI
{
    public class UI_List<T> : UI_MultiElement<T> where T : UI_Element
    {
        bool DrawBackground;
        Color BackgroundCol;
        public UI_List(Point pos) : base(pos)
        {

        }
        public UI_List(Point pos, Color backgroundcol) : base(pos)
        {
            DrawBackground = true;
            BackgroundCol = backgroundcol;
        }

        public override void Add_UI_Elements(params T[] elements)
        {
            elements.ForEach(x => x.parent = this);
            ui_elements.AddRange(elements);
            size.X = ui_elements.Max(x => x.size.X);
            size.Y = ui_elements.Sum(x => x.size.Y);
        }

        protected override void UpdateSpecific()
        {
            Point currentpos = Point.Zero;
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].pos = currentpos;
                ui_elements[i].Update();
                currentpos.Y += ui_elements[i].size.Y;
            }
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (DrawBackground)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundCol);
            base.DrawSpecific(spritebatch);
        }
    }
}
