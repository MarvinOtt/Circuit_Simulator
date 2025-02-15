﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
    public class UI_Button : UI_Element
    {
        public Generic_Conf conf;
        public bool IsHovered, IsActivated, IsToggle, DrawBorder;

        public delegate void Button_Activated_Handler(object sender);
        public event Button_Activated_Handler GotActivatedLeft = delegate { };
        public event Button_Activated_Handler GotToggledLeft = delegate { };
        public event Button_Activated_Handler GotActivatedRight = delegate { };
		public event Button_Activated_Handler GetsPressedLeft = delegate { };
		public event Button_Activated_Handler GetsPressedRight = delegate { };
		public event Button_Activated_Handler GetsHovered = delegate { };

        public UI_Button(Pos pos, Point size, bool DrawBorder, Generic_Conf conf) : base(pos, size)
        {
            this.DrawBorder = DrawBorder;
            this.conf = conf;
        }

        public override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            IsToggle = false;
            if (hitbox.Contains(App.mo_states.New.Position))
            {
                IsHovered = true;
                if (App.mo_states.IsLeftButtonToggleOff())
                {
                    IsActivated ^= true;
                    IsToggle = true;
                    if (IsActivated)
                        GotActivatedLeft(this);
                    GotToggledLeft(this);
					GetsPressedLeft(this);
				}
                if (App.mo_states.IsRightButtonToggleOff())
                {
                    GotActivatedRight(this);
                }
				if (App.mo_states.New.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
					GetsPressedLeft(this);
				if (App.mo_states.New.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
					GetsPressedRight(this);
				GetsHovered(this);
            }
			base.UpdateSpecific();
        }

        protected override void UpdateAlways()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            if (!hitbox.Contains(App.mo_states.New.Position) || UI_Handler.UI_AlreadyActivated || (UI_Handler.IsInScrollable && !UI_Handler.IsInScrollable_Bounds.Contains(App.mo_states.New.Position)))
                IsHovered = false;
            if (conf.behav == 2)
                IsActivated = false;
			base.UpdateAlways();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), conf.BGColor);
            if(DrawBorder)
                spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), conf.BorderColor, 1);
        }
    }
}
