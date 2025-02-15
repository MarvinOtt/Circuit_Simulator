﻿using Circuit_Simulator.COMP;
using Circuit_Simulator.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public struct FRectangle
    {
        public float X, Y, Width, Height;
        public FRectangle(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
            
        }
        public FRectangle(Vector2 XY, Vector2 size)
        {
            this.X = XY.X;
            this.Y = XY.Y;
            this.Width = size.X;
            this.Height = size.Y;
        }

        public Rectangle ToRectangle()
        {
            return new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
        }

        public static FRectangle operator * (FRectangle src, float mul)
        {
            return new FRectangle(src.X * mul, src.Y * mul, src.Width * mul, src.Height * mul);
        }
    }

    public struct ComponentPixel
    {
        public byte type;
        public Point pos;
        public byte IsEdge;

        public ComponentPixel(Point pos, byte type)
        {
            this.pos = pos;
            this.type = type;
            IsEdge = 0;
        }
        public ComponentPixel(Point pos, byte type, byte IsEdge)
        {
            this.pos = pos;
            this.type = type;
            this.IsEdge = IsEdge;
        }
    }
   
    public class Sim_Component
    {
        public static int PINOFFSET = 4;

        Simulator sim;
        Effect sim_effect;
        Texture2D placementtex;
        public static RenderTarget2D Comp_target, IsEdge_target, Highlight_target;
        public bool IsCompDrag;
        public static VertexPositionLine[] highlight_vertices;
        public static int curhighlightID = 0;

        public static List<CompData> Components_Data;
        public static Component[] components;
        public static int[] emptyComponentID;
        public static int emptyComponentID_count;
        public static List<int> CompMayneedoverlay;
        public static int nextComponentID = 1;
        public static byte[,] CompType;
        public static int[,][] CompGrid;
		public static List<int>[,] CompOverlayGrid;
		public static List<int>[,] PinDescGrid;
		public static byte[,] CompNetwork;
        public static Point[] pins2check;
        public static int pins2check_length;
        public static VertexPositionLine[] overlaylines;
        public static bool DropComponent;
        public static Effect overlay_effect, highlight_effect;

        public Sim_Component(Simulator sim, Effect sim_effect)
        {
            this.sim = sim;
            this.sim_effect = sim_effect;
            overlay_effect = App.content.Load<Effect>("overlay_effect");
            highlight_effect = App.content.Load<Effect>("UI\\highlight_effect");
            placementtex = new Texture2D(App.graphics.GraphicsDevice, 81, 81, false, SurfaceFormat.HalfSingle);
            Comp_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Highlight_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            IsEdge_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            CompType = new byte[Simulator.SIZEX, Simulator.SIZEY];
            CompGrid = new int[Simulator.SIZEX / 32, Simulator.SIZEY / 32][];
			CompOverlayGrid = new List<int>[Simulator.SIZEX / 32, Simulator.SIZEY / 32];
			PinDescGrid = new List<int>[Simulator.SIZEX / 32, Simulator.SIZEY / 32];
			CompNetwork = new byte[Simulator.SIZEX, Simulator.SIZEY];
            components = new Component[10000000];
            pins2check = new Point[20000000];
            overlaylines = new VertexPositionLine[1000000];
            emptyComponentID = new int[10000000];
            CompMayneedoverlay = new List<int>();
            Components_Data = new List<CompData>();
            string[] Libraries2Load = Directory.GetFiles(@"LIBRARIES\", "*.dcl");
            Sim_INF_DLL.LoadLibrarys(Libraries2Load);
			Sim_INF_DLL.SimFrameStep_maxperframe += DrawAllHighlights;
          
        }

        public void InizializeComponentDrag(int ID)
        {
            if (true)
            {
                UI_Handler.UI_IsWindowHide = true;
              
                App.simulator.ChangeToolmode(Simulator.TOOL_SELECT);
                IsCompDrag = true;
                HalfSingle[] data = new HalfSingle[81 * 81];
                List<ComponentPixel> datapixel = Components_Data[ID].data[Components_Data[ID].currentrotation];
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 40) * 81 + (datapixel[i].pos.X + 40)] = new HalfSingle((float)datapixel[i].type);
                }

                placementtex.SetData(data);
                sim_effect.Parameters["placementtex"].SetValue(placementtex);
            }
        }

        public void IsDrag()
        {
            if (UI_Handler.UI_Active_State == UI_Handler.UI_Active_CompDrag)
            {
                sim_effect.Parameters["currenttype"].SetValue(1);

                // Rotating Component clock-wise
                if (App.kb_states.IsKeyToggleDown(Keys.R))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_ROT[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
                if (App.kb_states.IsKeyToggleDown(Keys.X))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_FLIPX[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
                if (App.kb_states.IsKeyToggleDown(Keys.Y))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_FLIPY[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
            }
            else
                sim_effect.Parameters["currenttype"].SetValue(0);

            if (App.kb_states.IsKeyToggleDown(Keys.Escape) || App.mo_states.IsRightButtonToggleOff())
            {
                UI_Handler.dragcomp.GetsDrawn = false;
                UI_Handler.dragcomp.GetsUpdated = false;
                UI_Handler.ZaWarudo = null;
                DeactivateDrop();
            }
           
        }
        public void ComponentDrop(int dataID)
        {
            Point pos = Point.Zero;
            Simulator.Screen2worldcoo_int(App.mo_states.New.Position.ToVector2(), out pos.X, out pos.Y);
            ComponentDropAtPos(dataID, pos);
        }
        public static void ComponentDropAtPos(int dataID, Point pos)
        {
            ComponentDropAtPos(dataID, pos, Components_Data[dataID].currentrotation);
        }
        public static Component ComponentDropAtPos(int dataID, Point pos, byte rotation)
        {
            Component newcomp = null;
            if (Component.IsValidPlacement(dataID, pos, rotation))
            {
                FileHandler.IsUpToDate = false;
                if (emptyComponentID_count > 0)
                    newcomp = new Component(dataID, emptyComponentID[--emptyComponentID_count]);
                else
                    newcomp = new Component(dataID, nextComponentID++);
                components[newcomp.ID] = newcomp;
                newcomp.Place(pos, rotation);
            }
            return newcomp;
        }

        public void DeactivateDrop()
        {
            UI_Handler.UI_IsWindowHide = false;
            IsCompDrag = false;
            sim_effect.Parameters["currenttype"].SetValue(0);
            App.simulator.ChangeToolmode(Simulator.oldtoolmode);
        }

        public static int GetComponentID(Point pos)
        {
            Point gridpos = new Point(pos.X / 32, pos.Y / 32);
            int gridid = CompNetwork[pos.X, pos.Y];
            int[] arr = CompGrid[gridpos.X, gridpos.Y];
            if (arr != null)
                return arr[gridid];
            else
                return -1;
        }

        public static void CheckPins()
        {
            if (pins2check_length > 0)
            {
                for (int i = 0; i < pins2check_length; ++i)
                {
                    Point pos = pins2check[i];
                    Component cur_comp = components[CompGrid[pos.X / 32, pos.Y / 32][CompNetwork[pos.X, pos.Y]]];
                   
                    cur_comp.CheckAndUpdatePins();

                }

                pins2check_length = 0;
            }
        }

        public void Update()
        {
            if(DropComponent)
            {
                DropComponent = false;
                ComponentDrop(UI_Handler.dragcomp.comp.ID);
            }

            if (IsCompDrag)
            {
                IsDrag();
            }
            CheckPins();
            
        }

        public void Draw(SpriteBatch spritebatch)
        {
			if (Simulator.IsPinDesc && Simulator.worldzoom > 4)
			{
				int topleftchunk_X, topleftchunk_Y;
				Simulator.Screen2worldcoo_int(Vector2.Zero, out topleftchunk_X, out topleftchunk_Y);
				int bottomrightchunk_X, bottomrightchunk_Y;
				Simulator.Screen2worldcoo_int(new Vector2(App.Screenwidth, App.Screenheight), out bottomrightchunk_X, out bottomrightchunk_Y);
				int offset = 20;
				topleftchunk_X -= offset;
				topleftchunk_Y -= offset;
				bottomrightchunk_X += offset;
				bottomrightchunk_Y += offset;

				topleftchunk_X = MathHelper.Clamp(topleftchunk_X / 32, 0, Simulator.SIZEX / 32 - 1);
				topleftchunk_Y = MathHelper.Clamp(topleftchunk_Y / 32, 0, Simulator.SIZEY / 32 - 1);
				bottomrightchunk_X = MathHelper.Clamp(bottomrightchunk_X / 32, 0, Simulator.SIZEX / 32 - 1);
				bottomrightchunk_Y = MathHelper.Clamp(bottomrightchunk_Y / 32, 0, Simulator.SIZEY / 32 - 1);
				for (int x = topleftchunk_X; x <= bottomrightchunk_X; ++x)
				{
					for (int y = topleftchunk_Y; y <= bottomrightchunk_Y; ++y)
					{
						if (PinDescGrid[x, y] != null)
						{
							for (int i = 0; i < PinDescGrid[x, y].Count; ++i)
							{
								int ID = PinDescGrid[x, y][i];
								CompData compdata = Components_Data[components[ID].dataID];
								for (int j = 0; j < compdata.pindesc.Length; ++j)
								{
									if (compdata.pindesc[j] != null)
									{
										Vector2 size = CompData.overlayfont.MeasureString(compdata.pindesc[j]);
										float maxsize = Math.Max(size.X, size.Y);
										float mul = 1.0f;
										if (maxsize > 140)
											mul = 140.0f / maxsize;
										Vector2 pos = new Vector2((components[ID].pos.ToVector2().X + compdata.pindescpos[components[ID].rotation, j].X + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.X, (components[ID].pos.ToVector2().Y + compdata.pindescpos[components[ID].rotation, j].Y + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.Y);
										spritebatch.DrawString(CompData.overlayfont, compdata.pindesc[j], pos, Color.Red, 0, size / 2, 0.0036f * mul * (float)Math.Pow(2, Simulator.worldzoom), SpriteEffects.None, 0);
									}
								}
								//if (compdata.OverlayText.Length > 0)
								//{
								//	Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText);
								//	Vector2 pos = new Vector2((components[ID].pos.ToVector2().X + compdata.OverlayTextPos[components[ID].rotation].X + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.X, (components[ID].pos.ToVector2().Y + compdata.OverlayTextPos[components[ID].rotation].Y + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.Y);
								//	spritebatch.DrawString(CompData.overlayfont, compdata.OverlayText, pos, Color.Black, 0, size / 2, compdata.OverlayTextSize[components[ID].rotation] * (float)Math.Pow(2, Simulator.worldzoom), SpriteEffects.None, 0);
								//}
							}
						}
					}
				}
			}
		}

		public static void ClearAllHeighlighting()
		{
			if (highlight_vertices != null)
			{
				App.graphics.GraphicsDevice.SetRenderTarget(Highlight_target);
				highlight_effect.Parameters["highlightvalue"].SetValue(0.0f);
				highlight_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
				highlight_effect.CurrentTechnique.Passes[0].Apply();
				App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, highlight_vertices, 0, highlight_vertices.Length / 2);
				App.graphics.GraphicsDevice.SetRenderTarget(null);
				highlight_vertices = null;
			}
		}

		public void DrawAllHighlights(object sender)
		{
			if (Simulator.IsAllHighlight)
			{
				if (highlight_vertices == null)
				{
					//Generate vertices
					highlight_vertices = new VertexPositionLine[Sim_INF_DLL.line_num * 2];
					int count = 0;
					for (int i = 1; i < Sim_INF_DLL.WireStates_count; ++i)
					{
						int state = Sim_INF_DLL.WireStates_OUT[i];
						int netID = Sim_INF_DLL.WireMapInv[i];
						for (int j = 0; j < Simulator.networks[netID].lines.Count; ++j)
						{
							VertexPositionLine l1, l2;
							Line line = new Line(Simulator.networks[netID].lines[j].start, Simulator.networks[netID].lines[j].end);
							line.Convert2LineVertices(state, out l1, out l2);
							highlight_vertices[count++] = l1;
							highlight_vertices[count++] = l2;
						}
					}
				}
				else
				{
					App.graphics.GraphicsDevice.SetRenderTarget(Highlight_target);
					highlight_effect.Parameters["highlightvalue"].SetValue(0.0f);
					highlight_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
					highlight_effect.CurrentTechnique.Passes[0].Apply();
					App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, highlight_vertices, 0, highlight_vertices.Length / 2);
					App.graphics.GraphicsDevice.SetRenderTarget(null);

					int count = 0;
					for (int i = 1; i < Sim_INF_DLL.WireStates_count; ++i)
					{
						int state = Sim_INF_DLL.WireStates_OUT[i];
						int netID = Sim_INF_DLL.WireMapInv[i];
						for (int j = 0; j < Simulator.networks[netID].lines.Count; ++j)
						{
							highlight_vertices[count++].layers = state;
							highlight_vertices[count++].layers = state;
						}
					}
				}
				App.graphics.GraphicsDevice.SetRenderTarget(Highlight_target);

				highlight_effect.Parameters["highlightvalue"].SetValue(1.0f);
				highlight_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
				highlight_effect.CurrentTechnique.Passes[1].Apply();
				App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, highlight_vertices, 0, highlight_vertices.Length / 2);

				App.graphics.GraphicsDevice.SetRenderTarget(null);
			}
		}

        public void DrawLineOverlays(SpriteBatch spritebatch)
        {
            int count = 0;
            for(int i = 0; i < CompMayneedoverlay.Count; ++i)
            {
                int compID = CompMayneedoverlay[i];
                CompData compdata = Components_Data[components[compID].dataID];
                int ovstateID = compdata.internalstate_length;
                for(int k = 0; k < compdata.OverlaySeg_length; ++k)
                {
                    int ovstate = Sim_INF_DLL.CompInfos[Sim_INF_DLL.IntStatesMap[compID] + ovstateID + k];//  components[compID].internalstates[ovstateID + k];
                    if (Simulator.cursimframe == 0)
                        ovstate = 0;
                    Component comp = components[compID];
                    List<VertexPositionLine> CompOverlaylines = Components_Data[comp.dataID].overlaylines_vertices[k][comp.rotation];
                    for (int j = 0; j < CompOverlaylines.Count; ++j)
                    {
                        overlaylines[count] = CompOverlaylines[j];
                        overlaylines[count].layers = 4 - ovstate;
                        overlaylines[count].Position += new Vector3(components[compID].pos.X, components[compID].pos.Y, 0);
                        count++;
                    }
                }
                
            }
            
            if(count > 0)
            {
                App.graphics.GraphicsDevice.SetRenderTarget(Comp_target);
                overlay_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
                overlay_effect.CurrentTechnique.Passes[0].Apply();
                App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, overlaylines, 0, count / 2);
                App.graphics.GraphicsDevice.SetRenderTarget(null);
            }
            bool IsInGrid = Simulator.mo_worldposx > 0 && Simulator.mo_worldposy > 0 && Simulator.mo_worldposx < Simulator.SIZEX - 1 && Simulator.mo_worldposy < Simulator.SIZEY - 1;
            if (IsInGrid && !Simulator.IsAllHighlight)
            {
                int netID = Simulator.WireIDs[Simulator.mo_worldposx / 2, Simulator.mo_worldposy / 2, Simulator.currentlayer];
                if (netID > 0 && (Simulator.IsWire[Simulator.mo_worldposx, Simulator.mo_worldposy] & (1 << Simulator.currentlayer)) > 0)
                {
                    if (curhighlightID != netID)
                    {
                        Network netw = Simulator.networks[netID];
						App.graphics.GraphicsDevice.SetRenderTarget(Highlight_target);
						if (highlight_vertices != null)
						{
							highlight_effect.Parameters["highlightvalue"].SetValue(0.0f);
							highlight_effect.CurrentTechnique.Passes[0].Apply();
							App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, highlight_vertices, 0, highlight_vertices.Length / 2);
						}
						highlight_vertices = new VertexPositionLine[netw.lines.Count * 2];
						int count2 = 0;
                        for (int i = 0; i < netw.lines.Count; ++i)
                        {
                            VertexPositionLine l1, l2;
                            Line line = new Line(netw.lines[i].start, netw.lines[i].end);
                            line.Convert2LineVertices(1.0f, out l1, out l2);
                            highlight_vertices[count2++] = l1;
							highlight_vertices[count2++] = l2;
                        }

						//App.graphics.GraphicsDevice.Clear(Color.Transparent);
						highlight_effect.Parameters["highlightvalue"].SetValue(1.0f);
						highlight_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
                        highlight_effect.CurrentTechnique.Passes[0].Apply();
                        App.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, highlight_vertices, 0, highlight_vertices.Length / 2);
                        App.graphics.GraphicsDevice.SetRenderTarget(null);
                        //System.Threading.Thread.Sleep(100);
                        spritebatch.Begin();
                        spritebatch.Draw(Simulator.main_target, Vector2.Zero, Color.White);
                        spritebatch.End();
                    }
                    curhighlightID = netID;
                }
                else
                {
                    if (curhighlightID != 0)
						ClearAllHeighlighting();
                    curhighlightID = 0;
                }
            }

        }

        public void DrawCompOverlays(SpriteBatch spritebatch)
        {
            if (Simulator.worldzoom > 3)
            {
				int topleftchunk_X, topleftchunk_Y;
				Simulator.Screen2worldcoo_int(Vector2.Zero, out topleftchunk_X, out topleftchunk_Y);
				int bottomrightchunk_X, bottomrightchunk_Y;
				Simulator.Screen2worldcoo_int(new Vector2(App.Screenwidth, App.Screenheight), out bottomrightchunk_X, out bottomrightchunk_Y);
				int offset = 20;
				topleftchunk_X -= offset;
				topleftchunk_Y -= offset;
				bottomrightchunk_X += offset;
				bottomrightchunk_Y += offset;

				topleftchunk_X = MathHelper.Clamp(topleftchunk_X / 32, 0, Simulator.SIZEX / 32 - 1);
				topleftchunk_Y = MathHelper.Clamp(topleftchunk_Y / 32, 0, Simulator.SIZEY / 32 - 1);
				bottomrightchunk_X = MathHelper.Clamp(bottomrightchunk_X / 32, 0, Simulator.SIZEX / 32 - 1);
				bottomrightchunk_Y = MathHelper.Clamp(bottomrightchunk_Y / 32, 0, Simulator.SIZEY / 32 - 1);
				for(int x = topleftchunk_X; x <= bottomrightchunk_X; ++x)
				{
					for (int y = topleftchunk_Y; y <= bottomrightchunk_Y; ++y)
					{
						if(CompOverlayGrid[x, y] != null)
						{
							for(int i = 0; i < CompOverlayGrid[x, y].Count; ++i)
							{
								int ID = CompOverlayGrid[x, y][i];
								CompData compdata = Components_Data[components[ID].dataID];
								if (compdata.OverlayText.Length > 0)
								{
									Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText);
									Vector2 pos = new Vector2((components[ID].pos.ToVector2().X + compdata.OverlayTextPos[components[ID].rotation].X + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.X, (components[ID].pos.ToVector2().Y + compdata.OverlayTextPos[components[ID].rotation].Y + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.Y);
									spritebatch.DrawString(CompData.overlayfont, compdata.OverlayText, pos, Color.Black, 0, size / 2, compdata.OverlayTextSize[components[ID].rotation] * (float)Math.Pow(2, Simulator.worldzoom), SpriteEffects.None, 0);
								}
							}
						}
					}
				}



				//for (int i = 0; i < 2000; ++i)
    //            {
    //                if (components[i] != null)
    //                {
    //                    CompData compdata = Components_Data[components[i].dataID];
    //                    if (compdata.OverlayText.Length > 0)
    //                    {
    //                        Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText);
    //                        Vector2 pos = new Vector2((components[i].pos.ToVector2().X + compdata.OverlayTextPos[components[i].rotation].X + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.X, (components[i].pos.ToVector2().Y + compdata.OverlayTextPos[components[i].rotation].Y + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.Y);
    //                        spritebatch.DrawString(CompData.overlayfont, compdata.OverlayText, pos, Color.Black, 0, size / 2, compdata.OverlayTextSize[components[i].rotation] * (float)Math.Pow(2, Simulator.worldzoom), SpriteEffects.None, 0);
    //                    }
    //                }
    //            }
            }
        }
    }
}
