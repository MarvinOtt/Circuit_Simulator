using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_Configs;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_GridPaint : UI_Element
    {
        public int GridSize, zoom;
        public int currot;
        Vector2 worldpos;
        public Point Origin, minmaxzoom;
        Effect effect;
        Texture2D tex, logictex;
        byte[] data;
        public byte[,] ledsegment_IDs;
        public bool DenyInteraction = false;
        public List<ComponentPixel> pixel;
        public int curplacetype = 0;
        Generic_Conf conf;
        public List<byte> ledsegmentpixel;
        public List<Point> ledsegmentpixel_pos;
        List<Vector2> pinpositions;
        List<Vector2> ledsegmentpositions;

        public delegate void PixelChanged_Handler();
        public event PixelChanged_Handler PixelChanged = delegate { };

        private RasterizerState _rasterizerState = new RasterizerState() { ScissorTestEnable = true };

        public UI_GridPaint(Pos pos, Point size, int GridSize, Point Origin, Point minmaxzoom, Generic_Conf conf) : base(pos, size)
        {
            ledsegment_IDs = new byte[GridSize, GridSize];
            this.conf = conf;
            this.GridSize = GridSize;
            this.Origin = Origin;
            this.minmaxzoom = minmaxzoom;
            ledsegmentpixel = new List<byte>();
            ledsegmentpixel_pos = new List<Point>();
            pinpositions = new List<Vector2>();
            ledsegmentpositions = new List<Vector2>();
            pixel = new List<ComponentPixel>();
            tex = new Texture2D(App.graphics.GraphicsDevice, size.X, size.Y);
            logictex = new Texture2D(App.graphics.GraphicsDevice, GridSize, GridSize, false, SurfaceFormat.Alpha8);
            data = new byte[logictex.Width * logictex.Height];
            effect = App.content.Load<Effect>("UI\\GridPaint_effect");
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
            Array.Clear(ledsegment_IDs, 0, GridSize * GridSize);
            Array.Clear(data, 0, data.Length);
            for(int i = 0; i < pixel.Count; ++i)
            {
                data[(pixel[i].pos.X + Origin.X) + (pixel[i].pos.Y + Origin.Y) * logictex.Width] = pixel[i].type;
            }
            for(int i = 0; i < ledsegmentpixel.Count; ++i)
            {
                ledsegment_IDs[ledsegmentpixel_pos[i].X, ledsegmentpixel_pos[i].Y] = ledsegmentpixel[i];
            }
            logictex.SetData(data);
            UpdatePinTextPos();
            PixelChanged();
        }

        public void UpdatePinTextPos()
        {
            pinpositions.Clear();
            ledsegmentpositions.Clear();
            for (int i = 0; i < pixel.Count; ++i)
            {
                if (pixel[i].type > Sim_Component.PINOFFSET)
                {
                    Vector2 cur = new Vector2((float)Math.Pow(2, zoom) / 2.0f) - (conf.font.MeasureString((pixel[i].type - (Sim_Component.PINOFFSET + 1)).ToString()) / 2.0f);
                    pinpositions.Add(cur);
                }
                else if(pixel[i].type == 4)
                {
                    Vector2 cur = new Vector2((float)Math.Pow(2, zoom) / 2.0f) - (conf.font.MeasureString(ledsegment_IDs[pixel[i].pos.X + Origin.X, pixel[i].pos.Y + Origin.Y].ToString()) / 2.0f);
                    ledsegmentpositions.Add(cur);
                }
            }
        }

        public override void UpdateSpecific()
        {
            if (new Rectangle(pos.pos, size).Contains(App.mo_states.New.Position) && !DenyInteraction && !(UI_Handler.IsInScrollable && !UI_Handler.IsInScrollable_Bounds.Contains(App.mo_states.New.Position)))
            {
                #region Position and Zoom

                if (App.kb_states.New.IsKeyDown(Keys.W))
                    worldpos.Y += 10;
                if (App.kb_states.New.IsKeyDown(Keys.S))
                    worldpos.Y -= 10;
                if (App.kb_states.New.IsKeyDown(Keys.A))
                    worldpos.X += 10;
                if (App.kb_states.New.IsKeyDown(Keys.D))
                    worldpos.X -= 10;

                worldpos.X = (int)(worldpos.X);
                worldpos.Y = (int)(worldpos.Y);

                if (App.mo_states.New.ScrollWheelValue != App.mo_states.Old.ScrollWheelValue)
                {
                    if (App.mo_states.New.ScrollWheelValue < App.mo_states.Old.ScrollWheelValue && zoom > minmaxzoom.X) // Zooming Out
                    {
                        zoom -= 1;
                        Vector2 diff = worldpos - (App.mo_states.New.Position.ToVector2() - pos.pos.ToVector2());
                        worldpos = (App.mo_states.New.Position.ToVector2() - pos.pos.ToVector2()) + diff / 2;
                        UpdatePinTextPos();
                    }
                    else if (App.mo_states.New.ScrollWheelValue > App.mo_states.Old.ScrollWheelValue && zoom < minmaxzoom.Y) // Zooming In
                    {
                        zoom += 1;
                        Vector2 diff = worldpos - (App.mo_states.New.Position.ToVector2() - pos.pos.ToVector2());
                        worldpos += diff;
                        UpdatePinTextPos();
                    }
                }
                int mouse_worldpos_X, mouse_worldpos_Y;
                Screen2worldcoo_int(App.mo_states.New.Position.ToVector2() - absolutpos.ToVector2(), out mouse_worldpos_X, out mouse_worldpos_Y);
                if (mouse_worldpos_X >= 0 && mouse_worldpos_X < GridSize && mouse_worldpos_Y >= 0 && mouse_worldpos_Y < GridSize && currot == 0 && !UI_EditComp_Window.IsInOverlayMode)
                {
                    if (App.mo_states.New.LeftButton == ButtonState.Pressed)
                    {
                        // Place Pixel
                        if (pixel.Exists(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y))
                        {
                            int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                            ComponentPixel curtype = pixel[index];
                            if (MathHelper.Clamp(curtype.type, 0, (Sim_Component.PINOFFSET + 1)) != curplacetype)
                            {
                                int type = curplacetype;
                                if(curtype.type == 4)
                                {
                                    int index2 = ledsegmentpixel_pos.FindIndex(x => x.X == (mouse_worldpos_X) && x.Y == (mouse_worldpos_Y));
                                    ledsegmentpixel_pos.RemoveAt(index2);
                                    ledsegmentpixel.RemoveAt(index2);
                                }
                                pixel[index] = new ComponentPixel(new Point(mouse_worldpos_X - Origin.X, mouse_worldpos_Y - Origin.Y), (byte)type);
                                if(type == 4)
                                {
                                    ledsegmentpixel.Add(0);
                                    ledsegmentpixel_pos.Add(new Point(mouse_worldpos_X, mouse_worldpos_Y));
                                }
                                ApplyPixel();
                            }
                        }
                        else
                        {
                            int type = curplacetype;
                            pixel.Add(new ComponentPixel(new Point(mouse_worldpos_X - Origin.X, mouse_worldpos_Y - Origin.Y), (byte)type));
                            if (type == 4)
                            {
                                ledsegmentpixel.Add(0);
                                ledsegmentpixel_pos.Add(new Point(mouse_worldpos_X, mouse_worldpos_Y));
                            }
                            ApplyPixel();
                        }
                        if (curplacetype == 5)
                        {
                            for (int i = 0; i < 10; ++i)
                            {
                                if (App.kb_states.IsKeyToggleDown(Keys.D0 + i))
                                {
                                    int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                    ComponentPixel cur = pixel[index];
                                    if (cur.type > Sim_Component.PINOFFSET)
                                    {
                                        int val = cur.type - (Sim_Component.PINOFFSET + 1);
                                        if (val == 0 && i != 0)
                                            pixel[index] = new ComponentPixel(cur.pos, (byte)(i + Sim_Component.PINOFFSET + 1));
                                        else if (val != 0)
                                            pixel[index] = new ComponentPixel(cur.pos, (byte)(val * 10 + i + Sim_Component.PINOFFSET + 1));
                                        ApplyPixel();
                                    }
                                }
                            }
                            if (App.kb_states.IsKeyToggleDown(Keys.Back))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type > Sim_Component.PINOFFSET)
                                {
                                    pixel[index] = new ComponentPixel(pixel[index].pos, (byte)((pixel[index].type - (Sim_Component.PINOFFSET + 1)) / 10 + (Sim_Component.PINOFFSET + 1)));
                                    ApplyPixel();
                                }
                            }
                            else if (App.kb_states.IsKeyToggleDown(Keys.Escape))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type > Sim_Component.PINOFFSET)
                                {
                                    pixel[index] = new ComponentPixel(pixel[index].pos, (byte)(Sim_Component.PINOFFSET + 1));
                                    ApplyPixel();
                                }
                            }
                        }
                        if (curplacetype == 4)
                        {
                            for (int i = 0; i < 10; ++i)
                            {
                                if (App.kb_states.IsKeyToggleDown(Keys.D0 + i))
                                {
                                    int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                    ComponentPixel cur = pixel[index];
                                    if (cur.type == 4)
                                    {
                                        int val = ledsegment_IDs[mouse_worldpos_X, mouse_worldpos_Y];// cur.type - (Sim_Component.PINOFFSET + 1);
                                        int index2 = ledsegmentpixel_pos.FindIndex(x => x.X == mouse_worldpos_X && x.Y == mouse_worldpos_Y);
                                        ledsegmentpixel[index2] = (byte)(val * 10 + i);
                                        //ledsegment_IDs[mouse_worldpos_X, mouse_worldpos_Y] = (byte)(val * 10 + i);
                                        //if (val == 0 && i != 0)
                                        //    pixel[index] = new ComponentPixel(cur.pos, (byte)i);
                                        //else if (val != 0)
                                        //    pixel[index] = new ComponentPixel(cur.pos, (byte)(val * 10 + i));
                                        ApplyPixel();
                                    }
                                }
                            }
                            if (App.kb_states.IsKeyToggleDown(Keys.Back))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type == 4)
                                {
                                    int index2 = ledsegmentpixel_pos.FindIndex(x => x.X == mouse_worldpos_X && x.Y == mouse_worldpos_Y);
                                    ledsegmentpixel[index2] /= 10;
                                    ApplyPixel();
                                }
                            }
                            else if (App.kb_states.IsKeyToggleDown(Keys.Escape))
                            {
                                int index = pixel.FindIndex(x => x.pos.X == mouse_worldpos_X - Origin.X && x.pos.Y == mouse_worldpos_Y - Origin.Y);
                                if (pixel[index].type == 4)
                                {
                                    int index2 = ledsegmentpixel_pos.FindIndex(x => x.X == mouse_worldpos_X && x.Y == mouse_worldpos_Y);
                                    ledsegmentpixel[index2] = 0;
                                    ApplyPixel();
                                }
                            }
                        }
                    }
                    if (App.mo_states.New.RightButton == ButtonState.Pressed)
                    {
                        int index = pixel.FindIndex(x => x.pos.X == (mouse_worldpos_X - Origin.X) && x.pos.Y == (mouse_worldpos_Y - Origin.Y));
                        if (index >= 0)
                            pixel.RemoveAt(index);
                        index = ledsegmentpixel_pos.FindIndex(x => x.X == (mouse_worldpos_X) && x.Y == (mouse_worldpos_Y));
                        if (index >= 0)
                        {
                            ledsegmentpixel_pos.RemoveAt(index);
                            ledsegmentpixel.RemoveAt(index);
                        }
                        ApplyPixel();
                    }
                }
                else if(UI_EditComp_Window.IsInOverlayMode)
                {
                    CompData compdata = UI_EditComp_Window.rootcomp;
                    Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText) * compdata.OverlayTextSize[currot] * (float)Math.Pow(2, zoom);
                    Vector2 pos = absolutpos.ToVector2() + new Vector2((compdata.OverlayTextPos[currot].X + Origin.X + 0.5f) * (float)Math.Pow(2, zoom) + worldpos.X, (compdata.OverlayTextPos[currot].Y + Origin.Y + 0.5f) * (float)Math.Pow(2, zoom) + worldpos.Y) - size / 2;
                    if (App.mo_states.New.LeftButton == ButtonState.Pressed && App.kb_states.New.IsKeyDown(Keys.LeftControl))
                    {
                        compdata.OverlayTextSize[currot] += (((float)(App.mo_states.New.Position.Y - App.mo_states.Old.Position.Y)) / (float)Math.Pow(2, zoom)) * 0.01f;
                    }
                    else if ((new Rectangle(pos.ToPoint(), size.ToPoint())).Contains(App.mo_states.New.Position) && App.mo_states.New.LeftButton == ButtonState.Pressed)
                    {
                        compdata.OverlayTextPos[currot] += (App.mo_states.New.Position - App.mo_states.Old.Position).ToVector2() / (float)Math.Pow(2, zoom);
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
            if (App.Render_PreviousMatrix_Index > 0)
                matrix = App.Render_PreviousMatrix[App.Render_PreviousMatrix_Index - 1];
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, effect, matrix);
            spritebatch.Draw(tex, pos.pos.ToVector2(), Color.White);

            spritebatch.End();
            spritebatch.GraphicsDevice.ScissorRectangle = new Rectangle(absolutpos - parent.absolutpos, size);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, _rasterizerState, null, matrix);
            int pincount = 0, ledsegmentcount = 0;
            if (zoom > 3)
            {
                for (int i = 0; i < pixel.Count; ++i)
                {
                    if (pixel[i].type > Sim_Component.PINOFFSET)
                    {
                        spritebatch.DrawString(conf.font, (pixel[i].type - (Sim_Component.PINOFFSET + 1)).ToString(), absolutpos.ToVector2() + new Vector2((pixel[i].pos.X + Origin.X) * (float)Math.Pow(2, zoom) + worldpos.X + pinpositions[pincount].X, (pixel[i].pos.Y + Origin.Y) * (float)Math.Pow(2, zoom) + worldpos.Y + pinpositions[pincount].Y), Color.Red);
                        pincount++;
                    }
                    else if(pixel[i].type == 4)
                    {
                        spritebatch.DrawString(conf.font, ledsegment_IDs[pixel[i].pos.X + Origin.X, pixel[i].pos.Y + Origin.Y].ToString(), absolutpos.ToVector2() + new Vector2((pixel[i].pos.X + Origin.X) * (float)Math.Pow(2, zoom) + worldpos.X + ledsegmentpositions[ledsegmentcount].X, (pixel[i].pos.Y + Origin.Y) * (float)Math.Pow(2, zoom) + worldpos.Y + ledsegmentpositions[ledsegmentcount].Y), Color.Red);
                        ledsegmentcount++;
                    }
                }
            }
            if(UI_EditComp_Window.rootcomp.OverlayText.Length > 0)
            {
                CompData compdata = UI_EditComp_Window.rootcomp;
                Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText);
                Vector2 pos = absolutpos.ToVector2() + new Vector2((compdata.OverlayTextPos[currot].X + Origin.X + 0.5f) * (float)Math.Pow(2, zoom) + worldpos.X, (compdata.OverlayTextPos[currot].Y + Origin.Y + 0.5f) * (float)Math.Pow(2, zoom) + worldpos.Y);
                spritebatch.DrawString(CompData.overlayfont, compdata.OverlayText, pos, Color.Black, 0, size / 2, compdata.OverlayTextSize[currot] * (float)Math.Pow(2, zoom), SpriteEffects.None, 0);
                if(UI_EditComp_Window.IsInOverlayMode)
                    spritebatch.DrawHollowRectangle(new Rectangle((pos - (size * compdata.OverlayTextSize[currot] * (float)Math.Pow(2, zoom)) / 2).ToPoint(), (size * compdata.OverlayTextSize[currot] * (float)Math.Pow(2, zoom)).ToPoint()), Color.Blue, 2);
            }
            spritebatch.End();
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, matrix);

            base.DrawSpecific(spritebatch);
        }
    }
}
