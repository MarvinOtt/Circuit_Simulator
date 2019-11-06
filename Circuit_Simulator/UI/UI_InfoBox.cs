using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    public class UI_InfoBox : UI_Box
    {
        public UI_List<UI_String> values;

        public UI_InfoBox(Point pos, Point size) : base(pos, size)
        {
            GetsUpdated = GetsDrawn = false;
            values = new UI_List<UI_String>(pos, false);
            ui_elements.Add(values);
        }

        public  void showInfo()
        {
           
            pos = Game1.mo_states.New.Position + new Point(10, 10);
            size.X = values.size.X + 4;
            size.Y = values.size.Y * 2 + 4;
        }


        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

        }
    }
}
