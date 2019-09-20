﻿using Microsoft.Xna.Framework;
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
    public class UI_Window : UI_MultiElement<UI_Element>
    {
        static Color BackgroundColor = new Color(new Vector3(0.15f));
        static Color BorderColor = new Color(new Vector3(0.45f));
        string Title;
        static Texture2D tex;
        public static int headheight = 20;
        public int resize_type;
        public bool IsResize;
        bool IsGrab;
        Point Grabpos;

        public UI_Window(Point pos, Point size, string Title) : base(pos, size)
        {
            this.Title = Title;
            if (tex == null)
                tex = Game1.content.Load<Texture2D>("UI\\Window_SM");
            Add_UI_Elements(new TexButton(new Point(-18, 2), new Point(16), new Point(0), tex, new TexButton_Conf(2))); //X Button
           
            
            
        }

        protected virtual void Resize()
        {

        }

        protected override void UpdateSpecific()
        {
            if(((TexButton)ui_elements[0]).IsActivated)
                GetsUpdated = GetsDrawn = false;
            // Handling Draging of Window
            Rectangle Grabbox = new Rectangle(absolutpos, new Point(size.X - ui_elements[0].size.X - 4, ui_elements[0].size.Y + 4));
            if (Grabbox.Contains(Game1.mo_states.New.Position) && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsGrab = true;
                Grabpos = Game1.mo_states.New.Position - pos;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            }
            if (IsGrab && Game1.mo_states.IsLeftButtonToggleOff())
            {
                IsGrab = false;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            if(IsGrab)
            {
                pos = Game1.mo_states.New.Position - Grabpos;
            }
            if (pos.X < 0)
                pos.X = 0;
            if (pos.X + size.X > Game1.Screenwidth)
                pos.X = Game1.Screenwidth - size.X;
            if (pos.Y < 0)
                pos.Y = 0;
            if (pos.Y + headheight > Game1.Screenheight)
                pos.Y = Game1.Screenheight - headheight;
            absolutpos = pos;

            Rectangle Resize_bottom_box = new Rectangle(absolutpos + new Point(0, size.Y - 5), new Point(size.X, 10));
            if (Resize_bottom_box.Contains(Game1.mo_states.New.Position))
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                if(Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsResize = true;
                    resize_type = 1;
                }
                   
            }
            Rectangle Resize_right_box = new Rectangle(absolutpos + new Point(size.X - 5, headheight), new Point(10, size.Y - headheight));
            if (Resize_right_box.Contains(Game1.mo_states.New.Position))
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsResize = true;
                    resize_type = 2;
                }

            }
            Rectangle Resize_left_box = new Rectangle(absolutpos + new Point(-5, headheight), new Point(10, size.Y - headheight));
            if (Resize_left_box.Contains(Game1.mo_states.New.Position))
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsResize = true;
                    resize_type = 3;
                }

            }
            if (IsResize)
            {
                Resize();
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    IsResize = false;
                }
                switch (resize_type)
                {
                    case 1:
                        size.Y += Game1.mo_states.New.Position.Y - Game1.mo_states.Old.Position.Y;
                        break;
                    case 2:
                        size.X += Game1.mo_states.New.Position.X - Game1.mo_states.Old.Position.X;
                        break;
                    case 3:
                        size.X -= Game1.mo_states.New.Position.X - Game1.mo_states.Old.Position.X;
                        pos.X += Game1.mo_states.New.Position.X - Game1.mo_states.Old.Position.X;
                        absolutpos.X += Game1.mo_states.New.Position.X - Game1.mo_states.Old.Position.X;
                        break;



                }
            }
                

            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, new Point(size.X, ui_elements[0].size.Y + 4)), BorderColor); //Chartreuse Best Color

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
        }
    }
}
