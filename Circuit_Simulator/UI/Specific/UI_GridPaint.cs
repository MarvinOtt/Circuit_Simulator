using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_GridPaint : UI_Element
    {
        int GridSize, zoom;
        Vector2 worldpos;
        Point Origin;
        Effect effect;
        Texture2D tex, logictex;
        

        public UI_GridPaint(Pos pos, Point size, int GridSize, Point Origin) : base(pos, size)
        {
            this.GridSize = GridSize;
            this.Origin = Origin;
            tex = new Texture2D(Game1.graphics.GraphicsDevice, size.X, size.Y);
            logictex = new Texture2D(Game1.graphics.GraphicsDevice, GridSize, GridSize, false, SurfaceFormat.Alpha8);
            effect = Game1.content.Load<Effect>("UI\\GridPaint_effect");
            effect.Parameters["worldsizex"].SetValue(GridSize);
            effect.Parameters["worldsizey"].SetValue(GridSize);
            effect.Parameters["Screenwidth"].SetValue(size.X);
            effect.Parameters["Screenheight"].SetValue(size.Y);
            effect.Parameters["origin_X"].SetValue(Origin.X);
            effect.Parameters["origin_Y"].SetValue(Origin.Y);
        }

        public void Screen2worldcoo_int(Vector2 screencoos, out int x, out int y)
        {
            x = (int)((screencoos.X - worldpos.X) / (float)Math.Pow(2, zoom));
            y = (int)((screencoos.Y - worldpos.Y) / (float)Math.Pow(2, zoom));
        }

        protected override void UpdateSpecific()
        {
            if (new Rectangle(pos.pos, size).Contains(Game1.mo_states.New.Position))
            {
                #region Position and Zoom

                if (Game1.kb_states.New.IsKeyDown(Keys.W))
                    worldpos.Y += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.S))
                    worldpos.Y -= 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.A))
                    worldpos.X += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.D))
                    worldpos.X -= 10;

                worldpos.X = (int)(worldpos.X);
                worldpos.Y = (int)(worldpos.Y);

                if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
                {
                    if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue) // Zooming Out
                    {
                        zoom -= 1;
                        Vector2 diff = worldpos - Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2();
                        worldpos = Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2() + diff / 2;
                    }
                    else // Zooming In
                    {
                        zoom += 1;
                        Vector2 diff = worldpos - Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2();
                        worldpos += diff;
                    }
                }

                #endregion
            }

            int mouse_worldpos_X, mouse_worldpos_Y;
            Screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2() - absolutpos.ToVector2(), out mouse_worldpos_X, out mouse_worldpos_Y);
            effect.Parameters["zoom"].SetValue((float)Math.Pow(2, zoom));
            effect.Parameters["coos"].SetValue(worldpos);
            effect.Parameters["mousepos_X"].SetValue(mouse_worldpos_X);
            effect.Parameters["mousepos_Y"].SetValue(mouse_worldpos_Y);
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.End();
            effect.Parameters["logictex"].SetValue(logictex);

            Matrix matrix = Matrix.Identity;
            if (Game1.Render_PreviousMatrix_Index > 0)
                matrix = Game1.Render_PreviousMatrix[Game1.Render_PreviousMatrix_Index - 1];
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect, matrix);
            spritebatch.Draw(tex, pos.pos.ToVector2(), Color.White);
            spritebatch.End();

            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);
            base.DrawSpecific(spritebatch);
        }
    }
}
