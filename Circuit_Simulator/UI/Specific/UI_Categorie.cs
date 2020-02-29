using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_Categorie<T> : UI_MultiElement<UI_Element> where T : UI_Element
    {
        public static int height = 20;
        public string title;
        public Generic_Conf conf;
        public UI_Component cat;
        public UI_List<T> Components;
        public bool IsFold, IsHover2;
        Vector2 title_pos;

        public delegate void Object_Handler(object sender);
        public event Object_Handler GotFolded = delegate { };

        public UI_Categorie(string title, Generic_Conf conf ) : base(Pos.Zero)
        {
            this.title = title;
            this.conf = conf;
            Components = new UI_List<T>(new Pos(0, height), false);
            cat = new UI_Component(new Pos(0), new Point(20, height), title, 0, 0, conf);
            Add_UI_Elements(cat, Components);
            Vector2 titlesize = conf.font.MeasureString(title);
            title_pos = new Vector2(4, (int)(size.Y / 2 - titlesize.Y / 2));
        }

        public void AddComponents(params T[] components)
        {
            Components.Add_UI_Elements(components);
            Components.ui_elements.Sort(delegate (T x, T y)
            {
                if (x.Sort_Name == null && y.Sort_Name == null) return 0;
                else if (x.Sort_Name == null) return -1;
                else if (y.Sort_Name == null) return 1;
                else return x.Sort_Name.CompareTo(y.Sort_Name);
            });
        }

        public void SetXSize(int Xsize)
        {
            size.X = Xsize;
            cat.size.X = Xsize;
            Components.ui_elements.ForEach(x => x.size.X = Xsize);
        }

        public override void ChangedUpdate2False()
        {
            IsHover2 = false;
            base.ChangedUpdate2False();
        }

        public override void UpdatePos()
        {
            base.UpdatePos();
            if (IsFold)
                size.Y = height;
            else
                size.Y = height * (1 + Components.ui_elements.Count);
        }
        public void Fold(bool value)
        {
            IsFold = value;
            Components.GetsUpdated = Components.GetsDrawn = !IsFold;
            GotFolded(this);
        }

        public override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, new Point(size.X, height));

            if (hitbox.Contains(App.mo_states.New.Position))
            {
                if (App.mo_states.IsLeftButtonToggleOff())
                {
                    Fold(!IsFold);
                }
            }

            base.UpdateSpecific();
        }

        protected override void UpdateAlways()
        {
            Rectangle hitbox = new Rectangle(absolutpos, new Point(size.X, height));

  
            base.UpdateAlways();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
        }
    }
}
