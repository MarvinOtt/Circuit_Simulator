using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_List<T> : UI_MultiElement<T> where T : UI_Element
    {
        bool DrawBackground;
        Color BackgroundCol;

        public UI_List(Pos pos, bool CanBeSizeRelated) : base(pos)
        {
            this.CanBeSizeRelated = CanBeSizeRelated;
        }
        public UI_List(Pos pos, Color backgroundcol, bool CanBeSizeRelated) : base(pos)
        {
            DrawBackground = true;
            BackgroundCol = backgroundcol;
            this.CanBeSizeRelated = CanBeSizeRelated;
        }

        public override void Add_UI_Elements(params T[] elements)
        {
            if (elements.Length > 0)
            {
                elements.ForEach(x => x.parent = this);
                ui_elements.AddRange(elements);
                size.X = ui_elements.Max(x => x.size.X);
                size.Y = ui_elements.Sum(x => x.size.Y);
            }
        }

        public override void UpdatePos()
        {
            base.UpdatePos();
            Point currentpos = new Point(0, 0);
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].pos.pos = currentpos;
                ui_elements[i].UpdatePos();
                currentpos.Y += ui_elements[i].size.Y;
            }
            if (ui_elements.Count > 0)
            {
                size.Y = ui_elements.Max(x => x.pos.Y + x.size.Y);
                size.X = ui_elements.Max(x => x.size.X);
            }
        }


        protected override void UpdateAlways()
        {
            Point currentpos = Point.Zero;
            for (int i = 0; i < ui_elements.Count; ++i)
            {
                ui_elements[i].pos.pos = currentpos;
                ui_elements[i].Update();
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
