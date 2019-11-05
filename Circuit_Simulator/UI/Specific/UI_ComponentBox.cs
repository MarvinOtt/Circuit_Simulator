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
    public class UI_ComponentBox : UI_Window
    {
        public UI_Scrollable<UI_List<UI_Comp_Cat>> Catagories;
        //private RenderTarget2D comp_box_target;
        public static Rectangle cathitbox;

        public UI_ComponentBox(Point pos, Point size, string title, Point minsize, Button_Conf conf, bool IsResizeable) : base(pos, size, title, minsize, conf, IsResizeable)
        {
            Catagories = new UI_Scrollable<UI_List<UI_Comp_Cat>>(new Point(bezelsize, 50), Point.Zero);
            Catagories.Add_UI_Elements(new UI_List<UI_Comp_Cat>(Point.Zero, false));
            //comp_box_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, 1000, 1000);
            Add_UI_Elements(Catagories);
        }

        protected override void Resize()
        {
            Catagories.ui_elements[0].ui_elements.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            //ui_elements.Where(x => x.GetType() == typeof(UI_Comp_Cat)).ForEach(c => ((UI_Comp_Cat)c).SetXSize(size.X));
        }

        public void Add_Categories(params UI_Comp_Cat[] cats)
        {
            cats.ForEach(x => x.SetXSize(size.X - bezelsize * 2));
            Catagories.ui_elements[0].Add_UI_Elements(cats);
        }

        protected override void UpdateSpecific()
        {

            base.UpdateSpecific();
            Catagories.size = new Point(size.X - bezelsize * 2, size.Y - 50 - bezelsize);

            //Catagories.ScrollPosOrigin = absolutpos + new Point(bezelsize, 50);
            //Catagories.ScrollSize = new Point(size.X - bezelsize * 2, size.Y - 50 - bezelsize);
            //cathitbox = new Rectangle(absolutpos + new Point(bezelsize, 50), new Point(size.X - bezelsize * 2, size.Y - 50 - bezelsize));
            //if (!IsResize)
            //    Catagories.Update();

        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
            //spritebatch.End();
            //Game1.graphics.GraphicsDevice.SetRenderTarget(comp_box_target);
            //Game1.graphics.GraphicsDevice.Clear(new Color(new Vector3(0.1f)));
            //spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(-absolutpos.X - bezelsize, -absolutpos.Y - 50, 0)));
            //this.Catagories.Draw(spritebatch);
            //spritebatch.End();
            //Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            //spritebatch.Begin();
            //spritebatch.Draw(comp_box_target, absolutpos.ToVector2() + new Vector2(bezelsize, 50), new Rectangle(0, 0, size.X - bezelsize * 2, size.Y - 50 - bezelsize), Color.White);

        }

    }
}
