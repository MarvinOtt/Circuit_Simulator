using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_InfoBox : UI_Box<UI_Element>
    {
        public UI_List<UI_String> values;

        public UI_InfoBox(Pos pos, Point size) : base(pos, size)
        {
            GetsUpdated = GetsDrawn = false;
            values = new UI_List<UI_String>(new Pos(2), false);
            Add_UI_Elements(values);
        }

        public  void ShowInfo()
        {
            
            pos.pos = App.mo_states.New.Position + new Point(10, 10);
            absolutpos = pos.pos;
            size.X = values.size.X + 4;
            size.Y = values.size.Y + 4;
            GetsUpdated = GetsDrawn = true;
            UpdatePos();
        }

        public void HideInfo()
        {
            GetsUpdated = GetsDrawn = false;
        }


        public override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);

        }
    }
}
