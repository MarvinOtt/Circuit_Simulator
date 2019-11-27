using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_ValueInput : UI_Element
    {
        public Generic_Conf conf;

        public string value = "";
        bool IsTyping;
        int inputtype;
        Rectangle hitbox;
        Keys[] newkeys;
        Keys[] oldkeys;
        public UI_ValueInput(Pos pos, Point size, Generic_Conf conf, int inputtype) : base(pos, size)
        {
            this.conf = conf;
            this.inputtype = inputtype;
        }

        protected override void UpdateSpecific()
        {
            hitbox = new Rectangle(absolutpos, size);



            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsTyping = true;
                }
            }
            else if (!hitbox.Contains(Game1.mo_states.New.Position))
            {
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsTyping = false;
                }
            }

            if (IsTyping)
            {
                newkeys = Game1.kb_states.New.GetPressedKeys();
                oldkeys = Game1.kb_states.Old.GetPressedKeys();
                for (int i = 0; i < newkeys.Length; i++)
                {
                    if (!oldkeys.Contains(newkeys[i]))
                    {
                        switch (inputtype)
                        {
                            case 1: //Numbers only
                                if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 57 && value.Length <= 10)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }
                                
                                break;

                            case 2: //binary only
                                if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 49 && value.Length <= 16)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }
                                break;

                            case 3: //Text
                                if((int)newkeys[i] >= 65 && (int)newkeys[i] <= 90 && value.Length <= 16)
                                {
                                    if(Game1.kb_states.New.AreKeysDown(Keys.LeftShift) || Game1.kb_states.New.AreKeysDown(Keys.RightShift))
                                    {
                                        value += ((char)newkeys[i]);
                                    }
                                    else
                                        value += (char)(((int)newkeys[i]) + 32);
                                   
                                }
                                else if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 57 && value.Length <= 10)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }
                                break;
                         
                        }
                        if ((int)newkeys[i] == 8 && value.Length > 0)
                        {
                            value = value.Remove(value.Length - 1, 1);
                        }
                    }       
                    
                    
                    
                }

                base.UpdateSpecific();
            }
        }
        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), UI_Handler.BackgroundColor);

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), UI_Handler.BorderColor, 1);
            if (IsTyping)
            {
                if (DateTime.Now.Millisecond % 1000 < 500)
                {
                    int Xsize = (int)conf.font.MeasureString(value).X;
                    int Ysize = (int)conf.font.MeasureString("Test").Y;
                    spritebatch.DrawFilledRectangle(new Rectangle(new Point(absolutpos.X + 5 + Xsize, absolutpos.Y + size.Y / 2 - Ysize / 2), new Point(1, Ysize)), Color.White);
                }
            }
            spritebatch.DrawString(conf.font, value, (absolutpos + new Point(4, size.Y / 2 - (int)(conf.font.MeasureString(value).Y / 2))).ToVector2(), Color.White);
            base.DrawSpecific(spritebatch);



        }
    }
}
