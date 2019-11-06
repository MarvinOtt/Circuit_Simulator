//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Circuit_Simulator.UI.UI_Configs;

//namespace Circuit_Simulator.UI.Specific
//{
//    public class UI_WireInfoBox: UI_InfoBox
//    {
//        bool state;
//        UI_String layer;
        



//        public UI_WireInfoBox(Point pos, Point size, UI_String layer) : base(pos, size)
//        {
//            this.layer = layer;
//            GetsUpdated = GetsDrawn = false;
//        }


//        public override void showInfo()
//        {
//            base.showInfo();
//            List<string> strings = new List<string>();
//            strings.Add(state.ToString());
//            strings.Add(layer.value);
//            int maxsizeX = strings.Max(x => layer.conf.font.MeasureString(x).ToPoint().X);
//            pos = Game1.mo_states.New.Position + new Point(10, 10);
//            size.X = maxsizeX + 4;
//            size.Y = layer.conf.font.MeasureString(layer.value).ToPoint().Y * 2 + 4;
//        }
//        protected override void UpdateSpecific()
//        {
//            base.UpdateSpecific();
            
//        }

//        protected override void DrawSpecific(SpriteBatch spritebatch)
//        {
//            base.DrawSpecific(spritebatch);
//            spritebatch.DrawString(layer.conf.font, layer.value, pos.ToVector2() + new Vector2(2, layer.conf.font.MeasureString(layer.value).Y * 0 + 2), layer.conf.fontcol);
//            spritebatch.DrawString(layer.conf.font, state.ToString(), pos.ToVector2() + new Vector2(2, layer.conf.font.MeasureString(layer.value).Y * 1 + 2) , layer.conf.fontcol);
//        }
//    }
//}
