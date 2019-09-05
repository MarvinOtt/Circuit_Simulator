using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_QuickHBElement : UI_MultiElement
    {
        static Color BackgroundColor = new Color(new Vector3(0.15f));

        public UI_QuickHBElement(Point pos) : base(pos)
        {
            
        }

        public override void Add_UI_Element(UI_Element element)
        {
            int currentSizeX = ui_elements.Sum(x => x.size.X);
            element.pos = new Point(currentSizeX, 0);

            base.Add_UI_Element(element);
        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
       
            base.DrawSpecific(spritebatch);
        }
    }
}
