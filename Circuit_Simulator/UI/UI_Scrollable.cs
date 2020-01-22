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
    public class UI_Scrollable<T> : UI_MultiElement<T> where T : UI_Element
    {
        private RenderTarget2D target;
        public bool DenyScroll = false;

        public UI_Scrollable(Pos pos, Point size) : base(pos, size)
        {
            target = new RenderTarget2D(Game1.graphics.GraphicsDevice, 1000, 1000);
        }

        public override void Add_UI_Elements(params T[] elements)
        {
            base.Add_UI_Elements(elements);
            //ui_elements.ForEach(x => x.parent = null);
        }

        public override void UpdateSpecific()
        {
            UI_Handler.IsInScrollable = true;
            UI_Handler.IsInScrollable_Bounds = new Rectangle(absolutpos, size);
            //if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position))
            //    ui_elements.ForEach(x => x.GetsUpdated = true);
            //else
            //    ui_elements.ForEach(x => x.GetsUpdated = false);
            if (!DenyScroll)
            {
                int minpos = ui_elements.Min(x => x.pos.Y_abs - pos.Y_abs);
                int maxpos = ui_elements.Max(x => x.pos.Y_abs - pos.Y_abs + x.size.Y);
                int ysize = maxpos - minpos;
                if (ysize <= size.Y && minpos != 0)
                    ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y -= minpos; } });
                else if (ysize > size.Y)
                {
                    if (minpos < 0 && maxpos < size.Y)
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += (size.Y - maxpos); } });
                    if (minpos > 0 && maxpos > size.Y)
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y -= (minpos); } });
                    UpdatePos();
                }

                if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position) && ysize > size.Y)
                {
                    if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue)
                    {
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120; } });
                        UpdatePos();
                        int maxypos = 0;
                        maxypos = ui_elements.Max(x => x.pos.Y_abs - pos.Y_abs + x.size.Y);
                        if (maxypos < size.Y)
                            ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += size.Y - maxypos; } });
                    }
                    if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue)
                    {
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += 20 * (Game1.mo_states.New.ScrollWheelValue - Game1.mo_states.Old.ScrollWheelValue) / 120; } });
                        UpdatePos();
                        int minypos = ui_elements.Min(x => x.pos.Y_abs - pos.Y_abs);
                        if (minypos > 0)
                            ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y -= minypos; } });
                    }
                }
            }
            DenyScroll = false;
            UpdatePos();
            base.UpdateSpecific();
        }

        protected override void UpdateAlways()
        {
            UI_Handler.IsInScrollable = true;
            UI_Handler.IsInScrollable_Bounds = new Rectangle(absolutpos, size);
            base.UpdateAlways();
            UI_Handler.IsInScrollable = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.End();
            RenderTargetBinding[] previoustargets = Game1.graphics.GraphicsDevice.GetRenderTargets();
            Game1.graphics.GraphicsDevice.SetRenderTarget(target);
            Game1.graphics.GraphicsDevice.Clear(new Color(new Vector3(0.1f)));
            Matrix matrix = Matrix.CreateTranslation(new Vector3(-absolutpos.X, -absolutpos.Y, 0));
            Game1.Render_PreviousMatrix[Game1.Render_PreviousMatrix_Index] = matrix;
            Game1.Render_PreviousMatrix_Index++;
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

            base.DrawSpecific(spritebatch);

            spritebatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTargets(previoustargets);
            if(previoustargets.Length > 0)
                spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Game1.Render_PreviousMatrix[Game1.Render_PreviousMatrix_Index - 2]);
            else
                spritebatch.Begin();
            Game1.Render_PreviousMatrix_Index--;
            spritebatch.Draw(target, absolutpos.ToVector2(), new Rectangle(Point.Zero, size), Color.White);

        }
    }
}
