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
        public class Generic_Conf
        {
            public Color BGColor, BorderColor, tex_color, font_color, HoverColor, ActiveColor, ActiveHoverColor;
            public byte behav;
            public SpriteFont font;

            public Generic_Conf(Generic_Conf baseconf)
            {
                this.ActiveColor = baseconf.ActiveColor;
                this.ActiveHoverColor = baseconf.ActiveHoverColor;
                this.HoverColor = baseconf.HoverColor;
                this.BGColor = baseconf.BGColor;
                this.BorderColor = baseconf.BorderColor;
                this.tex_color = baseconf.tex_color;
                this.font_color = baseconf.font_color;
                this.font = baseconf.font;
                this.behav = baseconf.behav;
            }

            public Generic_Conf(Color ActiveColor = default(Color), Color ActiveHoverColor = default(Color), Color HoverColor = default(Color),
                Color font_color = default(Color), Color BGColor = default(Color), Color BorderColor = default(Color),
                Color tex_color = default(Color), SpriteFont font = null, byte behav = 1)
            {
                this.ActiveColor = ActiveColor;
                this.ActiveHoverColor = ActiveHoverColor;
                this.HoverColor = HoverColor;
                this.BGColor = BGColor;
                this.BorderColor = BorderColor;
                this.tex_color = tex_color;
                this.font_color = font_color;
                this.font = font;
                this.behav = behav;
            }
        }

        //public struct TexButton_Conf
        //{
        //    public bool IsHoverEnabled;
        //    public Color HoverCol;
        //    public byte behav;

        //    public TexButton_Conf(byte behav)
        //    {
        //        IsHoverEnabled = false;
        //        HoverCol = Color.Black;
        //        this.behav = behav;
        //    }
        //    public TexButton_Conf(byte behav, Color HoverCol)
        //    {
        //        IsHoverEnabled = true;
        //        this.HoverCol = HoverCol;
        //        this.behav = behav;
        //    }
        //}
    }
}
