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
            target = new RenderTarget2D(App.graphics.GraphicsDevice, 1000, 1000);
        }

        public override void Add_UI_Elements(params T[] elements)
        {
            base.Add_UI_Elements(elements);
        }

        public override void UpdateSpecific()
        {
            UI_Handler.IsInScrollable_Bounds = new Rectangle(absolutpos, size);
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

                if (new Rectangle(absolutpos, size).Contains(App.mo_states.New.Position) && ysize > size.Y)
                {
                    if (App.mo_states.New.ScrollWheelValue < App.mo_states.Old.ScrollWheelValue)
                    {
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += 20 * (App.mo_states.New.ScrollWheelValue - App.mo_states.Old.ScrollWheelValue) / 120; } });
                        UpdatePos();
                        int maxypos = ui_elements.Max(x => x.pos.Y_abs - pos.Y_abs + x.size.Y);
                        if (maxypos < size.Y)
                            ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += size.Y - maxypos; } });
                    }
                    if (App.mo_states.New.ScrollWheelValue > App.mo_states.Old.ScrollWheelValue)
                    {
                        ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y += 20 * (App.mo_states.New.ScrollWheelValue - App.mo_states.Old.ScrollWheelValue) / 120; } });
                        UpdatePos();
                        int minypos = ui_elements.Min(x => x.pos.Y_abs - pos.Y_abs);
                        if (minypos > 0)
                            ui_elements.ForEach(x => { if (x.pos.parent == this) { x.pos.Y -= minypos; } });
                    }
                }
            }
            DenyScroll = false;
            //UI_Handler.IsInScrollable = false;
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
            RenderTargetBinding[] previoustargets = App.graphics.GraphicsDevice.GetRenderTargets();
            App.graphics.GraphicsDevice.SetRenderTarget(target);
            App.graphics.GraphicsDevice.Clear(new Color(new Vector3(0.1f)));
            Matrix matrix = Matrix.CreateTranslation(new Vector3(-absolutpos.X, -absolutpos.Y, 0));
            App.Render_PreviousMatrix[App.Render_PreviousMatrix_Index] = matrix;
            App.Render_PreviousMatrix_Index++;
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

            base.DrawSpecific(spritebatch);

            spritebatch.End();
            App.graphics.GraphicsDevice.SetRenderTargets(previoustargets);
            if(previoustargets.Length > 0)
                spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, App.Render_PreviousMatrix[App.Render_PreviousMatrix_Index - 2]);
            else
                spritebatch.Begin();
            App.Render_PreviousMatrix_Index--;
            spritebatch.Draw(target, absolutpos.ToVector2(), new Rectangle(Point.Zero, size), Color.White);

        }
    }
}
