using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public class Simulator
    {
        public const int SIZEX = 16384;
        public const int SIZEY = 16384;
        public const int LAYER_NUM = 8;

        Effect sim_effect;
        RenderTarget2D main_target;
        Texture2D outputtex, logictex;
        byte[,] IsWire;
        byte[,,] WireID_Chunks;

        Point worldpos;
        int worldzoom = 0;

        int sim_speed = 1;


        public Simulator()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
            sim_effect = Game1.content.Load<Effect>("sim_effect");
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            outputtex = new Texture2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            logictex = new Texture2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8);
            IsWire = new byte[SIZEX, SIZEY];
            WireID_Chunks = new byte[SIZEX, SIZEY, LAYER_NUM];

            sim_effect.Parameters["Screenwidth"].SetValue(Game1.Screenwidth);
            sim_effect.Parameters["Screenheight"].SetValue(Game1.Screenheight);
            sim_effect.Parameters["worldsizex"].SetValue(SIZEX);
            sim_effect.Parameters["worldsizey"].SetValue(SIZEY);
        }

        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            main_target?.Dispose();
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            outputtex?.Dispose();
            outputtex = new Texture2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);

            sim_effect.Parameters["Screenwidth"].SetValue(Game1.Screenwidth);
            sim_effect.Parameters["Screenheight"].SetValue(Game1.Screenheight);
            sim_effect.Parameters["worldsizex"].SetValue(SIZEX);
            sim_effect.Parameters["worldsizey"].SetValue(SIZEY);
        }

        public void screen2worldcoo_int(Vector2 screencoos, out int x, out int y)
        {
            x = (int)((screencoos.X - worldpos.X) / (float)Math.Pow(2, worldzoom));
            y = (int)((screencoos.Y - worldpos.Y) / (float)Math.Pow(2, worldzoom));
        }

        public void Update()
        {
            int mo_worldposx, mo_worldposy;
            screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out mo_worldposx, out mo_worldposy);

            #region INPUT
            if (Game1.kb_states.New.IsKeyDown(Keys.W))
                worldpos.Y += 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.S))
                worldpos.Y -= 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.A))
                worldpos.X += 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.D))
                worldpos.X -= 10;

            if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
            {
                if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue && worldzoom > -8) // Zooming Out
                {
                    worldzoom -= 1;
                    Point diff = Game1.mo_states.New.Position - worldpos;
                    worldpos += new Point(diff.X / 2, diff.Y / 2);
                }
                else if(Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue && worldzoom < 8) // Zooming In
                {
                    worldzoom += 1;
                    Point diff = Game1.mo_states.New.Position - worldpos;
                    worldpos -= diff;
                }
            }
            #endregion

            sim_effect.Parameters["zoom"].SetValue((float)Math.Pow(2, worldzoom));
            sim_effect.Parameters["coos"].SetValue(worldpos.ToVector2());
            sim_effect.Parameters["mousepos_X"].SetValue(mo_worldposx);
            sim_effect.Parameters["mousepos_Y"].SetValue(mo_worldposy);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.End();

            sim_effect.Parameters["logictex"].SetValue(logictex);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, sim_effect, Matrix.Identity);
            spritebatch.Draw(outputtex, Vector2.Zero, Color.White);
            spritebatch.End();



            spritebatch.Begin();

        }
    }
}
