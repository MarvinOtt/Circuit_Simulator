using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator.UI
{
    public class UI_Scrollable<T> : UI_MultiElement<T> where T : UI_Element
    {
        private RenderTarget2D target;

        public UI_Scrollable(Point pos, Point size) : base(pos, size)
        {
            target = new RenderTarget2D(Game1.graphics.GraphicsDevice, 1000, 1000);
        }

        public override void Add_UI_Elements(params T[] elements)
        {
            base.Add_UI_Elements(elements);
            //ui_elements.ForEach(x => x.parent = null);
        }

        protected override void UpdateSpecific()
        {
            if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position))
                ui_elements.ForEach(x => x.GetsUpdated = true);
            else
                ui_elements.ForEach(x => x.GetsUpdated = false);

            if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue)
                {
                    ui_elements.ForEach(x => x.pos.Y -= 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120);
                    int maxypos = 0;
                    maxypos = ui_elements.Max(x => x.pos.Y + x.size.Y);
                    if (maxypos < size.Y)
                        ui_elements.ForEach(x => x.pos.Y += size.Y - maxypos);
                }
                if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue)
                {
                    ui_elements.ForEach(x => x.pos.Y -= 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120);
                    int minypos = ui_elements.Min(x => x.pos.Y);
                    if(minypos > 0)
                        ui_elements.ForEach(x => x.pos.Y -= minypos);
                }
            }


            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTarget(target);
            Game1.graphics.GraphicsDevice.Clear(new Color(new Vector3(0.1f)));
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(-absolutpos.X, -absolutpos.Y, 0)));

            base.DrawSpecific(spritebatch);

            spritebatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            spritebatch.Begin();
            spritebatch.Draw(target, absolutpos.ToVector2(), new Rectangle(Point.Zero, size), Color.White);

        }
    }
}
