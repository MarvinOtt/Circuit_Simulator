//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using static Circuit_Simulator.UI.UI_Configs;

//namespace Circuit_Simulator.UI
//{
//    class UI_ComponentInfoBox : UI_InfoBox
//    {
//        UI_List<UI_String> values;

//        public UI_ComponentInfoBox(Point pos, Point size) : base(pos, size)
//        {
//            GetsUpdated = GetsDrawn = false;
//            UI_List<UI_String> values = new UI_List<UI_String>(pos, false);

//        }

//        public override void showInfo()
//        {
//            base.showInfo();
//            pos = Game1.mo_states.New.Position + new Point(10, 10);
//            size.X = values.size.X + 4;
//            size.Y =values.size.Y * 2 + 4;
//        }


//        protected override void UpdateSpecific()
//        {
//            base.UpdateSpecific();
//        }

//        protected override void DrawSpecific(SpriteBatch spritebatch)
//        {
//            base.DrawSpecific(spritebatch);
//            values.
//            spritebatch.DrawString(conf.font, comptype, pos.ToVector2() + new Vector2(2, conf.font.MeasureString(comptype).Y * 0 + 2), conf.fontcol);

//        }
//    }
//}

