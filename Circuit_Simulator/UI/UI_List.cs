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
        bool IsScroll;
        Color BackgroundCol;
        public Point ScrollPosOrigin, ScrollSize;

        public UI_List(Point pos, bool IsScroll) : base(pos)
        {
            this.IsScroll = IsScroll;
            CanBeSizeRelated = !IsScroll;
        }
        public UI_List(Point pos, Color backgroundcol, bool IsScroll) : base(pos)
        {
            DrawBackground = true;
            this.IsScroll = IsScroll;
            BackgroundCol = backgroundcol;
            CanBeSizeRelated = !IsScroll;
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
            if(IsScroll && new Rectangle(ScrollPosOrigin, ScrollSize).Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue)
                {
                    pos.Y = pos.Y + 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120;
                    pos.Y = MathHelper.Min(pos.Y, 50);
                }
                if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue)
                {
                    pos.Y = pos.Y + 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120;
                    //pos.Y = MathHelper.Max(pos.Y, 50);
                }
            }
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
