using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public class Simulator
    {
        Effect sim_effect;
        RenderTarget2D main_target;

        int sim_speed = 1;


        public Simulator()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
            sim_effect = Game1.content.Load<Effect>("sim_effect");
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
        }

        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            main_target?.Dispose();
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
        }

        public void Update()
        {

        }

        public void Draw()
        {

        }
    }
}
