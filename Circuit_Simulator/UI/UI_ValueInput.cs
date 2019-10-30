using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    public class UI_ValueInput : UI_Element
    {
        public Button_Conf conf;
        string value = "";
        bool IsTyping;
        Rectangle hitbox;
        Keys[] newkeys;
        Keys[] oldkeys;
        public UI_ValueInput(Point pos, Point size, Button_Conf conf) : base( pos, size )
        {
            this.conf = conf;
        }

        protected override void UpdateSpecific()
        {
            hitbox = new Rectangle(absolutpos, size);
            newkeys = Game1.kb_states.New.GetPressedKeys();
            oldkeys = Game1.kb_states.Old.GetPressedKeys();


            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if(Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsTyping = true;
                }
            }
            else if(!hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsTyping = false;
                }
            }

            if(IsTyping)
            {
                if (true)
                {
                    for (int i = 0; i < newkeys.Length; i++)
                    {
                        if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 57)
                        {
                            value += ((int)newkeys[i] - 48).ToString();
                        }
                        else if ((int)newkeys[i] == 8 && value.Length > 0)
                        {
                            value = value.Remove(value.Length - 1, 1);
                        }
                    }
                }
              
            }
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.BackgroundColor);

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), UI_Handler.BackgroundColor, 1);
            if (IsTyping)
            {
                if (DateTime.Now.Millisecond % 1000 < 500)
                    spritebatch.DrawFilledRectangle(new Rectangle(new Point(absolutpos.X + (int)conf.font.MeasureString(value).X, absolutpos.Y), new Point(1, size.Y)), Color.Wheat);
            }
            spritebatch.DrawString(conf.font, value, absolutpos.ToVector2(), Color.White);
            base.DrawSpecific(spritebatch);



        }
    } 
}
