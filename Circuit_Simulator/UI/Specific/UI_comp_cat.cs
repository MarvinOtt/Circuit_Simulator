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
    public class UI_Comp_Cat : UI_MultiElement<UI_Element>
    {
        public string title;
        public Generic_Conf conf;
        public UI_List<UI_Component> Components;
        public bool IsFold, IsHover;
        Vector2 title_pos;

        public UI_Comp_Cat(string title, Generic_Conf conf ) : base(Point.Zero)
        {
            this.title = title;
            this.conf = conf;
            Components = new UI_List<UI_Component>(new Point(0, UI_Component.height), false);
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

        public override void ChangedUpdate2False()
        {
            IsHover = false;
            base.ChangedUpdate2False();
        }

        public override void UpdatePos()
        {
            base.UpdatePos();
            if (IsFold)
                size.Y = UI_Component.height;
            else
                size.Y = UI_Component.height * (1 + Components.ui_elements.Count);
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, new Point(size.X, UI_Component.height));

            //if (hitbox.Contains(Game1.mo_states.New.Position) == UI_ComponentBox.cathitbox.Contains(Game1.mo_states.New.Position))
            //{
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHover = true;
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    IsFold ^= true;
                    Components.GetsUpdated = Components.GetsDrawn = !IsFold;
                }
            }
            else
                IsHover = false;
            //}

            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (IsHover)
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, new Point(size.X, UI_Component.height)),conf.Syscolors[1]);
            spritebatch.DrawString(conf.font, title, absolutpos.ToVector2() + title_pos, conf.fontcol);

            base.DrawSpecific(spritebatch);
        }
    }
}
