using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_ComponentBox : UI_Window
    {
        public UI_List<UI_Comp_Cat> Catagories;
        private RenderTarget2D comp_box_target;
        public static Rectangle cathitbox;

        public UI_ComponentBox(Point pos, Point size, string title, Point minsize) : base(pos, size, title, minsize )
        {
            Catagories = new UI_List<UI_Comp_Cat>(new Point(0, 50));
            comp_box_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, 1000, 1000);
            Catagories.parent = this;
            //Add_UI_Elements(Catagories);
        }

        protected override void Resize()
        {
            Catagories.ui_elements.ForEach(x => x.SetXSize(size.X));
            //ui_elements.Where(x => x.GetType() == typeof(UI_Comp_Cat)).ForEach(c => ((UI_Comp_Cat)c).SetXSize(size.X));
        }

        public void Add_Categories(params UI_Comp_Cat[] cats)
        {
            cats.ForEach(x => x.SetXSize(size.X));
            Catagories.Add_UI_Elements(cats);
        }

        protected override void UpdateSpecific()
        {
            cathitbox = new Rectangle(absolutpos + new Point(0, 50), new Point(size.X, size.Y - 50));
            if (!IsResize)
                Catagories.Update();

            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            base.DrawSpecific(spritebatch);
            spritebatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTarget(comp_box_target);
            Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(new Vector3(-absolutpos.X, -absolutpos.Y - 50, 0)));
            this.Catagories.Draw(spritebatch);
            spritebatch.End();
            Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            spritebatch.Begin();
            spritebatch.Draw(comp_box_target, absolutpos.ToVector2() + new Vector2(1, 51), new Rectangle(0, 0, size.X - 2, size.Y - 50 - 2), Color.White);

        }

    }
}
