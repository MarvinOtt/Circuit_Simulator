﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    class UI_TB_Dropdown : UI_MultiElement
    {
        Color BackgroundColor;
        Color BorderColor;

        public UI_TB_Dropdown(Point pos) : base(pos)
        {
            BackgroundColor = new Color(new Vector3(0.08f));
            BorderColor = new Color(new Vector3(0.25f));
        }

        public UI_TB_Dropdown(Point pos, Point size) : base(pos, size)
        {
            BackgroundColor = new Color(new Vector3(0.08f)); 
            BorderColor = new Color(new Vector3(0.25f));
        }




        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }
        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
        }
    }
}