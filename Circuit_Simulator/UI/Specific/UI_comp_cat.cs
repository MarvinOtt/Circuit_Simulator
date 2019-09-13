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
    public class UI_comp_cat : UI_MultiElement
    {
        public string title;
        public Button_Conf conf;
        public Point foldedsize, nonfoldedsize;
        public bool IsFold;
        public UI_comp_cat(Point pos, string title, Button_Conf conf ) : base(pos)
        {
            this.title = title;
            this.conf = conf;
            this.nonfoldedsize = new Point(0, 16);
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (hitbox.Contains(Game1.mo_states.New.Position) && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsFold ^= true;

            }

        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            //spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[3]);
            spritebatch.DrawString(conf.font, title, absolutpos.ToVector2() + text_pos, conf.fontcol);
        }
    }
}
