﻿using Microsoft.Xna.Framework;
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

        public int final_value;
        public string value = "";
        private string oldvalue;
        public bool IsTyping;
        public int maxlength = 40;
        int inputtype;
        Rectangle hitbox;
        Keys[] newkeys;
        Keys[] oldkeys;

        public delegate void Value_Changed_Handler(object sender);
        public event Value_Changed_Handler ValueChanged = delegate { };

        public UI_ValueInput(Pos pos, Point size, Generic_Conf conf, int inputtype) : base(pos, size)
        {
            this.conf = conf;
            this.inputtype = inputtype;
            ValueChanged += ValueChange;
        }
        public UI_ValueInput(Pos pos, Point size, Generic_Conf conf, int inputtype, int maxlength) : base(pos, size)
        {
            this.conf = conf;
            this.inputtype = inputtype;
            this.maxlength = maxlength;
            ValueChanged += ValueChange;
        }

        public void ValueChange(object sender)
        {
            if (inputtype == 1)
            {
                try
                {
                    final_value = int.Parse(value);
                }
                catch (Exception exp)
                { Console.WriteLine(exp.Message); }
            }
        }

        public void Set2Typing()
        {
            IsTyping = true;
            oldvalue = value;
        }

        public void MakeValueChanged()
        {
            ValueChanged(this);
        }

        public override void UpdateSpecific()
        {
            hitbox = new Rectangle(absolutpos, size);



            if (hitbox.Contains(App.mo_states.New.Position))
            {
                if (App.mo_states.IsLeftButtonToggleOn())
                {
                    IsTyping = true;
                }
            }
            else if (!hitbox.Contains(App.mo_states.New.Position) && IsTyping)
            {
                if (App.mo_states.IsLeftButtonToggleOff() || App.mo_states.IsRightButtonToggleOff())
                {
                    IsTyping = false;
                    ValueChanged(this);
                }
            }
            if (IsTyping && App.kb_states.New.IsKeyDown(Keys.Enter))
            {
                IsTyping = false;
                ValueChanged(this);
            }
            if (IsTyping && App.kb_states.New.IsKeyDown(Keys.Escape))
            {
                IsTyping = false;
                value = oldvalue;
                ValueChanged(this);
            }

            if (IsTyping)
            {
                UI_Handler.UI_Active_State = UI_Handler.UI_Active_Main;
                newkeys = App.kb_states.New.GetPressedKeys();
                oldkeys = App.kb_states.Old.GetPressedKeys();
                bool IsShift = App.kb_states.New.AreKeysDown(Keys.LeftShift) || App.kb_states.New.AreKeysDown(Keys.RightShift);
                for (int i = 0; i < newkeys.Length; i++)
                {
                    if (!oldkeys.Contains(newkeys[i]))
                    {
                        switch (inputtype)
                        {
                            case 1: //Numbers only
                                if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 57 && value.Length <= maxlength - 1)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }

                                break;

                            case 2: //binary only
                                if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 49 && value.Length <= maxlength - 1)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }
                                break;

                            case 3: //Text
                                if (value.Length <= maxlength - 1)
                                {
                                    char key_char;
                                    bool IsValid = Extensions.TryConvertKeyboardInput(newkeys[i], IsShift, out key_char);
                                    if(IsValid)
                                    {
                                        value += key_char;
                                    }
                             
                                }
                                else if ((int)newkeys[i] >= 48 && (int)newkeys[i] <= 57 && value.Length <= maxlength - 1)
                                {
                                    value += ((int)newkeys[i] - 48).ToString();
                                }
                                break;
                            default:
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
                    spritebatch.DrawFilledRectangle(new Rectangle(new Point(absolutpos.X + 5 + Xsize, absolutpos.Y + 2), new Point(1, size.Y - 4)), Color.White);
                }
            }
            spritebatch.DrawString(conf.font, value, (absolutpos + new Point(4, size.Y / 2 - (int)(conf.font.MeasureString(value).Y / 2))).ToVector2(), Color.White);
            base.DrawSpecific(spritebatch);
        }
    }
}
