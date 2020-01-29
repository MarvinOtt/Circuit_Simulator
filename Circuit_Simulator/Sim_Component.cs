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
                //for (int j = 0; j < 7; ++j)
                //{
                //    int wireID = Simulator.WireIDs[curpos.X / 2, curpos.Y / 2, j];
                //    if (wireID != 0 && (Simulator.IsWire[curpos.X, curpos.Y] & (1 << j)) != 0)
                //    {
                //        if (pinNetworkIDs[Sim_Component.CompType[curpos.X, curpos.Y] - (Sim_Component.PINOFFSET + 1)] == 0)
                //            pinNetworkIDs[Sim_Component.CompType[curpos.X, curpos.Y] - (Sim_Component.PINOFFSET + 1)] = wireID;
                //    }
                //}
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
            //Sim_Component.components[Sim_Component.nextComponentID++] = new Component(pos, ID, Sim_Component.nextComponentID - 1);
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
                    Point datapos = currentcoo - area.Location;// datapixel[i].pos - Sim_Component.Components_Data[dataID].bounds[newrotation].Location;
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
            //Console.WriteLine(milis);
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
    //public class ComponentData
    //{
    //    public List<ComponentPixel>[] data;
    //    public string name;
    //    public string catagory;
    //    public Rectangle[] bounds;
    //    public int currentrotation;
    //    public int pin_num, OverlayStateID, internalstate_length;
    //    public bool IsOverlay, IsUpdateAfterSim;
    //    public bool CanBeClicked;
    //    public Action<Component> ClickAction, AfterSimAction;
    //    public List<VertexPositionLine> overlaylines;
    //    public Texture2D overlaytex;
    //    public FRectangle[] overlay_bounds;
    //    public Rectangle overlaytex_bounds;

    //    public ComponentData(string name, string catagory, bool IsOverlay, bool IsClickable, bool IsUpdateAfterSim)
    //    {
    //        this.name = name;
    //        this.CanBeClicked = IsClickable;
    //        this.IsOverlay = IsOverlay;
    //        this.IsUpdateAfterSim = IsUpdateAfterSim;
    //        if (IsOverlay)
    //            overlaylines = new List<VertexPositionLine>();
    //        data = new List<ComponentPixel>[4];
    //        bounds = new Rectangle[4];
    //        for (int i = 0; i < 4; ++i)
    //            data[i] = new List<ComponentPixel>();
    //        overlay_bounds = new FRectangle[4];
    //    }

    //    public void CalculateBounds(int rotation)
    //    {
    //        bounds[rotation].X = data[rotation].Min(x => x.pos.X);
    //        bounds[rotation].Y = data[rotation].Min(x => x.pos.Y);
    //        bounds[rotation].Width = data[rotation].Max(x => (x.pos.X - bounds[rotation].X) + 1);
    //        bounds[rotation].Height = data[rotation].Max(x => (x.pos.Y - bounds[rotation].Y) + 1);
    //    }

    //    public void addData(ComponentPixel dat)
    //    {
    //        if (dat.type > Sim_Component.PINOFFSET)
    //            pin_num++;
    //        data[0].Add(dat);
    //        data[1].Add(new ComponentPixel(new Point(-dat.pos.Y, dat.pos.X), dat.type));
    //        data[2].Add(new ComponentPixel(new Point(-dat.pos.X, -dat.pos.Y), dat.type));
    //        data[3].Add(new ComponentPixel(new Point(dat.pos.Y, -dat.pos.X), dat.type));
    //        for (int i = 0; i < 4; ++i)
    //            CalculateBounds(i);
    //    }

    //    public void Finish()
    //    {
    //        for (int i = 0; i < 4; ++i)
    //        {
    //            for(int j = 0; j < data[i].Count; ++j)
    //            {
    //                if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X - 1) && x.pos.Y == (data[i][j].pos.Y)))
    //                    data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 0)));
    //                if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X) && x.pos.Y == (data[i][j].pos.Y - 1)))
    //                    data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 1)));
    //                if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X + 1) && x.pos.Y == (data[i][j].pos.Y)))
    //                    data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 2)));
    //                if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X) && x.pos.Y == (data[i][j].pos.Y + 1)))
    //                    data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 3)));
    //            }
    //        }
    //        int a = 3;
    //    }

    //    public void addOverlayLine(VertexPositionLine line)
    //    {
    //        overlaylines.Add(line);
    //    }
    //}

    public class Sim_Component
    {
        public static int PINOFFSET = 4;

        Simulator sim;
        Effect sim_effect;
        Texture2D placementtex;
        public static RenderTarget2D CompTex, IsEdgeTex, HighlightTex;
        public bool IsCompDrag;

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
        Effect overlay_effect;

        public Sim_Component(Simulator sim, Effect sim_effect)
        {
            this.sim = sim;
            this.sim_effect = sim_effect;
            overlay_effect = Game1.content.Load<Effect>("overlay_effect");
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

            // Basic Components Data

//            CompLibrary compLibrary = new CompLibrary("Main_Library", @"LIBRARIES\Main_Library.dcl");
//            CompData newcomp = new CompData("AND", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_AND(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = WireStatesIN[CompInfo[1]] & WireStatesIN[CompInfo[2]];
//            }";
//            newcomp.Code_Sim_FuncName = "CF_AND";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("OR", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_OR(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = WireStatesIN[CompInfo[1]] | WireStatesIN[CompInfo[2]];
//            }";
//            newcomp.Code_Sim_FuncName = "CF_OR";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("XOR", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_XOR(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = WireStatesIN[CompInfo[1]] ^ WireStatesIN[CompInfo[2]];
//            }";
//            newcomp.Code_Sim_FuncName = "CF_XOR";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("NAND", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_NAND(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = (~(WireStatesIN[CompInfo[1]] & WireStatesIN[CompInfo[2]])) & 1;
//            }";
//            newcomp.Code_Sim_FuncName = "CF_NAND";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("NOR", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_NOR(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = (~(WireStatesIN[CompInfo[1]] | WireStatesIN[CompInfo[2]])) & 1;
//            }";
//            newcomp.Code_Sim_FuncName = "CF_NOR";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("XNOR", "Gates", false, false, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 1));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 1));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(2, 0), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 5));
//            newcomp.Code_Sim = @"void CF_XNOR(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//            {
//	            WireStatesOUT[CompInfo[3]] = (~(WireStatesIN[CompInfo[1]] ^ WireStatesIN[CompInfo[2]])) & 1;
//            }";
//            newcomp.Code_Sim_FuncName = "CF_XNOR";
//            newcomp.ShowOverlay = true;
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("Switch", "Input", true, true, false);
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 2));
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 2));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 2));
//            newcomp.addData(new ComponentPixel(new Point(-1, 0), 2));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 2));
//            newcomp.addData(new ComponentPixel(new Point(-1, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 5));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 6));
//            newcomp.addData(new ComponentPixel(new Point(-1, 1), 7));
//            newcomp.InitializeLineOverlays(1);
//            newcomp.addOverlayLine(new Line(new Point(-1, 0), new Point(1, 0)), 200, 0);
//            newcomp.addOverlayLine(new Line(new Point(0, -1), new Point(0, 1)), 200, 0);
//            newcomp.internalstate_length = 1;
//            newcomp.ClickAction_Type = 0;
//            newcomp.OverlayStateID = 0;
//            newcomp.Code_Sim = @"void CF_SWITCH(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//{
//	WireStatesOUT[CompInfo[1]] = CompInfo[5];
//	WireStatesOUT[CompInfo[2]] = CompInfo[5];
//	WireStatesOUT[CompInfo[3]] = CompInfo[5];
//	WireStatesOUT[CompInfo[4]] = CompInfo[5];
//}";
//            newcomp.Code_Sim_FuncName = "CF_SWITCH";
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            newcomp = new CompData("Led 2x2", "Output", true, false, true);
//            newcomp.addData(new ComponentPixel(new Point(0, 0), 2));
//            newcomp.addData(new ComponentPixel(new Point(1, 0), 2));
//            newcomp.addData(new ComponentPixel(new Point(0, 1), 2));
//            newcomp.addData(new ComponentPixel(new Point(1, 1), 2));
//            newcomp.addData(new ComponentPixel(new Point(0, -1), 4));
//            newcomp.addData(new ComponentPixel(new Point(1, -1), 5));
//            newcomp.InitializeLineOverlays(1);
//            newcomp.addOverlayLine(new Line(new Point(0, 0), new Point(1, 0)), 200, 0);
//            newcomp.addOverlayLine(new Line(new Point(0, 1), new Point(1, 1)), 200, 0);
//            newcomp.internalstate_length = 1;
//            newcomp.OverlayStateID = 0;
//            newcomp.Code_Sim = @"void CF_LED2x2(unsigned char* WireStatesIN, unsigned char* WireStatesOUT, int* CompInfo)
//{
//	CompInfo[3] =  WireStatesIN[CompInfo[1]];
//}";
//            newcomp.Code_Sim_FuncName = "CF_LED2x2";
//            newcomp.Code_AfterSim = @"void DLL_EXPORT ASA_LED2x2(int* internalstates, int* CompInfos, int intstatesindex)
//{
//    internalstates[0] = CompInfos[intstatesindex];
//}";
//            newcomp.Code_AfterSim_FuncName = "ASA_LED2x2";
//            newcomp.Finish();
//            compLibrary.AddComponent(newcomp);

//            compLibrary.Save();
            int breaki = 1;

            //Components_Data = new List<CompData>();
            //Components_Data.Add(new CompData("Button", "Input", true, true, false));
            //Components_Data[0].addData(new ComponentPixel(new Point(0, -1), 2));
            //Components_Data[0].addData(new ComponentPixel(new Point(0, 0), 2));
            //Components_Data[0].addData(new ComponentPixel(new Point(0, 1), 2));
            //Components_Data[0].addData(new ComponentPixel(new Point(-1, 0), 2));
            //Components_Data[0].addData(new ComponentPixel(new Point(1, 0), 2));
            //Components_Data[0].addData(new ComponentPixel(new Point(-1, -1), 4));
            //Components_Data[0].addData(new ComponentPixel(new Point(1, -1), 5));
            //Components_Data[0].addData(new ComponentPixel(new Point(1, 1), 6));
            //Components_Data[0].addData(new ComponentPixel(new Point(-1, 1), 7));
            //Components_Data[0].addOverlayLine(new VertexPositionLine(new Point(-1, 0), 200));
            //Components_Data[0].addOverlayLine(new VertexPositionLine(new Point(2, 0), 200));
            //Components_Data[0].addOverlayLine(new VertexPositionLine(new Point(0, -1), 200));
            //Components_Data[0].addOverlayLine(new VertexPositionLine(new Point(0, 2), 200));
            //Components_Data[0].internalstate_length = 1;
            //Components_Data[0].ClickAction_Type = 0;
            //Components_Data[0].OverlayStateID = 0;
            //Components_Data[0].Finish();
            //Point size = new Point(384, 256);
            //Rectangle[] bounds = new Rectangle[] { new Rectangle(Point.Zero, size), new Rectangle(new Point(384, 0), size), Rectangle.Empty, Rectangle.Empty, Rectangle.Empty, Rectangle.Empty };
            //string[] Comp_Names = new string[] { "AND", "OR", "XOR", "NAND", "NOR", "XNOR" };
            //for (int i = 1; i <= 6; ++i)
            //{
            //    Components_Data.Add(new CompData(Comp_Names[i - 1], "Gates", false, false, false));
            //    Components_Data[i].addData(new ComponentPixel(new Point(0, -1), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(0, 0), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(0, 1), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(1, -1), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(1, 0), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(1, 1), 1));
            //    Components_Data[i].addData(new ComponentPixel(new Point(-1, -1), 4));
            //    Components_Data[i].addData(new ComponentPixel(new Point(2, 0), 6));
            //    Components_Data[i].addData(new ComponentPixel(new Point(-1, 1), 5));
            //    Components_Data[i].overlaytex = Game1.content.Load<Texture2D>("Overlays\\Overlay_AND");
            //    Components_Data[i].overlaytex_bounds = bounds[i - 1];
            //    Components_Data[i].overlay_bounds[0] = new FRectangle(-1.0f, -1.0f, 3.0f, 2.0f);
            //    Components_Data[i].overlay_bounds[1] = new FRectangle(-1.5f, -0.5f, 3.0f, 2.0f);
            //    Components_Data[i].overlay_bounds[2] = new FRectangle(-2.0f, -1.0f, 3.0f, 2.0f);
            //    Components_Data[i].overlay_bounds[3] = new FRectangle(-1.5f, -1.5f, 3.0f, 2.0f);
            //    Components_Data[i].Finish();
            //}

            //Components_Data.Add(new CompData("Led 2x2", "Output", true, false, true));
            //Components_Data[7].addData(new ComponentPixel(new Point(0, 0), 2));
            //Components_Data[7].addData(new ComponentPixel(new Point(1, 0), 2));
            //Components_Data[7].addData(new ComponentPixel(new Point(0, 1), 2));
            //Components_Data[7].addData(new ComponentPixel(new Point(1, 1), 2));
            //Components_Data[7].addData(new ComponentPixel(new Point(0, -1), 4));
            //Components_Data[7].addData(new ComponentPixel(new Point(1, -1), 5));
            //Components_Data[7].addOverlayLine(new VertexPositionLine(new Point(0, 0), 200));
            //Components_Data[7].addOverlayLine(new VertexPositionLine(new Point(2, 0), 200));
            //Components_Data[7].addOverlayLine(new VertexPositionLine(new Point(0, 1), 200));
            //Components_Data[7].addOverlayLine(new VertexPositionLine(new Point(2, 1), 200));
            //Components_Data[7].internalstate_length = 1;
            //Components_Data[7].Finish();
        }

        public void InizializeComponentDrag(int ID)
        {
            if (true)//UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                UI_Handler.UI_IsWindowHide = true;
                //((UI_TexButton)UI_Handler.QuickHotbar.ui_elements[5]).IsActivated = false;
                //UI_Handler.wire_ddbl.GetsUpdated = UI_Handler.wire_ddbl.GetsDrawn = false;
                //UI_Handler.wire_ddbl.GetsUpdated = UI_Handler.wire_ddbl.GetsDrawn = false;
                Game1.simulator.ChangeToolmode(Simulator.TOOL_COMPONENT);
                IsCompDrag = true;
                byte[] data = new byte[81 * 81];
                List<ComponentPixel> datapixel = Components_Data[ID].data[Components_Data[ID].currentrotation];
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 40) * 81 + (datapixel[i].pos.X + 40)] = datapixel[i].type;
                }

                placementtex.SetData(data);
                //sim_effect.Parameters["currenttype"].SetValue(1);
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
                    Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation = (Components_Data[UI_Handler.dragcomp.comp.ID].currentrotation + 1) % 4;
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
            //if (UI_Handler.UI_Active_State == UI_Handler.UI_Active_CompDrag)
            //{
                if (Component.IsValidPlacement(dataID, pos, rotation))
                {
                    FileHandler.IsUpToDate = false;
                    Component newcomp;// = new Component(ID, nextComponentID);
                    if (emptyComponentID_count > 0)
                        newcomp = new Component(dataID, emptyComponentID[--emptyComponentID_count]);
                    else
                        newcomp = new Component(dataID, nextComponentID++);
                    components[newcomp.ID] = newcomp;
                    newcomp.Place(pos, rotation);

                }
            //}
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
                    //if(cur_comp != null)
                    cur_comp.CheckAndUpdatePins();
                    //bool IsNetwork = false;
                    //int wireID = 0;
                    //for(int j = 0; j < 7; ++j)
                    //{
                    //    int wireID2 = Simulator.WireIDs[pos.X / 2, pos.Y / 2, j];
                    //    if (wireID2 != 0 && (Simulator.IsWire[pos.X, pos.Y] & (1 << j)) != 0)
                    //    {
                    //        IsNetwork = true;
                    //        wireID = wireID2;
                    //    }
                    //}
                    //cur_comp.pinNetworkIDs[CompType[pos.X, pos.Y] - (PINOFFSET + 1)] = wireID;
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
                //InizializeComponentDrag(UI_Handler.dragcomp.comp.ID);
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
            //if()

        }

        public void DrawCompOverlays(SpriteBatch spritebatch)
        {
            if (Simulator.worldzoom > 4)
            {
                for (int i = 0; i < 1000; ++i)
                {
                    if (components[i] != null)
                    {
                        CompData compdata = Components_Data[components[i].dataID];
                        if(compdata.ShowOverlay)
                        {
                            float pow = (float)Math.Pow(2, Simulator.worldzoom);
                            Vector2 screencoo = Simulator.worldpos.ToVector2() + pow * (components[i].pos.ToVector2() + new Vector2(0.5f)/*compdata.bounds[components[i].rotation].Size.ToVector2() / 2.0f*/);
                            spritebatch.DrawString(Game1.basefont, compdata.name, screencoo - compdata.overlaysize.ToVector2() / 2, Color.Black);
                        }
                        //if (compdata.overlaytex != null)
                        //{
                        //    FRectangle destrec = compdata.overlay_bounds[components[i].rotation];
                        //    float pow = (float)Math.Pow(2, Simulator.worldzoom);
                        //    destrec.X += 0.5f;
                        //    destrec.Y += 0.5f;
                        //    destrec *= pow;
                        //    Vector2 screencoo = Simulator.worldpos.ToVector2() + pow * components[i].pos.ToVector2();
                        //    destrec.X += screencoo.X;
                        //    destrec.Y += screencoo.Y;
                        //    spritebatch.Draw(compdata.overlaytex, destrec.ToRectangle(), compdata.overlaytex_bounds, Color.White);
                        //}
                    }
                }
            }
        }
    }
}
