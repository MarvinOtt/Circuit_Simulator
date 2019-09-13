using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.Configs;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Component : UI_Element
    {
        public string name, designation;
        public bool IsGrab;
        Button_Conf conf;
        Vector2 text_pos;

        public UI_Component(Point pos, string name, string designation, Button_Conf conf) : base(pos, new Point(0,16))
        {
            
            this.name = name;
            this.designation = designation;
            this.conf = conf;
            Vector2 textsize = conf.font.MeasureString(name);
            text_pos = new Vector2(0, (int)(size.Y / 2 - textsize.Y / 2));
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            IsGrab = false;

            if (hitbox.Contains(Game1.mo_states.New.Position) && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsGrab ^= true;
                   
                       
               
            }
            
        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            //spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[3]);
            spritebatch.DrawString(conf.font, name, absolutpos.ToVector2() + text_pos, conf.fontcol);
        }
    }
}
