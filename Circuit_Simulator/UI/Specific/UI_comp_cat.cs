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
    public class UI_Comp_Cat : UI_MultiElement<UI_Element>
    {
        public string title;
        public Button_Conf conf;
        UI_List<UI_Component> Components;
        public bool IsFold;
        Vector2 title_pos;

        public UI_Comp_Cat(string title, Button_Conf conf ) : base(Point.Zero)
        {
            this.title = title;
            this.conf = conf;
            Components = new UI_List<UI_Component>(new Point(0, UI_Component.height));
            Add_UI_Elements(Components);
            Vector2 titlesize = conf.font.MeasureString(title);
            title_pos = new Vector2(4, (int)(size.Y / 2 - titlesize.Y / 2));
        }

        public void AddComponents(params UI_Component[] components)
        {
            Components.Add_UI_Elements(components);
        }

        public void SetXSize(int Xsize)
        {
            size.X = Xsize;
            Components.ui_elements.ForEach(x => x.size.X = Xsize);
        }

        protected override void UpdateSpecific()
        {
            if (IsFold)
                size.Y = UI_Component.height;
            else
                size.Y = UI_Component.height * (1 + Components.ui_elements.Count);
            Rectangle hitbox = new Rectangle(absolutpos, new Point(size.X, UI_Component.height));

            if (hitbox.Contains(Game1.mo_states.New.Position) && Game1.mo_states.IsLeftButtonToggleOff())
            {
                IsFold ^= true;
                Components.GetsUpdated = Components.GetsDrawn = !IsFold;
            }
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawString(conf.font, title, absolutpos.ToVector2() + title_pos, conf.fontcol);

            base.DrawSpecific(spritebatch);
        }
    }
}