using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public static class Configs
    {
        public class Button_Conf
        {
            public Color[] BackgroundCol;
            public SpriteFont font;
            public Color fontcol;

            public Button_Conf(Color fontcol, SpriteFont font, params Color[] backroundColors)
            {
                this.fontcol = fontcol;
                this.font = font;
                BackgroundCol = backroundColors;
            }


        }
    }
}
