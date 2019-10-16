using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    class UI_ComponentInfoBox : UI_InfoBox
    {
        string comptype = "Missing";
        string infotext = "Missing";


        public UI_ComponentInfoBox(Point pos, Point size, Button_Conf conf) : base(pos, size, conf)
        {
            GetsUpdated = GetsDrawn = false;
        }

        public override void showInfo()
        {
            base.showInfo();
            List<string> strings = new List<string>();
            strings.Add(comptype);
            strings.Add(infotext);
            int maxsizeX = strings.Max(x => conf.font.MeasureString(x).ToPoint().X);
            pos = Game1.mo_states.New.Position + new Point(10, 10);
            size.X = maxsizeX + 4;
            size.Y = conf.font.MeasureString(comptype).ToPoint().Y * 2 + 4;
        }

        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
            spritebatch.DrawString(conf.font, comptype, pos.ToVector2() + new Vector2(2, conf.font.MeasureString(comptype).Y * 0 + 2), conf.fontcol);
            spritebatch.DrawString(conf.font, infotext.ToString(), pos.ToVector2() + new Vector2(2, conf.font.MeasureString(comptype).Y * 1 + 2), conf.fontcol);
        }
    }
}

