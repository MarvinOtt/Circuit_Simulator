using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_GridPaint : UI_Element
    {
        int GridSize, zoom;
        public int currot;
        Vector2 worldpos;
        Point Origin, minmaxzoom;
        Effect effect;
        Texture2D tex, logictex;
        byte[] data;
        public bool DenyInteraction = false;
        public List<ComponentPixel> pixel;
        public int curplacetype = 0;
        Generic_Conf conf;
        List<Vector2> pinpositions;

        public delegate void PixelChanged_Handler();
        public event PixelChanged_Handler PixelChanged = delegate { };

        private RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };

        public UI_GridPaint(Pos pos, Point size, int GridSize, Point Origin, Point minmaxzoom, Generic_Conf conf) : base(pos, size)
        {
            this.conf = conf;
            this.GridSize = GridSize;
            this.Origin = Origin;
            this.minmaxzoom = minmaxzoom;
            pinpositions = new List<Vector2>();
            pixel = new List<ComponentPixel>();
            tex = new Texture2D(Game1.graphics.GraphicsDevice, size.X, size.Y);
            logictex = new Texture2D(Game1.graphics.GraphicsDevice, GridSize, GridSize, false, SurfaceFormat.Alpha8);
            data = new byte[logictex.Width * logictex.Height];
            effect = Game1.content.Load<Effect>("UI\\GridPaint_effect");
            effect.Parameters["worldsizex"].SetValue(GridSize);
            effect.Parameters["worldsizey"].SetValue(GridSize);
            effect.Parameters["Screenwidth"].SetValue(size.X);
            effect.Parameters["Screenheight"].SetValue(size.Y);
            effect.Parameters["origin_X"].SetValue(Origin.X);
            effect.Parameters["origin_Y"].SetValue(Origin.Y);

            zoom = 4;
            worldpos.X = (size.X / 2) - (GridSize / 2 + 0.5f) * (float)Math.Pow(2, zoom);
            worldpos.Y = (size.Y / 2) - (GridSize / 2 + 0.5f) * (float)Math.Pow(2, zoom);
        }

        public void Screen2worldcoo_int(Vector2 screencoos, out int x, out int y)
        {
            x = (int)((screencoos.X - worldpos.X) / (float)Math.Pow(2, zoom));
            y = (int)((screencoos.Y - worldpos.Y) / (float)Math.Pow(2, zoom));
        }

        public void ApplyPixel()
        {
            //int count = 0;
            //for (int i = 0; i < pixel.Count; ++i)
            //{
            //    if (pixel[i].type > 3)
            //    {
            //        pixel[i] = new ComponentPixel(pixel[i].pos, (byte)(4 + count));
            //        count++;
            //    }
            //}
            Array.Clear(data, 0, data.Length);
            for(int i = 0; i < pixel.Count; ++i)
            {
                data[(pixel[i].pos.X + Origin.X) + (pixel[i].pos.Y + Origin.Y) * logictex.Width] = pixel[i].type;
            }
            logictex.SetData(data);
            UpdatePinTextPos();
            PixelChanged();
        }

        public void UpdatePinTextPos()
        {
            pinpositions.Clear();
            for (int i = 0; i < pixel.Count; ++i)
            {
                if (pixel[i].type > 3)
                {
                    Vector2 cur = new Vector2((float)Math.Pow(2, zoom) / 2.0f) - (conf.font.MeasureString((pixel[i].type - 4).ToString()) / 2.0f);
                    pinpositions.Add(cur);
                }
            }
        }

        public override void UpdateSpecific()
        {
            if (new Rectangle(pos.pos, size).Contains(Game1.mo_states.New.Position) && !DenyInteraction && !(UI_Handler.IsInScrollable && !UI_Handler.IsInScrollable_Bounds.Contains(Game1.mo_states.New.Position)))
            {
                #region Position and Zoom

                if (Game1.kb_states.New.IsKeyDown(Keys.W))
                    worldpos.Y += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.S))
                    worldpos.Y -= 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.A))
                    worldpos.X += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.D))
                    worldpos.X -= 10;

                worldpos.X = (int)(worldpos.X);
                worldpos.Y = (int)(worldpos.Y);

                if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
                {
                    if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue && zoom > minmaxzoom.X) // Zooming Out
                    {
                        zoom -= 1;
                        Vector2 diff = worldpos - (Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2());
                        worldpos = (Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2()) + diff / 2;
                        UpdatePinTextPos();
                    }
                    else if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue && zoom < minmaxzoom.Y) // Zooming In
                    {
                        zoom += 1;
                        Vector2 diff = worldpos - (Game1.mo_states.New.Position.ToVector2() - pos.pos.ToVector2());
                        worldpos += diff;
                        UpdatePinTextPos();
                    }
                }
                int mouse_worldpos_X, mouse_worldpos_Y;
                Screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2() - absolutpos.ToVector2(), out mouse_worldpos_X, out mouse_worldpos_Y);
                if (mouse_worldpos_X >= 0 && mouse_worldpos_X < GridSize && mouse_worldpos_Y >= 0 && mouse_worldpos_Y < GridSize)
                {
                    if (Game1.mo_states.New.LeftButton == ButtonState.Pressed)
                    {
                        // Place Pixel
                        if (pixel.Exists(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y))
                        {
                            int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                            ComponentPixel curtype = pixel[index];
                            if (MathHelper.Clamp(curtype.type, 0, 4) != curplacetype)
                            {
                                int type = curplacetype;
                                pixel[index] = new ComponentPixel(new Point(mouse_worldpos_X - Origin.X, mouse_worldpos_Y - Origin.Y), (byte)type);
                                ApplyPixel();
                            }
                        }
                        else
                        {
                            int type = curplacetype;
                            pixel.Add(new ComponentPixel(new Point(mouse_worldpos_X - Origin.X, mouse_worldpos_Y - Origin.Y), (byte)type));
                            ApplyPixel();
                        }
                        if (curplacetype == 4)
                        {
                            for (int i = 0; i < 10; ++i)
                            {
                                if (Game1.kb_states.IsKeyToggleDown(Keys.D0 + i))
                                {
                                    int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                    ComponentPixel cur = pixel[index];
                                    if (cur.type > 3)
                                    {
                                        int val = cur.type - 4;
                                        if (val == 0 && i != 0)
                                            pixel[index] = new ComponentPixel(cur.pos, (byte)(i + 4));
                                        else if (val != 0)
                                            pixel[index] = new ComponentPixel(cur.pos, (byte)(val * 10 + i + 4));
                                        ApplyPixel();
                                    }
                                }
                            }
                            if (Game1.kb_states.IsKeyToggleDown(Keys.Back))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type > 3)
                                {
                                    pixel[index] = new ComponentPixel(pixel[index].pos, (byte)((pixel[index].type - 4) / 10 + 4));
                                    ApplyPixel();
                                }
                            }
                            else if (Game1.kb_states.IsKeyToggleDown(Keys.Escape))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type > 3)
                                {
                                    pixel[index] = new ComponentPixel(pixel[index].pos, 4);
                                    ApplyPixel();
                                }
                            }
                        }
                    }
                    if (Game1.mo_states.New.RightButton == ButtonState.Pressed)
                    {
                        int index = pixel.FindIndex(x => x.pos.X == (mouse_worldpos_X - Origin.X) && x.pos.Y == (mouse_worldpos_Y - Origin.Y));
                        if (index >= 0)
                            pixel.RemoveAt(index);
                        ApplyPixel();
                    }
                }

                #endregion

                effect.Parameters["mousepos_X"].SetValue(mouse_worldpos_X);
                effect.Parameters["mousepos_Y"].SetValue(mouse_worldpos_Y);
            }
            effect.Parameters["zoom"].SetValue((float)Math.Pow(2, zoom));
            effect.Parameters["coos"].SetValue(worldpos);
            base.UpdateSpecific();
        }

        protected override void DrawSpecific(SpriteBatch spritebatch)
        {
            spritebatch.End();
            effect.Parameters["logictex"].SetValue(logictex);

            Matrix matrix = Matrix.Identity;
            if (Game1.Render_PreviousMatrix_Index > 0)
                matrix = Game1.Render_PreviousMatrix[Game1.Render_PreviousMatrix_Index - 1];
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect, matrix);
            spritebatch.Draw(tex, pos.pos.ToVector2(), Color.White);

            spritebatch.End();
            spritebatch.GraphicsDevice.ScissorRectangle = new Rectangle(absolutpos - parent.absolutpos, size);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, _rasterizerState, null, matrix);
            int count = 0;
            if (zoom > 3)
            {
                for (int i = 0; i < pixel.Count; ++i)
                {
                    if (pixel[i].type > 3)
                    {
                        spritebatch.DrawString(conf.font, (pixel[i].type - 4).ToString(), absolutpos.ToVector2() + new Vector2((pixel[i].pos.X + Origin.X) * (float)Math.Pow(2, zoom) + worldpos.X + pinpositions[count].X, (pixel[i].pos.Y + Origin.Y) * (float)Math.Pow(2, zoom) + worldpos.Y + pinpositions[count].Y), Color.Red);
                        count++;
                    }
                }
            }
            spritebatch.End();
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

            base.DrawSpecific(spritebatch);
        }
    }
}
