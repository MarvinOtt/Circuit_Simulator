using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public class Button_Menu : UI_Element
    {
        public bool IsHovered, IsActivated;
        private byte config;

        public Button_Menu(Point pos, Point size, byte config) : base(pos, size)
        {
            this.size = size;
            this.config = config;
        }

        protected override void UpdateSpecific()
        {
            Rectangle hitbox = new Rectangle(absolutpos, size);
            if (config == 2)
                IsActivated = false;
            if (hitbox.Contains(Game1.mo_states.New.Position))
            {
                IsHovered = true;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                    IsActivated ^= true;
            }
            else
                IsHovered = false;
        }

        public override void DrawSpecific(SpriteBatch spritebatch)
        {
            if (!IsHovered && !IsActivated)                                                                                 //PassiveState
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Blue);
            else if (IsHovered && !IsActivated)                                                                             //Hover
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Red);
            else if (!IsHovered && IsActivated)                                                                             //Click
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Purple);
            else                                                                                                            //PostClickHover
                spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), Color.Red);

        }
    }
}
