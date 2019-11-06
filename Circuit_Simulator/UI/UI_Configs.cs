using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public static class UI_Configs
    {
        public struct Generic_Conf
        {
            public Color[] Syscolors;
            public SpriteFont font;
            public Color fontcol;
            public byte behav;

            public Generic_Conf(Color fontcol, SpriteFont font, byte behav, params Color[] backroundColors)
            {
                this.fontcol = fontcol;
                this.font = font;
                Syscolors = backroundColors;
                this.behav = behav;
            }
        }

        public struct TexButton_Conf
        {
            public bool IsHoverEnabled;
            public Color HoverCol;
            public byte behav;

            public TexButton_Conf(byte behav)
            {
                IsHoverEnabled = false;
                HoverCol = Color.Black;
                this.behav = behav;
            }
            public TexButton_Conf(byte behav, Color HoverCol)
            {
                IsHoverEnabled = true;
                this.HoverCol = HoverCol;
                this.behav = behav;
            }
        }
    }
}
