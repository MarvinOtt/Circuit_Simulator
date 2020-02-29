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
    public class UI_Window : UI_MultiElement<UI_Element>
    {
        public static List<UI_Window> All_Windows = new List<UI_Window>();

        static Color BackgroundColor = new Color(new Vector3(0.15f));
        static Color BorderColor = new Color(new Vector3(0.45f));
        private UI_TexButton ExitButton;
        RenderTarget2D target;
        public Generic_Conf conf;
        public string Title;
        Vector2 Title_pos;
        public Point minsize;
        Point oldrightborderpos;
        Point PreSnapSize;
        static Texture2D tex;
        public static int headheight = 20;
        public static int bezelsize = 10;
        public int resize_type;
        public bool IsResizeable;
        bool IsGrab;
        bool Snap, Snapleft, Snapright, Snapbottom;
        Point Grabpos;

        public UI_Window(Pos pos, Point size, string Title, Point minsize, Generic_Conf conf, bool IsResizeable) : base(pos, size)
        {
            IsTypeOfWindow = true;
            this.IsResizeable = IsResizeable;
            this.minsize = minsize;
            this.Title = Title;
            this.conf = conf;
            this.PreSnapSize = size;
            Vector2 title_dim = conf.font.MeasureString(Title);
            Title_pos = new Vector2(5, headheight / 2 - title_dim.Y / 2);
            target = new RenderTarget2D(App.graphics.GraphicsDevice, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height, false, SurfaceFormat.Bgra32, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            if (tex == null)
                tex = App.content.Load<Texture2D>("UI\\Window_SM");
            ExitButton = new UI_TexButton(new Pos(-18, 2, ORIGIN.TR), new Point(16), new Point(0), tex, UI_Handler.gen_conf);
            ExitButton.GotActivatedLeft += Exit_Pressed;
            Add_UI_Elements(ExitButton); //X Button
            All_Windows.Add(this);


        }

        public static void All_Update()
        {
            for (int i = 0; i < All_Windows.Count; ++i)
            {
                All_Windows[i].UpdateMain();
            }
        }

        public static void All_Draw(SpriteBatch spritebatch)
        {
            for (int i = All_Windows.Count - 1; i >= 0; --i)
            {
                All_Windows[i].Draw(spritebatch);
            }
        }

        public static void All_Highlight(UI_Window window)
        {
            int curindex = All_Windows.IndexOf(window);
            All_Windows.Move(curindex, 0);
        }

        protected virtual void Resize()
        {

        }

        public void Exit_Pressed(object sender)
        {
            GetsUpdated = GetsDrawn = false;
        }

        public override void UpdateSpecific()
        {
            if (size.Y >= App.Screenheight)
                size.Y = App.Screenheight;

            Rectangle Hitbox = new Rectangle(absolutpos, size);
            if (Hitbox.Contains(App.mo_states.New.Position) && App.mo_states.IsLeftButtonToggleOn())
            {
                All_Highlight(this);
            }

            // Handling Snapping of the Window
            if (!Snap)
            {
                //Snapleft
                PreSnapSize = size;
                if (IsGrab && App.mo_states.New.Position.X < 2 && App.mo_states.IsLeftButtonToggleOff())
                {

                    size.Y = App.Screenheight - (UI_Handler.buttonheight + UI_Handler.sqarebuttonwidth + UI_Handler.LayerSelectHotbar.size.Y) - 24 ;
                    size.X = minsize.X;

                    pos.X = 0;
                    pos.Y = UI_Handler.buttonheight + UI_Handler.sqarebuttonwidth;
                    Resize();
                    Snap = Snapleft = true;
                    IsGrab = false;



                }
                //Snapright
                else if (IsGrab && App.mo_states.New.Position.X > App.Screenwidth - 2 && App.mo_states.IsLeftButtonToggleOff())
                {
                    size.Y = App.Screenheight - (UI_Handler.buttonheight + UI_Handler.sqarebuttonwidth) - 24;
                    size.X = minsize.X;

                    pos.X = App.Screenwidth - size.X;
                    pos.Y = UI_Handler.buttonheight + UI_Handler.sqarebuttonwidth;
                    Resize();
                    Snap = Snapright = true;
                    IsGrab = false;

                }
                //Toplimit
                else if (pos.Y < 0)
                {
                    pos.Y = 0;
                }
                //Snapbottom
                else if (IsGrab && App.mo_states.New.Position.Y > App.Screenheight - 24 - 2 && App.mo_states.IsLeftButtonToggleOff())
                {
                    size.X = App.Screenwidth;
                    size.Y = minsize.Y;

                    pos.X = 0;
                    pos.Y = App.Screenheight - size.Y - UI_Handler.LayerSelectHotbar.size.Y - 24;
                    Resize();
                    Snap = Snapbottom = true;
                    IsGrab = false;
                }
            }
            if (Snap)
            {
                if (IsGrab)
                {
                    Snap = Snapbottom = Snapleft = Snapright = false;
                    size = PreSnapSize;
                    Grabpos = new Point(size.X / 2, headheight / 2);
                    Resize();
                }


            }

            // Handling Draging of Window
            Rectangle Grabbox = new Rectangle(absolutpos, new Point(size.X - ui_elements[0].size.X - 4, ui_elements[0].size.Y + 4));
            if (Grabbox.Contains(App.mo_states.New.Position) && App.mo_states.IsLeftButtonToggleOn())
            {
                IsGrab = true;
                Grabpos = App.mo_states.New.Position - absolutpos;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Hand;
            }
            if (IsGrab && App.mo_states.IsLeftButtonToggleOff())
            {
                IsGrab = false;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }
            if (IsGrab)
            {
                UI_Handler.UI_AlreadyActivated = true;
                UI_Handler.UI_Active_State = 1;
                pos.pos = App.mo_states.New.Position - Grabpos;
            }

            

            absolutpos = pos.pos;

            int RSsize = 8;
            int RSsize2 = RSsize * 2;
            bool IsResizeHover = false;

            if (IsResizeable) //Resize
            {
                if (resize_type == 0)
                {
                    Rectangle Resize_Bottom_Right_box = new Rectangle(absolutpos + size - new Point(RSsize), new Point(RSsize2));
                    if (Resize_Bottom_Right_box.Contains(App.mo_states.New.Position) && resize_type == 0 && !IsResizeHover)
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNWSE;
                        IsResizeHover = true;
                        UI_Handler.UI_AlreadyActivated = true;
                        if (App.mo_states.IsLeftButtonToggleOn())
                            resize_type = 4;
                    }
                    Rectangle Resize_Bottom_Left_box = new Rectangle(absolutpos + new Point(-RSsize, size.Y - RSsize), new Point(RSsize2));
                    if (Resize_Bottom_Left_box.Contains(App.mo_states.New.Position) && resize_type == 0 && !IsResizeHover)
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNESW;
                        IsResizeHover = true;
                        UI_Handler.UI_AlreadyActivated = true;
                        if (App.mo_states.IsLeftButtonToggleOn())
                        {
                            oldrightborderpos = new Point(pos.X + size.X, 0);
                            resize_type = 5;
                        }
                    }
                    Rectangle Resize_bottom_box = new Rectangle(absolutpos + new Point(0, size.Y - RSsize), new Point(size.X, RSsize2));
                    if (Resize_bottom_box.Contains(App.mo_states.New.Position) && resize_type == 0 && !IsResizeHover)
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeNS;
                        IsResizeHover = true;
                        UI_Handler.UI_AlreadyActivated = true;
                        if (App.mo_states.IsLeftButtonToggleOn())
                            resize_type = 1;
                    }
                    Rectangle Resize_right_box = new Rectangle(absolutpos + new Point(size.X - RSsize, headheight), new Point(RSsize2, size.Y - headheight));
                    if (Resize_right_box.Contains(App.mo_states.New.Position) && resize_type == 0 && !IsResizeHover)
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;
                        IsResizeHover = true;
                        UI_Handler.UI_AlreadyActivated = true;
                        if (App.mo_states.IsLeftButtonToggleOn())
                            resize_type = 2;
                    }
                    Rectangle Resize_left_box = new Rectangle(absolutpos + new Point(-RSsize, headheight), new Point(RSsize2, size.Y - headheight));
                    if (Resize_left_box.Contains(App.mo_states.New.Position) && resize_type == 0 && !IsResizeHover)
                    {
                        System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.SizeWE;
                        IsResizeHover = true;
                        UI_Handler.UI_AlreadyActivated = true;
                        if (App.mo_states.IsLeftButtonToggleOn())
                        {
                            oldrightborderpos = new Point(pos.X + size.X, 0);
                            resize_type = 3;
                        }
                    }
                }

                if (resize_type != 0)
                {
                    UI_Handler.UI_AlreadyActivated = true;
                    UI_Handler.UI_Active_State = 1;
                    if (App.mo_states.IsLeftButtonToggleOff())
                    {
                        resize_type = 0;
                    }
                    switch (resize_type)
                    {
                        case 1: // Bottom Resize
                            size.Y = App.mo_states.New.Position.Y - absolutpos.Y;
                            if (size.Y <= minsize.Y)
                                size.Y = minsize.Y;
                            break;
                        case 2: // Right Resize
                            size.X = App.mo_states.New.Position.X - absolutpos.X;
                            if (size.X <= minsize.X)
                                size.X = minsize.X;
                            break;
                        case 3: // Left Resize
                            size.X = oldrightborderpos.X - App.mo_states.New.Position.X;
                            if (size.X <= minsize.X)
                                size.X = minsize.X;
                            pos.X = oldrightborderpos.X - size.X;
                            absolutpos.X = oldrightborderpos.X - size.X;
                            break;
                        case 4: // Bottom Right Resize
                            size.X = App.mo_states.New.Position.X - absolutpos.X;
                            size.Y = App.mo_states.New.Position.Y - absolutpos.Y;
                            if (size.X <= minsize.X)
                                size.X = minsize.X;
                            if (size.Y <= minsize.Y)
                                size.Y = minsize.Y;
                            break;
                        case 5: // Bottom Left Resize
                            size.X = oldrightborderpos.X - App.mo_states.New.Position.X;
                            size.Y = App.mo_states.New.Position.Y - absolutpos.Y;
                            if (size.X <= minsize.X)
                                size.X = minsize.X;
                            if (size.Y <= minsize.Y)
                                size.Y = minsize.Y;
                            pos.X = oldrightborderpos.X - size.X;
                            absolutpos.X = oldrightborderpos.X - size.X;
                            break;
                    }
                    Resize();
                }
            }
            base.UpdateSpecific();

        }

        protected override void UpdateAlways()
        {
            if (resize_type != 0 && App.mo_states.IsLeftButtonToggleOff())
            {
                resize_type = 0;
            }
            base.UpdateAlways();
        }

        public float PointRectDist(Point p, Rectangle rec)
        {
            var cx = Math.Max(Math.Min(p.X, rec.X + rec.Width), rec.X);
            var cy = Math.Max(Math.Min(p.Y, rec.Y + rec.Height), rec.Y);
            return (float)Math.Sqrt((p.X - cx) * (p.X - cx) + (p.Y - cy) * (p.Y - cy));
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            float trans = 1.0f;
            if (UI_Handler.UI_IsWindowHide)
            {
                float closestdist = PointRectDist(App.mo_states.New.Position, new Rectangle(pos.pos, size));
                trans = MathHelper.Clamp(0.0f + closestdist * 0.0035f, 0.15f, 0.5f);
            }
            spritebatch.End();


            App.graphics.GraphicsDevice.SetRenderTarget(target);
            Matrix matrix = Matrix.CreateTranslation(new Vector3(new Vector2(-absolutpos.X, -absolutpos.Y), 0));
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);
            App.Render_PreviousMatrix[App.Render_PreviousMatrix_Index] = matrix;
            App.Render_PreviousMatrix_Index++;
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, size), BackgroundColor);
            spritebatch.DrawFilledRectangle(new Rectangle(absolutpos, new Point(size.X, headheight)), BorderColor); //Chartreuse Best Color
            spritebatch.DrawString(conf.font, Title, absolutpos.ToVector2() + Title_pos, conf.font_color);

            base.DrawSpecific(spritebatch);

            spritebatch.DrawHollowRectangle(new Rectangle(absolutpos, size), BorderColor, 1);
            spritebatch.End();

            App.graphics.GraphicsDevice.SetRenderTarget(null);
            App.Render_PreviousMatrix_Index--;

            spritebatch.Begin();
            spritebatch.Draw(target, new Rectangle(absolutpos, size), new Rectangle(Point.Zero, size), Color.White * trans);

        }
    }
}
