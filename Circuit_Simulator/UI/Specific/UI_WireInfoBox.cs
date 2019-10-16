﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_WireInfoBox: UI_InfoBox
    {
        bool state;
        string layer = "Missing";



        public UI_WireInfoBox(Point pos, Point size, Button_Conf conf) : base(pos, size, conf)
        {
            GetsUpdated = GetsDrawn = false;
        }


        public override void showInfo()
        {
            base.showInfo();
            List<string> strings = new List<string>();
            strings.Add(state.ToString());
            strings.Add(layer);
            int maxsizeX = strings.Max(x => conf.font.MeasureString(x).ToPoint().X);
            pos = Game1.mo_states.New.Position + new Point(10, 10);
            size.X = maxsizeX + 4;
            size.Y = conf.font.MeasureString(layer).ToPoint().Y * 2 + 4;
        }
        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
            
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
            spritebatch.DrawString(conf.font, layer, pos.ToVector2() + new Vector2(2, conf.font.MeasureString(layer).Y * 0 + 2), conf.fontcol);
            spritebatch.DrawString(conf.font, state.ToString(), pos.ToVector2() + new Vector2(2, conf.font.MeasureString(layer).Y * 1 + 2) , conf.fontcol);
        }
    }
}
