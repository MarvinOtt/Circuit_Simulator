using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    class UI_Dropdown_Button : UI_Element
    {
        Color BackgroundColor;
        Color BorderColor;
        Texture2D tex;
        public Point tex_pos;
        public UI_Dropdown_Button(Point pos, Point size, Point tex_pos, Texture2D tex) : base(pos, size)
        {
            BackgroundColor = new Color(new Vector3(0.08f));
            BorderColor = new Color(new Vector3(0.25f));
            this.tex = tex;
            this.tex_pos = tex_pos;
        }

        protected override void UpdateSpecific()
        {
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);

            base.DrawSpecific(spritebatch);
            spritebatch.Draw(tex, absolutpos.ToVector2(), new Rectangle(tex_pos, size), Color.White);
            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
           
        }
    }
    

}
