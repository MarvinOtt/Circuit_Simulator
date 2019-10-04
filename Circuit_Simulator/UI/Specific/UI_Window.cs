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
    public class UI_Window : UI_MultiElement<UI_Element>
    {
        static Color BackgroundColor = new Color(new Vector3(0.15f));
        static Color BorderColor = new Color(new Vector3(0.45f));
        public Button_Conf conf;
        string Title;
        Vector2 Title_pos;
        Point minsize;
        Point oldsize;
        Point oldrightborderpos;
        static Texture2D tex;
        public static int headheight = 20;
        public static int bezelsize = 10;
        public int resize_type;
        public bool IsResize;
        bool IsGrab;
        Point Grabpos;

        public UI_Window(Point pos, Point size, string Title, Point minsize, Button_Conf conf) : base(pos, size)
        {
            this.minsize = minsize;
            this.Title = Title;
            this.conf = conf;
            Vector2 title_dim = conf.font.MeasureString(Title);
            Title_pos = new Vector2(5, headheight / 2 - title_dim.Y / 2);
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

            Rectangle Resize_bottom_box = new Rectangle(absolutpos + new Point(0, size.Y), new Point(size.X, 10));
            if (Resize_bottom_box.Contains(Game1.mo_states.New.Position) && !IsResize)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNS;
                if(Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsResize = true;
                    resize_type = 1;
                }
                   
            }
            Rectangle Resize_right_box = new Rectangle(absolutpos + new Point(size.X, headheight), new Point(10, size.Y - headheight));
            if (Resize_right_box.Contains(Game1.mo_states.New.Position) && !IsResize)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    IsResize = true;
                    resize_type = 2;
                }

            }
            Rectangle Resize_left_box = new Rectangle(absolutpos + new Point(-10, headheight), new Point(10, size.Y - headheight));
            if (Resize_left_box.Contains(Game1.mo_states.New.Position) && !IsResize)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;
                if (Game1.mo_states.IsLeftButtonToggleOn())
                {
                    oldrightborderpos = new Point(pos.X + size.X, 0);
                    IsResize = true;
                    resize_type = 3;
                }

            }
            if (IsResize)
            {
                UI_Handler.UI_Active = true;
                if (Game1.mo_states.IsLeftButtonToggleOff())
                {
                    IsResize = false;
                }
                switch (resize_type)
                {
                    case 1: // Bottom Resize
                        size.Y = Game1.mo_states.New.Position.Y - absolutpos.Y;
                        if (size.Y <= headheight + minsize.Y)
                            size.Y = headheight + minsize.Y;
                        break;
                    case 2: // Right Resize
                        size.X = Game1.mo_states.New.Position.X - absolutpos.X;
                        if (size.X <= headheight + minsize.X)
                            size.X = headheight + minsize.X;
                        break;
                    case 3: // Left Resize
                        size.X = oldrightborderpos.X - Game1.mo_states.New.Position.X;
                        if (size.X <= headheight + minsize.X)
                            size.X = headheight + minsize.X;
                        pos.X = oldrightborderpos.X - size.X;
                        absolutpos.X = oldrightborderpos.X - size.X;
                        break;
                }
                Resize();
            }
                
            if(!IsResize)

                base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, new Point(size.X, headheight )), BorderColor); //Chartreuse Best Color
            spritebatch.DrawString(conf.font, Title, absolutpos.ToVector2() + Title_pos, conf.fontcol);

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);

        }
    }
}
