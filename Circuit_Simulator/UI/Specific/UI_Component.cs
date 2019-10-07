﻿using Microsoft.Xna.Framework;
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
        public bool IsDrag, IsHover;
        public Button_Conf conf;
        Vector2 text_pos;

        public UI_Component(string name, Button_Conf conf) : base(Point.Zero, new Point(0, height))
        {
            
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

            //if (hitbox.Contains(Game1.mo_states.New.Position) == UI_ComponentBox.cathitbox.Contains(Game1.mo_states.New.Position))
            //{
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsDrag = true;
                    UI_Handler.dragcomp.GetsUpdated = true;
                    UI_Handler.dragcomp.GetsDrawn = true;
                    UI_Handler.dragcomp.comp = this;
                    UI_Handler.ZaWarudo = UI_Handler.dragcomp;
                    Game1.simulator.InizializeComponentDrag(ID);
                    Game1.simulator.IsCompDrag = true;
                    System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                }
                IsHover = true;
            }
            else
                IsHover = false;
            //}
            if (IsDrag && Game1.mo_states.IsLeftButtonToggleOff())
                IsDrag = false;
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if(IsHover)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.Syscolors[1]);
            spritebatch.DrawString(conf.font, name, absolutpos.ToVector2() + text_pos, conf.fontcol);

        }
    }
}
