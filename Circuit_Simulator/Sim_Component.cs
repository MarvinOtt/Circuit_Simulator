using Circuit_Simulator.COMP;
using Circuit_Simulator.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    public class Component
    {
        public int ID;
        public int dataID;
        public Point pos;
        public int rotation;
        public int[] pinNetworkIDs;
        public int[] internalstates;

        public Component(int dataID, int ID)
        {
            this.dataID = dataID;
            this.ID = ID;
            CompData compdata = Sim_Component.Components_Data[dataID];
            if (compdata.IsOverlay)
                Sim_Component.CompMayneedoverlay.Add(ID);
            if(compdata.totalstate_length > 0)
                internalstates = new int[compdata.totalstate_length];
            pinNetworkIDs = new int[compdata.pin_num];
        }

        public void Clicked()
        {
            if(Sim_Component.Components_Data[dataID].IsClickable)
            {
                Sim_Component.Components_Data[dataID].ClickAction(this);
            }
        }

        public void CheckAndUpdatePins()
        {
            List<ComponentPixel> curpixel = Sim_Component.Components_Data[dataID].data[rotation];
            Array.Clear(pinNetworkIDs, 0, pinNetworkIDs.Length);
            for (int i = 0; i < Sim_Component.Components_Data[dataID].data[0].Count; ++i)
            {
                if (curpixel[i].type > Sim_Component.PINOFFSET)
                {
                    Point curpos = curpixel[i].pos + pos;
                    pinNetworkIDs[Sim_Component.CompType[curpos.X, curpos.Y] - (Sim_Component.PINOFFSET + 1)] = Simulator.WireIDPs[curpos.X, curpos.Y];
                }
                
            }
        }

        public static bool IsValidPlacement(int dataID, Point pos, int rotation)
        {
            //Check if component can be placed
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[rotation];
            bool IsPlacementValid = true;
            if (Simulator.IsSimulating)
                IsPlacementValid = false;
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                if (currentcoo.X >= Simulator.MINCOO && currentcoo.Y >= Simulator.MINCOO && currentcoo.X < Simulator.MAXCOO && currentcoo.Y < Simulator.MAXCOO)
                {
                    if(Sim_Component.CompType[currentcoo.X, currentcoo.Y] != 0)
                        IsPlacementValid = false;
                    else
                    {
                        if (datapixel[i].type < Sim_Component.PINOFFSET + 1 && Simulator.IsWire[currentcoo.X, currentcoo.Y] != 0)
                            IsPlacementValid = false;
                    }
                }
                else
                    IsPlacementValid = false;
            }
            return IsPlacementValid;
        }

        public void Place(Point pos, int newrotation, bool SkipNetworkRouting = false)
        {
            FileHandler.IsUpToDate = false;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            this.pos = pos;
            this.rotation = newrotation;
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[newrotation];
            Rectangle area = Sim_Component.Components_Data[dataID].bounds[newrotation];
            area.Location += pos;
            byte[,] data2place = new byte[area.Size.X, area.Size.Y];
            Simulator.IsWire.GetArea(data2place, area);
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                Sim_Component.CompType[currentcoo.X, currentcoo.Y] = datapixel[i].type;
                Sim_Component.CompTex.SetPixel(datapixel[i].type, currentcoo);
                Sim_Component.IsEdgeTex.SetPixel(datapixel[i].IsEdge, currentcoo);
                if (datapixel[i].type > Sim_Component.PINOFFSET)
                {
                    Point datapos = currentcoo - area.Location;
                    data2place[datapos.X, datapos.Y] |= 128;

                }
                Point gridpos = new Point(currentcoo.X / 32, currentcoo.Y / 32);
                if (Sim_Component.CompGrid[gridpos.X, gridpos.Y] == null)
                {
                    Sim_Component.CompGrid[gridpos.X, gridpos.Y] = new int[200];
                    Sim_Component.CompGrid[gridpos.X, gridpos.Y][0] = ID;
                    Sim_Component.CompNetwork[currentcoo.X, currentcoo.Y] = 0;
                }
                else
                {
                    int[] curNetworks = Sim_Component.CompGrid[gridpos.X, gridpos.Y];
                    int Index = Array.IndexOf(curNetworks, ID);
                    if (Index < 0)
                    {
                        int j = 0;
                        for (; ; ++j)
                        {
                            if (curNetworks[j] == 0)
                            {
                                curNetworks[j] = ID;
                                break;
                            }
                        }
                        Sim_Component.CompNetwork[currentcoo.X, currentcoo.Y] = (byte)j;
                    }
                    else
                        Sim_Component.CompNetwork[currentcoo.X, currentcoo.Y] = (byte)Index;
                }
            }
            Game1.simulator.PlaceArea(area, data2place, SkipNetworkRouting);
            watch.Stop();
            double milis = (1000.0 * watch.ElapsedTicks) / (double)Stopwatch.Frequency;
        }

        public void Delete()
        {
            FileHandler.IsUpToDate = false;
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[rotation];
            Rectangle area = Sim_Component.Components_Data[dataID].bounds[rotation];
            area.Location += pos;
            byte[,] data2place = new byte[area.Size.X, area.Size.Y];
            Simulator.IsWire.GetArea(data2place, area);

            if (Sim_Component.Components_Data[dataID].IsOverlay)
                Sim_Component.CompMayneedoverlay.Remove(ID);
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                Sim_Component.CompTex.SetPixel(0, currentcoo);
                Sim_Component.CompType[currentcoo.X, currentcoo.Y] = 0;
                Point gridpos = new Point(currentcoo.X / 32, currentcoo.Y / 32);
                Sim_Component.CompGrid[gridpos.X, gridpos.Y][Sim_Component.CompNetwork[currentcoo.X, currentcoo.Y]] = 0;
                Sim_Component.CompNetwork[currentcoo.X, currentcoo.Y] = 0;
                if (datapixel[i].type > Sim_Component.PINOFFSET)
                {
                    Point datapos = datapixel[i].pos - Sim_Component.Components_Data[dataID].bounds[rotation].Location;
                    data2place[datapos.X, datapos.Y] &= 127;
                }
            }
            Game1.simulator.PlaceArea(area, data2place);

            Sim_Component.emptyComponentID[Sim_Component.emptyComponentID_count++] = ID;
            Sim_Component.components[ID] = null;
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
        public static RenderTarget2D CompTex, IsEdgeTex, HighlightTex;
        public bool IsCompDrag;
        List<VertexPositionLine> vertices = new List<VertexPositionLine>();

        public static List<CompData> Components_Data;
        public static Component[] components;
        public static int[] emptyComponentID;
        public static int emptyComponentID_count;
        public static List<int> CompMayneedoverlay;
        public static int nextComponentID = 1;
        public static byte[,] CompType;
        public static int[,][] CompGrid;
        public static byte[,] CompNetwork;
        public static Point[] pins2check;
        public static int pins2check_length;
        public static VertexPositionLine[] overlaylines;
        public static bool DropComponent;
        Effect overlay_effect, highlight_effect;

        public Sim_Component(Simulator sim, Effect sim_effect)
        {
            this.sim = sim;
            this.sim_effect = sim_effect;
            overlay_effect = Game1.content.Load<Effect>("overlay_effect");
            highlight_effect = Game1.content.Load<Effect>("UI\\highlight_effect");
            placementtex = new Texture2D(Game1.graphics.GraphicsDevice, 81, 81, false, SurfaceFormat.Alpha8);
            CompTex = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            HighlightTex = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            IsEdgeTex = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            CompType = new byte[Simulator.SIZEX, Simulator.SIZEY];
            CompGrid = new int[Simulator.SIZEX / 32, Simulator.SIZEY / 32][];
            CompNetwork = new byte[Simulator.SIZEX, Simulator.SIZEY];
            components = new Component[1000000];
            pins2check = new Point[4000000];
            overlaylines = new VertexPositionLine[1000000];
            emptyComponentID = new int[1000000];
            CompMayneedoverlay = new List<int>();
            Components_Data = new List<CompData>();
            string[] Libraries2Load = Directory.GetFiles(@"LIBRARIES\", "*.dcl");
            Sim_INF_DLL.LoadLibrarys(Libraries2Load);

          
        }

        public void InizializeComponentDrag(int ID)
        {
            if (true)
            {
                UI_Handler.UI_IsWindowHide = true;
              
                Game1.simulator.ChangeToolmode(Simulator.TOOL_COMPONENT);
                IsCompDrag = true;
                byte[] data = new byte[81 * 81];
                List<ComponentPixel> datapixel = Components_Data[ID].data[Components_Data[ID].currentrotation];
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 40) * 81 + (datapixel[i].pos.X + 40)] = datapixel[i].type;
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
                if (Game1.kb_states.IsKeyToggleDown(Keys.R))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_ROT[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
                if (Game1.kb_states.IsKeyToggleDown(Keys.X))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_FLIPX[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
                if (Game1.kb_states.IsKeyToggleDown(Keys.Y))
                {
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = CompData.rottable_FLIPY[Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation];
                    InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
                }
            }
            else
                sim_effect.Parameters["currenttype"].SetValue(0);

            if (Game1.kb_states.IsKeyToggleDown(Keys.Escape) || Game1.mo_states.IsRightButtonToggleOff())
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
            Game1.simulator.Screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out pos.X, out pos.Y);
            ComponentDropAtPos(dataID, pos);
        }
        public static void ComponentDropAtPos(int dataID, Point pos)
        {
            ComponentDropAtPos(dataID, pos, Components_Data[dataID].currentrotation);
        }
        public static void ComponentDropAtPos(int dataID, Point pos, int rotation)
        {
                if (Component.IsValidPlacement(dataID, pos, rotation))
                {
                    FileHandler.IsUpToDate = false;
                    Component newcomp;
                    if (emptyComponentID_count > 0)
                        newcomp = new Component(dataID, emptyComponentID[--emptyComponentID_count]);
                    else
                        newcomp = new Component(dataID, nextComponentID++);
                    components[newcomp.ID] = newcomp;
                    newcomp.Place(pos, rotation);

                }
        }

        public void DeactivateDrop()
        {
            UI_Handler.UI_IsWindowHide = false;
            IsCompDrag = false;
            sim_effect.Parameters["currenttype"].SetValue(0);
            Game1.simulator.ChangeToolmode(Simulator.oldtoolmode);
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
            sim_effect.Parameters["comptex"].SetValue(CompTex);
            sim_effect.Parameters["highlighttex"].SetValue(HighlightTex);
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
                    int ovstate = components[compID].internalstates[ovstateID + k];
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
                Game1.graphics.GraphicsDevice.SetRenderTarget(CompTex);
                overlay_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
                overlay_effect.CurrentTechnique.Passes[0].Apply();
                Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, overlaylines, 0, count / 2);
                Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            }
            bool IsInGrid = Simulator.mo_worldposx > 0 && Simulator.mo_worldposy > 0 && Simulator.mo_worldposx < Simulator.SIZEX - 1 && Simulator.mo_worldposy < Simulator.SIZEY - 1;
            if (IsInGrid)
            {
                int netID = Simulator.WireIDs[Simulator.mo_worldposx / 2, Simulator.mo_worldposy / 2, Simulator.currentlayer];
                if (netID > 0 && (Simulator.IsWire[Simulator.mo_worldposx, Simulator.mo_worldposy] & (1 << Simulator.currentlayer)) > 0)
                {
                    Network netw = Simulator.networks[netID];
                    vertices.Clear();
                    for(int i = 0; i < netw.lines.Count; ++i)
                    {
                        VertexPositionLine l1, l2;
                        Line line = new Line(netw.lines[i].start, netw.lines[i].end);
                        line.Convert2LineVertices(1, out l1, out l2);
                        vertices.Add(l1);
                        vertices.Add(l2);
                    }

                    Game1.graphics.GraphicsDevice.SetRenderTarget(HighlightTex);
                    Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
                    highlight_effect.Parameters["WorldViewProjection"].SetValue(Simulator.linedrawingmatrix);
                    highlight_effect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
                    Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                }
                else
                {
                    Game1.graphics.GraphicsDevice.SetRenderTarget(HighlightTex);
                    Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
                    Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                }
            }

        }

        public void DrawCompOverlays(SpriteBatch spritebatch)
        {
            if (Simulator.worldzoom > 2)
            {
                for (int i = 0; i < 1000; ++i)
                {
                    if (components[i] != null)
                    {
                        CompData compdata = Components_Data[components[i].dataID];
                        if(compdata.ShowOverlay)
                        {
                            float pow = (float)Math.Pow(2, Simulator.worldzoom);
                            Vector2 screencoo = Simulator.worldpos.ToVector2() + pow * (components[i].pos.ToVector2() + new Vector2(0.5f));
                            spritebatch.DrawString(Game1.basefont, compdata.name, screencoo - compdata.overlaysize.ToVector2() / 2, Color.Black);
                        }
                        if (compdata.OverlayText.Length > 0)
                        {
                            Vector2 size = CompData.overlayfont.MeasureString(compdata.OverlayText);
                            Vector2 pos = new Vector2((components[i].pos.ToVector2().X + compdata.OverlayTextPos[components[i].rotation].X + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.X, (components[i].pos.ToVector2().Y + compdata.OverlayTextPos[components[i].rotation].Y + 0.5f) * (float)Math.Pow(2, Simulator.worldzoom) + Simulator.worldpos.Y);
                            spritebatch.DrawString(CompData.overlayfont, compdata.OverlayText, pos, Color.Black, 0, size / 2, compdata.OverlayTextSize[components[i].rotation] * (float)Math.Pow(2, Simulator.worldzoom), SpriteEffects.None, 0);
                        }
                    }
                }
            }
        }
    }
}
