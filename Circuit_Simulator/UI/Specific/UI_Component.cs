using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Component : UI_Element
    {
        public static int height = 20;
        public string name;
        public int ID;
        public bool IsHover;
        public Button_Conf conf;
        Vector2 text_pos;

        public UI_Component(string name, Button_Conf conf, int ID) : base(Point.Zero, new Point(0, height))
        {
            this.ID = ID;
            this.name = name;
            this.conf = conf;
            Vector2 textsize = conf.font.MeasureString(name);
            text_pos = new Vector2(20, (int)(size.Y / 2 - textsize.Y / 2));
        }

        public override void ChangedUpdate2False()
        {
            IsHover = false;
            base.ChangedUpdate2False();
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);

            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    UI_Handler.dragcomp.GetsUpdated = true;
                    UI_Handler.dragcomp.GetsDrawn = true;
                    UI_Handler.dragcomp.comp = this;
                    UI_Handler.ZaWarudo = UI_Handler.dragcomp;
                    UI_Handler.UI_Active_State = UI_Handler.UI_Active_CompDrag;
                    Game1.simulator.sim_comp.InizializeComponentDrag(ID);
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                }
                IsHover = true;
            }
            else
                IsHover = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHover)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[1]);
            spritebatch.DrawString(conf.font, name, absolutpos.ToVector2() + text_pos, conf.fontcol);

        }
    }
}
