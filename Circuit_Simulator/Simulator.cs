using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using Circuit_Simulator.COMP;
using Circuit_Simulator.UI;

namespace Circuit_Simulator
{
    public struct Line
    {
        public Point start, end;

        public Line(Point start, Point end)
        {
            this.start = start;
            this.end = end;
        }

        public void Convert2LineVertices(float layers, out VertexPositionLine line1, out VertexPositionLine line2)
        {
            if (start == end)
            {
                line1 = new VertexPositionLine(start, layers);
                line2 = new VertexPositionLine(start + new Point(1), layers);
            }
            else if(start.X == end.X && start.Y > end.Y)
            {
                line1 = new VertexPositionLine(start + new Point(0, 1), layers);
                line2 = new VertexPositionLine(end, layers);
            }
            else if (start.X == end.X && start.Y < end.Y)
            {
                line1 = new VertexPositionLine(start, layers);
                line2 = new VertexPositionLine(end + new Point(0, 1), layers);
            }
            else if(start.X < end.X && start.Y > end.Y)
            {
                line1 = new VertexPositionLine(start, layers);
                line2 = new VertexPositionLine(end + new Point(1, -1), layers);
            }
            else if(start.X > end.X && start.Y < end.Y)
            {
                line1 = new VertexPositionLine(start + new Point(1, -1), layers);
                line2 = new VertexPositionLine(end, layers);
            }
            else if(start.Y == end.Y && start.X < end.X)
            {
                line1 = new VertexPositionLine(start, layers);
                line2 = new VertexPositionLine(end + new Point(1, 0), layers);
            }
            else if (start.Y == end.Y && start.X > end.X)
            {
                line1 = new VertexPositionLine(start + new Point(1, 0), layers);
                line2 = new VertexPositionLine(end, layers);
            }
            else if(start.X < end.X && start.Y < end.Y)
            {
                line1 = new VertexPositionLine(start, layers);
                line2 = new VertexPositionLine(end + new Point(1), layers);
            }
            else
            {
                line1 = new VertexPositionLine(start + new Point(1), layers);
                line2 = new VertexPositionLine(end, layers);
            }
        }
    }

    public struct Line_Netw
    {
        public byte layers;
        public int length;
        public Point start, end, dir;

        public Line_Netw(Point start, Point end, Point dir, int length, byte layers)
        {
            this.start = start;
            this.end = end;
            this.dir = dir;
            this.length = length;
            this.layers = layers;
        }
    }

    public class Network
    {
        public static List<Network> CheckForDrawing = new List<Network>();
        public List<Line_Netw> lines;
        public int ID;
        public byte state;
        public static bool HasRealWires;
        public bool NeedsDrawing;

        public Network(int ID)
        {
            NeedsDrawing = true;
            CheckForDrawing.Add(this);
            this.ID = ID;
            if (ID > Simulator.highestNetworkID)
                Simulator.highestNetworkID = ID;
            lines = new List<Line_Netw>();
        }

        public static void Delete(IEnumerable<int> nets)
        {
            foreach(int net in nets)
            {
                if (Simulator.networks[net] != null)
                {
                    Network curnet = Simulator.networks[net];
                    for (int i = 0; i < curnet.lines.Count; ++i)
                    {
                        Point curpos = curnet.lines[i].start;
                        for (int j = 0; j < curnet.lines[i].length; ++j)
                        {
                            byte linelayers = curnet.lines[i].layers;
                            for(int k = 0; k < 7; ++k)
                            {
                                if (((linelayers >> k) & 1) > 0)
                                    Simulator.WireIDs[curpos.X / 2, curpos.Y / 2, k] = 0;
                            }
                            if(linelayers >= 128)
                                Simulator.WireIDPs[curpos.X, curpos.Y] = 0;


                            curpos += curnet.lines[i].dir;
                        }

                    }
                    Simulator.emptyNetworkID[Simulator.emptyNetworkID_count++] = net;
                }
                Simulator.networks[net] = null;
            }
        }

        public void ClearCalcGrid()
        {
            for(int i = 0; i < lines.Count; ++i)
            {
                for(int l = 0; l < lines[i].length; ++l)
                {
                    int x = lines[i].start.X + lines[i].dir.X * l;
                    int y = lines[i].start.Y + lines[i].dir.Y * l;
                    Simulator.CalcGridData[x, y] = Simulator.CalcGridStat[x, y] = 0;
                  
                }
            }
        }

        public void PlaceNetwork()
        {
            for(int i = 0; i < lines.Count; ++i)
            {
                Point curpos = lines[i].start;
                for(int j = 0; j < lines[i].length; ++j)
                {
                    byte linelayers = lines[i].layers;
                    Simulator.IsWire[curpos.X, curpos.Y] |= linelayers;

                    for (int b = 0; b < 7; ++b)
                        if (((linelayers >> b) & 1) != 0)
                            Simulator.WireIDs[curpos.X / 2, curpos.Y / 2, b] = ID;
                    if (linelayers >= 128)
                        Simulator.WireIDPs[curpos.X, curpos.Y] = ID;

                    curpos += lines[i].dir;
                }
                
            }
            Draw();
        }

        public void Draw()
        {
            if (NeedsDrawing)
            {
                for (int i = 0; i < lines.Count; ++i)
                {
                
                    for (int j = 0; j < Simulator.LAYER_NUM; ++j)
                    {
                        if ((lines[i].layers & (1 << j)) > 0)
                        {
                            Line line = new Line(lines[i].start, lines[i].end);
                            VertexPositionLine line1, line2;
                            line.Convert2LineVertices((1 << j), out line1, out line2);
                            Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = line1;
                            Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = line2;
                        }
                    }
                    if ((lines[i].layers & (1 << 7)) > 0)
                    {
                        Line line = new Line(lines[i].start, lines[i].end);
                        VertexPositionLine line1, line2;
                        line.Convert2LineVertices(128, out line1, out line2);
                        Simulator.lines2draw[7][Simulator.lines2draw_count[7]++] = line1;
                        Simulator.lines2draw[7][Simulator.lines2draw_count[7]++] = line2;
                    }
                }
                NeedsDrawing = false;
            }
        }
    }

    public struct VertexPositionLine : IVertexType
    {
        public Vector3 Position;
        public float layers;

        public VertexPositionLine(Point pos, float layers)
        {
            this.Position = new Vector3(pos.X + 0.45f, pos.Y + 0.45f, 0);
            this.layers = layers;
        }

        public VertexPositionLine(Vector2 pos, float layers)
        {
            this.Position = new Vector3(pos.X, pos.Y, 0);
            this.layers = layers;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
        (
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(12, VertexElementFormat.Single, VertexElementUsage.Color, 0)
        );

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get { return VertexDeclaration; }
        }
    }

    public class Simulator
    {
        public const int TOOL_SELECT = 0;
        public const int TOOL_COMPONENT = 1;
        public const int TOOL_WIRE = 2;

        public const int SIZEX = 10240;
        public const int SIZEY = 10240;
        public const int LAYER_NUM = 7;
        public const int BORDERSIZE = 1;
        public const int MINCOO = BORDERSIZE;
        public const int MAXCOO = SIZEX - BORDERSIZE;

        // Simulator Parts
        public Sim_Component sim_comp;
        public Sim_INF_DLL sim_inf_dll;

        public static Effect sim_effect, line_effect, iswirerender_effect;
        Texture2D copyWiretex, copyComptex;
        RenderTarget2D main_target, WireCalc_target;
        public static RenderTarget2D logic_target, sec_target;
        public static Network CalcNetwork;

        public static Network[] networks;
        public static VertexPositionLine[][] lines2draw;
        public static int[] lines2draw_count;
        public static Matrix linedrawingmatrix;
        public static int highestNetworkID = 3;
        public static int[] emptyNetworkID;
        public static int emptyNetworkID_count;
        public static byte[,] IsWire, CalcGridData, CalcGridStat, IsChange;

        public static HashSet<int> FoundNetworks;
        public static int[] CalcOccurNetw;
        public static byte[] revshiftarray;
        public static int CalcOccurNetw_Pos;
        public static int[,,] WireIDs;
        public static int[,] WireIDPs;

        public static Point worldpos, copymouseoffset, copypos, copysize;
        public static int worldzoom = 0;

        public static int toolmode = TOOL_WIRE, oldtoolmode = TOOL_WIRE, simspeed, simspeed_count, selectstate = 0, copystate;
        public static bool IsSimulating;

        #region Selection

        Point Selection_StartPos, Selection_EndPos, Selection_Size;
        byte[] CopiedIsWire, CopiedCompType;
        int[] CopiedParameterStates, CopiedParameterStates_Indices;
        List<int> CopiedCompIDs, CopiedCompRot;
        List<Point> CopiedCompPos;
        #endregion

        #region INPUT

        public static int currentlayer;
        public static int mo_worldposx, mo_worldposy;
        bool IsInGrid;

        #endregion


        public Simulator()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;

            // Loading Effects
            sim_effect = Game1.content.Load<Effect>("sim_effect");
            line_effect = Game1.content.Load<Effect>("line_effect");
            iswirerender_effect = Game1.content.Load<Effect>("iswirerender_effect");

            // Initializing Render Targets
            sec_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            logic_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            WireCalc_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
          

            FoundNetworks = new HashSet<int>();
            CopiedCompIDs = new List<int>();
            CopiedCompRot = new List<int>();
            CopiedCompPos = new List<Point>();
            IsWire = new byte[SIZEX, SIZEY];
            CalcGridData = new byte[SIZEX, SIZEY];
            CalcGridStat = new byte[SIZEX, SIZEY];
            IsChange = new byte[SIZEX, SIZEY];
            CalcOccurNetw = new int[10000];
            WireIDs = new int[SIZEX / 2, SIZEY / 2, LAYER_NUM];
            WireIDPs = new int[SIZEX, SIZEY];
            networks = new Network[10000000];
            
            emptyNetworkID = new int[10000000];
            revshiftarray = new byte[256];
            for (int i = 0; i < 8; ++i)
                for(int j = 0; j < (1 << i); ++j)
                    revshiftarray[(1 << i) + j] = (byte)i;
            // Initializing Array for the Line Drawing
            lines2draw_count = new int[LAYER_NUM + 2];
            lines2draw = new VertexPositionLine[LAYER_NUM + 2][];
            for (int i = 0; i < LAYER_NUM + 2; ++i)
                lines2draw[i] = new VertexPositionLine[600000];
            linedrawingmatrix = Matrix.CreateOrthographicOffCenter(0, SIZEX + 0.01f, SIZEY + 0.01f, 0, 0, 1);

            //Initializing Simulator Parts
            sim_comp = new Sim_Component(this, sim_effect);
            sim_inf_dll = new Sim_INF_DLL();
        }

        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            main_target?.Dispose();
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);

            sim_effect.Parameters["Screenwidth"].SetValue(Game1.Screenwidth);
            sim_effect.Parameters["Screenheight"].SetValue(Game1.Screenheight);
            sim_effect.Parameters["worldsizex"].SetValue(SIZEX);
            sim_effect.Parameters["worldsizey"].SetValue(SIZEY);
        }

        public void Screen2worldcoo_int(Vector2 screencoos, out int x, out int y)
        {
            x = (int)((screencoos.X - worldpos.X) / (float)Math.Pow(2, worldzoom));
            y = (int)((screencoos.Y - worldpos.Y) / (float)Math.Pow(2, worldzoom));
        }

        public static void DoFFIfValid(int x, int y, byte curval, byte curcomptype, int curcompid)
        {
            byte nextval = IsWire[x, y];
            byte finalval = (byte)(curval & nextval & 0x7F);
           
            if (nextval >= 128 && curval >= 128)
            {
                byte comptype_next = Sim_Component.CompType[x, y];
                byte gridid = Sim_Component.CompNetwork[x, y];
                int[] arr = Sim_Component.CompGrid[x / 32, y / 32];
                int compid_next = -1;
                if (arr != null)
                    compid_next = arr[gridid];
                if (curcomptype == comptype_next && curcompid == compid_next && curcomptype > 0)
                    finalval |= 0x80;
         
            }
            if (finalval > 0 && ((CalcGridData[x, y] ^ finalval) & finalval) > 0)
                FloodFillCellAndNeighbours(x, y, finalval);

       
        }
        public static void FloodFillCellAndNeighbours(int x, int y, int mask)
        {
            byte curval = (byte)(IsWire[x, y] & mask);
            if (IsWire[x, y] >= 128)
                curval = IsWire[x, y];
            if (IsWire[x, y] != 128 || (IsWire[x, y] == 128 && Sim_Component.CompType[x, y] == 0))
                Network.HasRealWires = true;

            CalcGridData[x, y] |= curval;
            byte comptype_cur = Sim_Component.CompType[x, y];
            int compid_cur = Sim_Component.GetComponentID(new Point(x, y));

            if (curval != 0)
            {
                if (Sim_Component.CompType[x, y] > Sim_Component.PINOFFSET)
                    Sim_Component.pins2check[Sim_Component.pins2check_length++] = new Point(x, y);

                DoFFIfValid(x - 1, y, curval, comptype_cur, compid_cur);
                DoFFIfValid(x + 1, y, curval, comptype_cur, compid_cur);
                DoFFIfValid(x, y - 1, curval, comptype_cur, compid_cur);
                DoFFIfValid(x, y + 1, curval, comptype_cur, compid_cur);
                DoFFIfValid(x - 1, y - 1, curval, comptype_cur, compid_cur);
                DoFFIfValid(x + 1, y - 1, curval, comptype_cur, compid_cur);
                DoFFIfValid(x - 1, y + 1, curval, comptype_cur, compid_cur);
                DoFFIfValid(x + 1, y + 1, curval, comptype_cur, compid_cur);
                if(true)
                {
                    if (curval >= 128)
                    {
                        int netID = WireIDPs[x, y];
                        if ((CalcOccurNetw_Pos == 0 || CalcOccurNetw[CalcOccurNetw_Pos - 1] != netID) && netID > 3)
                            CalcOccurNetw[CalcOccurNetw_Pos++] = netID;
                    }
                    else
                    {
                        int netID = WireIDs[x / 2, y / 2, revshiftarray[curval]];
                        if ((CalcOccurNetw_Pos == 0 || CalcOccurNetw[CalcOccurNetw_Pos - 1] != netID) && netID > 3)
                            CalcOccurNetw[CalcOccurNetw_Pos++] = netID;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int MakeLineDir(int x, int y)
        {
            byte curval = CalcGridData[x, y];
            if (CalcGridData[x, y - 1] == curval && CalcGridData[x, y + 1] == curval)
                return 1;
            if (CalcGridData[x + 1, y - 1] == curval && CalcGridData[x - 1, y + 1] == curval)
                return 2;
            if (CalcGridData[x + 1, y] == curval && CalcGridData[x - 1, y] == curval)
                return 3;
            if (CalcGridData[x + 1, y + 1] == curval && CalcGridData[x - 1, y - 1] == curval)
                return 4;

            if (CalcGridData[x, y - 1] == curval || CalcGridData[x, y + 1] == curval)
                return 1;
            if (CalcGridData[x + 1, y - 1] == curval || CalcGridData[x - 1, y + 1] == curval)
                return 2;
            if (CalcGridData[x + 1, y] == curval || CalcGridData[x - 1, y] == curval)
                return 3;
            if (CalcGridData[x + 1, y + 1] == curval || CalcGridData[x - 1, y - 1] == curval)
                return 4;

            return 0;
        }
        public static void Check4NewLine(int x, int y, Point dirvec)
        {
            for(int xx = -1; xx < 2; ++xx)
            {
                for (int yy = -1; yy < 2; ++yy)
                {
                    if(!(xx == 0 && yy == 0))
                    {
                        if (CalcGridStat[x + xx, y + yy] != CalcGridData[x + xx, y + yy] && CalcGridData[x + xx, y + yy] != 0)
                            CalculateLine(x + xx, y + yy);
                        byte d = CalcGridData[x + xx, y + yy];
                    }
                }
            }
        }
        public static void CalculateLine(int x, int y)
        {
            int dir = MakeLineDir(x, y);
            byte linelayers = CalcGridData[x, y];
            Point dirvec = Point.Zero;
            if (dir == 1)
                dirvec = new Point(0, 1);
            if (dir == 2)
                dirvec = new Point(1, -1);
            if (dir == 3)
                dirvec = new Point(1, 0);
            if (dir == 4)
                dirvec = new Point(1, 1);
            if (dir == 0)
                dirvec = new Point(0, 1);

            // Marching every cell for this line
            int i, j;
            for(i = 0; ;++i)
            {
                int xx = x + dirvec.X * i;
                int yy = y + dirvec.Y * i;
                if ((CalcGridStat[xx, yy] & linelayers) != linelayers && CalcGridData[xx, yy] == linelayers)
                {
                    CalcGridStat[xx, yy] |= linelayers;
                    for (int b = 0; b < 7; ++b)
                        if (((linelayers >> b) & 1) != 0)
                            WireIDs[xx / 2, yy / 2, b] = CalcNetwork.ID;
                    if(linelayers >= 128)
                        WireIDPs[xx, yy] = CalcNetwork.ID;
                }
                else
                    break;
            }
            for (j = 1; ; ++j)
            {
                int xx = x - dirvec.X * j;
                int yy = y - dirvec.Y * j;
                if ((CalcGridStat[xx, yy] & linelayers) != linelayers && CalcGridData[xx, yy] == linelayers)
                {
                    CalcGridStat[xx, yy] |= linelayers;
                    for (int b = 0; b < 7; ++b)
                        if (((linelayers >> b) & 1) != 0)
                            WireIDs[xx / 2, yy / 2, b] = CalcNetwork.ID;
                    if (linelayers >= 128)
                        WireIDPs[xx, yy] = CalcNetwork.ID;
                }
                else
                    break;
            }
            i--;
            j--;

            for(int k = -j; k <= i; ++k)
            {
                int xx = x + dirvec.X * k;
                int yy = y + dirvec.Y * k;
                Check4NewLine(xx, yy, dirvec);
            }
            Point start = new Point(x - dirvec.X * j, y - dirvec.Y * j);
            Point end = new Point(x + dirvec.X * i, y + dirvec.Y * i);
            CalcNetwork.lines.Add(new Line_Netw(start, end, dirvec, i + j + 1, linelayers));

        }
        public static int CalculateNewNetwork(int x, int y, int layermask)
        {
            int curvalue = CalcGridData[x, y] & layermask;
            if (curvalue == 0)
                return -1;
            if(!Network.HasRealWires)
            {
              
            }
            int ID = 0;
            if (emptyNetworkID_count > 0)
                ID = emptyNetworkID[--emptyNetworkID_count];
            else
                ID = highestNetworkID + 1;
           
            CalcNetwork = new Network(ID);
            CalculateLine(x, y);
            networks[ID] = CalcNetwork;
            return ID;
        }

        public static void CalculateNetworkAt(int x, int y, byte layermask)
        {
            CalcOccurNetw_Pos = 0;
            Network.HasRealWires = false;
            Array.Clear(CalcOccurNetw, 0, CalcOccurNetw.Length);

            // Flood Fill CalcGrid
            FloodFillCellAndNeighbours(x, y, layermask);

            // Delete previous networks
            for (int i = 0; i < CalcOccurNetw_Pos; ++i)
            {
                if (networks[CalcOccurNetw[i]] != null)
                {
                    FoundNetworks.Add(CalcOccurNetw[i]);
                }
            }
            Network.Delete(FoundNetworks);
            FoundNetworks.Clear();

            // Main Line Algorithm
            int netID = CalculateNewNetwork(x, y, layermask);
            if (netID == -1)
                return;

          

            // Clear Calc Grid
            networks[netID].ClearCalcGrid();
          
        }

        

        public void PlaceArea(Rectangle rec, byte[,] data, bool SkipNetworkDeletion = false)
        {
            HashSet<int> nets = new HashSet<int>();
            Rectangle Brec = rec;
            Brec.Location -= new Point(1);
            Brec.Size += new Point(2);
            byte[,] finaldata = new byte[Brec.Width, Brec.Height];

            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    finaldata[(x - Brec.Left), (y - Brec.Top)] = IsWire[x, y];
                }
            }
            for (int x = rec.Left; x < rec.Right; ++x)
            {
                for (int y = rec.Top; y < rec.Bottom; ++y)
                {
                    finaldata[(x - Brec.Left), (y - Brec.Top)] = data[(x - rec.Left), (y - rec.Top)];
                }
            }


            for (int x = rec.Left; x < rec.Right; ++x)
            {
                for (int y = rec.Top; y < rec.Bottom; ++y)
                {
                    for (int i = 0; i < LAYER_NUM; ++i)
                    {
                        if (WireIDs[x / 2, y / 2, i] != 0)
                        {
                            int xx = (x / 2) * 2;
                            int yy = (y / 2) * 2;
                            int xxx = xx - Brec.Left;
                            int yyy = yy - Brec.Top;
                            if (true) 
                            {
                                nets.Add(WireIDs[x / 2, y / 2, i]);
                                WireIDs[x / 2, y / 2, i] = 0;
                            }
                        }
                    }
                    if(WireIDPs[x, y] != 0)
                    {
                        int xx = x - Brec.Left;
                        int yy = y - Brec.Top;
                        if (true)
                        {
                            nets.Add(WireIDPs[x, y]);
                            WireIDPs[x, y] = 0;
                        }
                    }
                }
            }

            Network.Delete(nets);
            // Placing Data
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    if (Sim_Component.CompType[x, y] > Sim_Component.PINOFFSET)
                        Sim_Component.pins2check[Sim_Component.pins2check_length++] = new Point(x, y);
                    IsWire[x, y] = finaldata[(x - Brec.Left), (y - Brec.Top)];
                }
            }

            //Drawing Area Black
            for (int y = 0; y < Brec.Height; ++y)
            {
                Line line = new Line(new Point(Brec.Left, Brec.Top + y), new Point(Brec.Right - 1, Brec.Top + y));
                VertexPositionLine line1, line2;
                line.Convert2LineVertices(0, out line1, out line2);

                lines2draw[LAYER_NUM + 1][lines2draw_count[LAYER_NUM + 1]++] = line1;
                lines2draw[LAYER_NUM + 1][lines2draw_count[LAYER_NUM + 1]++] = line2;
            }

            // Main Network Calculations
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    if (IsWire[x, y] >= 128)
                        CalculateNetworkAt(x, y, IsWire[x, y]);
                    else
                    {
                        for (int i = 0; i < LAYER_NUM; ++i)
                        {
                            if ((IsWire[x, y] & (1 << i)) > 0)
                                CalculateNetworkAt(x, y, (byte)(1 << i));
                        }
                    }

                  
                }
            }



            if (!SkipNetworkDeletion || true)
            {
                Network.Delete(FoundNetworks);
                FoundNetworks.Clear();
            }
            Sim_Component.CheckPins();
        }

        public bool IsValidPlacementCoo(Point pos)
        {
            return IsInGrid && (Sim_Component.CompType[pos.X, pos.Y] == 0 || Sim_Component.CompType[pos.X, pos.Y] > Sim_Component.PINOFFSET) && !IsSimulating;
        }

        public void SetSimulationState(bool IsSimulating)
        {
            Simulator.IsSimulating = IsSimulating;
            if (IsSimulating)
                sim_inf_dll.GenerateSimulationData();
        }

        public byte GetUILayers()
        { 
            byte OUT = 0;
            for(int i = 0; i < LAYER_NUM + 1; ++i)
            {
                OUT |= (byte)(Convert.ToByte(UI_Handler.WireMaskHotbar.ui_elements[i].IsActivated) << i);
            }

            return OUT;
        }

        public void ChangeToolmode(int newtoolmode)
        {
            if (toolmode == TOOL_COMPONENT)
            { }
            else if (toolmode == TOOL_WIRE)
            { }

            oldtoolmode = toolmode;
            toolmode = newtoolmode;
        }

        public void Update()
        {


            Screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out mo_worldposx, out mo_worldposy);
            Point mo_worldpos = new Point(mo_worldposx, mo_worldposy);
            IsInGrid = mo_worldposx > 0 && mo_worldposy > 0 && mo_worldposx < SIZEX - 1 && mo_worldposy < SIZEY - 1;


            ((UI_String)UI_Handler.GeneralInfoBox.ui_elements[0]).setValue("Pos: X: " + mo_worldposx.ToString() + " Y: " + mo_worldposy.ToString());
            ((UI_String)UI_Handler.GeneralInfoBox.ui_elements[3]).setValue("Speed: 2^" + simspeed.ToString() + " (" + Math.Pow(2, simspeed).ToString() + ")");

            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                #region INPUT
                if (Game1.kb_states.New.IsKeyDown(Keys.W))
                    worldpos.Y += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.S))
                    worldpos.Y -= 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.A))
                    worldpos.X += 10;
                if (Game1.kb_states.New.IsKeyDown(Keys.D))
                    worldpos.X -= 10;
                if (Game1.kb_states.IsKeyToggleDown(Keys.Up))
                    simspeed++;
                if (Game1.kb_states.IsKeyToggleDown(Keys.Down))
                    simspeed--;
                if (Game1.kb_states.IsKeyToggleDown(Keys.Add))
                {
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = false;
                  
                    currentlayer = MathHelper.Clamp(++currentlayer, 0, LAYER_NUM - 1);
                    if (currentlayer == LAYER_NUM - 1)
                    {
                        currentlayer = 0;
                    }
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = true;
                }
                if (Game1.kb_states.IsKeyToggleDown(Keys.Subtract))
                {
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = false;
                   
                    currentlayer = MathHelper.Clamp(--currentlayer, -1, LAYER_NUM - 1);
                    if (currentlayer == -1)
                    {
                        currentlayer = LAYER_NUM - 1;
                    }
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = true;
                }

                if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
                {
                    if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue && worldzoom > -8) // Zooming Out
                    {
                        worldzoom -= 1;
                        Point diff = Game1.mo_states.New.Position - worldpos;
                        worldpos += new Point(diff.X / 2, diff.Y / 2);
                    }
                    else if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue && worldzoom < 10) // Zooming In
                    {
                        worldzoom += 1;
                        Point diff = Game1.mo_states.New.Position - worldpos;
                        worldpos -= diff;
                    }
                }
                #endregion
            }

            if (selectstate > 0 && Game1.kb_states.IsKeyToggleDown(Keys.Escape))
            {
                selectstate = 0;
            }
            // Finishing Selection
            if (Game1.mo_states.IsLeftButtonToggleOff() && selectstate == 1)
            {
                Selection_EndPos.X = MathHelper.Clamp(mo_worldposx, MINCOO, MAXCOO);
                Selection_EndPos.Y = MathHelper.Clamp(mo_worldposy, MINCOO, MAXCOO);
                Point start = new Point(Math.Min(Selection_StartPos.X, Selection_EndPos.X), Math.Min(Selection_StartPos.Y, Selection_EndPos.Y));
                Point end = new Point(Math.Max(Selection_StartPos.X, Selection_EndPos.X), Math.Max(Selection_StartPos.Y, Selection_EndPos.Y));
                Selection_StartPos = start;
                Selection_EndPos = end;
                Selection_Size = (end - start) + new Point(1);
                selectstate = 2;
            }
            if (!IsSimulating)
            {
                // Copying
                if (selectstate == 2)
                {
                    if (Game1.kb_states.New.AreKeysDown(Keys.LeftControl, Keys.C) && !Game1.kb_states.Old.AreKeysDown(Keys.LeftControl, Keys.C))
                    {
                        copysize = Selection_Size;
                        CopiedIsWire = new byte[Selection_Size.X * Selection_Size.Y];
                        CopiedCompType = new byte[Selection_Size.X * Selection_Size.Y];
                        IsWire.GetAreaWithMask(CopiedIsWire, new Rectangle(Selection_StartPos, Selection_Size), GetUILayers());
                        CopiedCompIDs.Clear();
                        CopiedCompPos.Clear();
                        CopiedCompRot.Clear();
                        if (UI_Handler.WireMaskHotbar.ui_elements[8].IsActivated) // Only copy components if the corresponding mask is set
                        {
                            HashSet<Component> FoundComponents = new HashSet<Component>();
                            for (int x = 0; x < Selection_Size.X; ++x)
                            {
                                for (int y = 0; y < Selection_Size.Y; ++y)
                                {
                                    int xx = x + Selection_StartPos.X;
                                    int yy = y + Selection_StartPos.Y;
                                    if (Sim_Component.CompType[xx, yy] > 0)
                                        FoundComponents.Add(Sim_Component.components[Sim_Component.GetComponentID(new Point(xx, yy))]);
                                }
                            }
                            Component[] comps = FoundComponents.ToArray();
                            Rectangle SelectionRec = new Rectangle(Selection_StartPos, Selection_Size);
                            int parametercount = 0, compcount = 0;
                            for (int i = 0; i < comps.Length; ++i)
                            {
                                CompData compdata = Sim_Component.Components_Data[comps[i].dataID];
                                Rectangle comprec = Sim_Component.Components_Data[comps[i].dataID].bounds[comps[i].rotation];
                                comprec.Location += comps[i].pos;
                                Rectangle intersection = Rectangle.Intersect(SelectionRec, comprec);
                                if (intersection.Width == comprec.Width && intersection.Height == comprec.Height)
                                {
                                    compcount++;
                                    parametercount += compdata.valuebox_length;
                                }
                            }
                            CopiedParameterStates_Indices = new int[compcount + 1];
                            CopiedParameterStates = new int[parametercount + 1];
                            parametercount = 0;
                            for (int i = 0; i < comps.Length; ++i)
                            {
                                CompData compdata = Sim_Component.Components_Data[comps[i].dataID];
                                Rectangle comprec = Sim_Component.Components_Data[comps[i].dataID].bounds[comps[i].rotation];
                                comprec.Location += comps[i].pos;
                                Rectangle intersection = Rectangle.Intersect(SelectionRec, comprec);
                                if (intersection.Width == comprec.Width && intersection.Height == comprec.Height)
                                {
                                    CopiedCompIDs.Add(comps[i].dataID);
                                    CopiedCompRot.Add(comps[i].rotation);
                                    CopiedCompPos.Add(comps[i].pos - Selection_StartPos);
                                    CopiedParameterStates_Indices[CopiedCompIDs.Count] = parametercount;
                                    for(int j = 0; j < compdata.valuebox_length; ++j)
                                    {
                                        CopiedParameterStates[parametercount + j] = comps[i].internalstates[compdata.internalstate_length + compdata.OverlaySeg_length + j];
                                    }
                                    parametercount += compdata.valuebox_length;
                                    List<ComponentPixel> pixels = compdata.data[comps[i].rotation];
                                    for (int j = 0; j < pixels.Count; ++j)
                                    {
                                        int x = (pixels[j].pos.X + comps[i].pos.X) - Selection_StartPos.X;
                                        int y = (pixels[j].pos.Y + comps[i].pos.Y) - Selection_StartPos.Y;
                                        CopiedCompType[x + y * Selection_Size.X] = pixels[j].type;
                                    }
                                }
                            }
                            //CopiedParameterStates = new int[parametercount + 1];
                        }
                    }
                    if (Game1.kb_states.IsKeyToggleDown(Keys.Delete))
                    {
                        selectstate = copystate = 0;

                        byte[,] data = new byte[Selection_Size.X, Selection_Size.Y];
                        Extensions.GetArea(IsWire, data, new Rectangle(Selection_StartPos, Selection_Size));
                        byte layers = GetUILayers();
                        for (int x = 0; x < Selection_Size.X; ++x)
                        {
                            for (int y = 0; y < Selection_Size.Y; ++y)
                            {
                                data[x, y] &= (byte)(~layers);
                            }
                        }
                        PlaceArea(new Rectangle(Selection_StartPos, Selection_Size), data);

                        if (UI_Handler.WireMaskHotbar.ui_elements[8].IsActivated) // Only delete components if the corresponding mask is set
                        {
                            HashSet<Component> FoundComponents = new HashSet<Component>();
                            for (int x = 0; x < Selection_Size.X; ++x)
                            {
                                for (int y = 0; y < Selection_Size.Y; ++y)
                                {
                                    int xx = x + Selection_StartPos.X;
                                    int yy = y + Selection_StartPos.Y;
                                    if (Sim_Component.CompType[xx, yy] > 0)
                                        FoundComponents.Add(Sim_Component.components[Sim_Component.GetComponentID(new Point(xx, yy))]);
                                }
                            }
                            Component[] comps = FoundComponents.ToArray();
                            Rectangle SelectionRec = new Rectangle(Selection_StartPos, Selection_Size);
                            for (int i = 0; i < comps.Length; ++i)
                            {
                                CompData compdata = Sim_Component.Components_Data[comps[i].dataID];
                                Rectangle comprec = Sim_Component.Components_Data[comps[i].dataID].bounds[comps[i].rotation];
                                comprec.Location += comps[i].pos;
                                Rectangle intersection = Rectangle.Intersect(SelectionRec, comprec);
                                if (intersection.Width == comprec.Width && intersection.Height == comprec.Height)
                                {
                                    comps[i].Delete();
                                }
                            }
                        }
                    }
                }

                if (Game1.kb_states.New.IsKeyDown(Keys.LeftControl))
                {
                    // Starting Selection
                    if (IsInGrid && Game1.mo_states.IsLeftButtonToggleOn() && (selectstate == 0 || selectstate == 2))
                    {
                        selectstate = 1;
                        Selection_StartPos = mo_worldpos;
                    }



                    // Placing Copy Shadow
                    if (CopiedIsWire != null)
                    {
                        if (Game1.kb_states.New.AreKeysDown(Keys.LeftControl, Keys.V) && !Game1.kb_states.Old.AreKeysDown(Keys.LeftControl, Keys.V))
                        {
                            selectstate = 4;
                            copystate = 0;
                            if (copyWiretex != null && !copyWiretex.IsDisposed)
                                copyWiretex.Dispose();
                            if (copyComptex != null && !copyComptex.IsDisposed)
                                copyComptex.Dispose();
                            copyWiretex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                            copyComptex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                            copyWiretex.SetData(CopiedIsWire);
                            copyComptex.SetData(CopiedCompType);
                            copypos = new Point(mo_worldposx, mo_worldposy);
                            copypos.X = MathHelper.Clamp(copypos.X, MINCOO, MAXCOO - copysize.X);
                            copypos.Y = MathHelper.Clamp(copypos.Y, MINCOO, MAXCOO - copysize.Y);
                        }
                    }
                }
                // Moving Copy
                if (selectstate == 4)
                {
                    if(Game1.kb_states.IsKeyToggleDown(Keys.R)) // Rotating Copy
                    {
                        byte[] newCopiedIsWire = new byte[CopiedIsWire.Length];
                        byte[] newCopiedCompType = new byte[CopiedCompType.Length];
                        Point newcopysize = new Point(copysize.Y, copysize.X);
                        for(int x = 0; x < newcopysize.X; ++x)
                        {
                            for (int y = 0; y < newcopysize.Y; ++y)
                            {
                                newCopiedIsWire[x + y * newcopysize.X] = CopiedIsWire[(y) + (newcopysize.X - x - 1) * copysize.X];
                                newCopiedCompType[x + y * newcopysize.X] = CopiedCompType[(y) + (newcopysize.X - x - 1) * copysize.X];
                            }
                        }
                        copysize = newcopysize;
                        CopiedIsWire = newCopiedIsWire;
                        CopiedCompType = newCopiedCompType;
                        if (copyWiretex != null && !copyWiretex.IsDisposed)
                            copyWiretex.Dispose();
                        if (copyComptex != null && !copyComptex.IsDisposed)
                            copyComptex.Dispose();
                        copyWiretex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyComptex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyWiretex.SetData(CopiedIsWire);
                        copyComptex.SetData(CopiedCompType);
                        for (int i = 0; i < CopiedCompIDs.Count; ++i)
                        {
                            CopiedCompRot[i] = CompData.rottable_ROT[CopiedCompRot[i]];
                            CopiedCompPos[i] = new Point(copysize.X - CopiedCompPos[i].Y - 1, CopiedCompPos[i].X);
                        }
                    }

                    if (Game1.kb_states.IsKeyToggleDown(Keys.X)) // Flipping Copy X
                    {
                        byte[] newCopiedIsWire = new byte[CopiedIsWire.Length];
                        byte[] newCopiedCompType = new byte[CopiedCompType.Length];
                        Point newcopysize = new Point(copysize.X, copysize.Y);
                        for (int x = 0; x < newcopysize.X; ++x)
                        {
                            for (int y = 0; y < newcopysize.Y; ++y)
                            {
                                newCopiedIsWire[x + y * newcopysize.X] = CopiedIsWire[(copysize.X - x - 1) + y * copysize.X];
                                newCopiedCompType[x + y * newcopysize.X] = CopiedCompType[(copysize.X - x - 1) + y * copysize.X];
                            }
                        }
                        
                        copysize = newcopysize;
                        CopiedIsWire = newCopiedIsWire;
                        CopiedCompType = newCopiedCompType;
                        if (copyWiretex != null && !copyWiretex.IsDisposed)
                            copyWiretex.Dispose();
                        if (copyComptex != null && !copyComptex.IsDisposed)
                            copyComptex.Dispose();
                        copyWiretex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyComptex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyWiretex.SetData(CopiedIsWire);
                        copyComptex.SetData(CopiedCompType);
                        for (int i = 0; i < CopiedCompIDs.Count; ++i)
                        {
                            CopiedCompRot[i] = CompData.rottable_FLIPX[CopiedCompRot[i]];
                            CopiedCompPos[i] = new Point(copysize.X - CopiedCompPos[i].X - 1, CopiedCompPos[i].Y);
                        }
                    }
                    if (Game1.kb_states.IsKeyToggleDown(Keys.Y)) // Flipping Copy Y
                    {
                        byte[] newCopiedIsWire = new byte[CopiedIsWire.Length];
                        byte[] newCopiedCompType = new byte[CopiedCompType.Length];
                        Point newcopysize = new Point(copysize.X, copysize.Y);
                        for (int x = 0; x < newcopysize.X; ++x)
                        {
                            for (int y = 0; y < newcopysize.Y; ++y)
                            {
                                newCopiedIsWire[x + y * newcopysize.X] = CopiedIsWire[x + (copysize.Y - y - 1) * copysize.X];
                                newCopiedCompType[x + y * newcopysize.X] = CopiedCompType[x + (copysize.Y - y - 1) * copysize.X];
                            }
                        }

                        copysize = newcopysize;
                        CopiedIsWire = newCopiedIsWire;
                        CopiedCompType = newCopiedCompType;
                        if (copyWiretex != null && !copyWiretex.IsDisposed)
                            copyWiretex.Dispose();
                        if (copyComptex != null && !copyComptex.IsDisposed)
                            copyComptex.Dispose();
                        copyWiretex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyComptex = new Texture2D(Game1.graphics.GraphicsDevice, copysize.X, copysize.Y, false, SurfaceFormat.Alpha8);
                        copyWiretex.SetData(CopiedIsWire);
                        copyComptex.SetData(CopiedCompType);
                        for (int i = 0; i < CopiedCompIDs.Count; ++i)
                        {
                            CopiedCompRot[i] = CompData.rottable_FLIPY[CopiedCompRot[i]];
                            CopiedCompPos[i] = new Point(CopiedCompPos[i].X, copysize.Y - CopiedCompPos[i].Y - 1);
                        }
                    }


                    if (Game1.mo_states.IsLeftButtonToggleOn() && (new Rectangle(copypos, copysize)).Contains(mo_worldpos))
                    {
                        copystate = 1;
                        copymouseoffset = copypos - mo_worldpos;
                    }
                    if (copystate == 1)
                    {
                        if (Game1.mo_states.New.LeftButton == ButtonState.Released)
                            copystate = 0;

                        copypos = mo_worldpos + copymouseoffset;
                        copypos.X = MathHelper.Clamp(copypos.X, MINCOO, MAXCOO - copysize.X);
                        copypos.Y = MathHelper.Clamp(copypos.Y, MINCOO, MAXCOO - copysize.Y);
                    }

                    // Placing Copy
                    if (Game1.kb_states.IsKeyToggleDown(Keys.Enter))
                    {
                        // Check if placement is valid
                        bool IsValid = true;
                        for (int x = 0; x < copysize.X; ++x)
                        {
                            for (int y = 0; y < copysize.Y; ++y)
                            {
                                int xx = x + copypos.X;
                                int yy = y + copypos.Y;
                                if (CopiedIsWire[x + y * copysize.X] > 0 && Sim_Component.CompType[xx, yy] > 0 && Sim_Component.CompType[xx, yy] <= Sim_Component.PINOFFSET)
                                    IsValid = false;
                                if (CopiedCompType[x + y * copysize.X] > 0 && CopiedCompType[x + y * copysize.X] <= Sim_Component.PINOFFSET && IsWire[xx, yy] > 0)
                                    IsValid = false;
                                if (CopiedCompType[x + y * copysize.X] > Sim_Component.PINOFFSET && (Sim_Component.CompType[xx, yy] > 0 || IsWire[xx, yy] > 0))
                                    IsValid = false;
                            }
                        }

                        if (IsValid)
                        {
                            selectstate = copystate = 0;
                            byte[,] data = new byte[copysize.X, copysize.Y];
                            Extensions.GetArea(IsWire, data, new Rectangle(copypos, copysize));
                            for (int x = 0; x < copysize.X; ++x)
                            {
                                for (int y = 0; y < copysize.Y; ++y)
                                {
                                    data[x, y] |= CopiedIsWire[x + y * copysize.X];
                                }
                            }
                            PlaceArea(new Rectangle(copypos, copysize), data);
                            for (int i = 0; i < CopiedCompIDs.Count; ++i)
                            {
                                Component comp = Sim_Component.ComponentDropAtPos(CopiedCompIDs[i], CopiedCompPos[i] + copypos, CopiedCompRot[i]);
                                CompData compdata = Sim_Component.Components_Data[CopiedCompIDs[i]];
                                if (comp != null)
                                {
                                    for (int j = 0; j < compdata.valuebox_length; ++j)
                                    {
                                        comp.internalstates[compdata.internalstate_length + compdata.OverlaySeg_length + j] = CopiedParameterStates[CopiedParameterStates_Indices[i] + j];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Handling Tool Modes
            if (UI_Handler.UI_Active_State == 0 && toolmode == TOOL_WIRE && !IsSimulating && selectstate == 0)
            {

                if (IsValidPlacementCoo(mo_worldpos) && Game1.mo_states.New.RightButton == ButtonState.Pressed)
                {
                    FileHandler.IsUpToDate = false;
                    byte[,] data = new byte[1, 1];
                    byte wiredata = IsWire[mo_worldposx, mo_worldposy];
                    data[0, 0] = IsWire[mo_worldposx, mo_worldposy];
                    data[0, 0] &= (byte)~GetUILayers();
                    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 1, 1), data);
                }

                if (!IsSimulating && IsInGrid && (IsWire[mo_worldposx, mo_worldposy] & (1 << currentlayer)) > 0 && Game1.kb_states.IsKeyToggleDown(Keys.L))
                {
                    networks[WireIDs[mo_worldposx / 2, mo_worldposy / 2, currentlayer]].state ^= 1;
                }

                // Placing Wires
                if (IsValidPlacementCoo(mo_worldpos) && Game1.mo_states.New.LeftButton == ButtonState.Pressed)
                {
                    FileHandler.IsUpToDate = false;
                    byte layers = GetUILayers();
                   
                    byte[,] data = new byte[1, 1];
                    data[0, 0] = IsWire[mo_worldposx, mo_worldposy];
                    data[0, 0] |= (byte)GetUILayers();
                    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 1, 1), data);

                }

                // Placing Via
                if (IsValidPlacementCoo(mo_worldpos) && Game1.mo_states.IsMiddleButtonToggleOn())
                {
                    FileHandler.IsUpToDate = false;
                    byte[,] data = new byte[1, 1];
                    data[0, 0] = IsWire[mo_worldposx, mo_worldposy];
                    data[0, 0] |= 128;
                    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 1, 1), data);

                    Line line = new Line(new Point(mo_worldposx, mo_worldposy), new Point(mo_worldposx, mo_worldposy));
                    VertexPositionLine line1, line2;
                    line.Convert2LineVertices(data[0, 0], out line1, out line2);

                    lines2draw[LAYER_NUM][lines2draw_count[LAYER_NUM]++] = line1;
                    lines2draw[LAYER_NUM][lines2draw_count[LAYER_NUM]++] = line2;

                  
                }


            }
            else if(UI_Handler.UI_Active_State == 0 && toolmode == TOOL_SELECT && selectstate == 0)
            {
                if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0)
                {
                    int typeID = Sim_Component.CompNetwork[mo_worldposx, mo_worldposy];
                    int compID = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32][typeID];
                    Sim_Component.components[compID].Clicked();
                }
                if (!IsSimulating)
                {
                    if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0 && Game1.mo_states.IsRightButtonToggleOff())
                    {
                        int typeID = Sim_Component.CompNetwork[mo_worldposx, mo_worldposy];
                        int[] arr = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32];
                        int compID = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32][typeID];
                        Sim_Component.components[compID].Delete();
                    }

                    if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0 && Game1.mo_states.IsLeftButtonToggleOff())
                    {
                        int typeID = Sim_Component.CompNetwork[mo_worldposx, mo_worldposy];
                        int[] arr = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32];
                        int compID = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32][typeID];
                        if (Sim_Component.Components_Data[Sim_Component.components[compID].dataID].valuebox_length > 0)
                            UI_Handler.parameterWindow.SetRootcomp(Sim_Component.components[compID]);
                    }

                    
                }

            }
            if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0 && toolmode == TOOL_SELECT && selectstate == 0 && UI_Handler.UI_Active_State == 0)
            {

                int typeID = Sim_Component.CompNetwork[mo_worldposx, mo_worldposy];
                int compID = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32][typeID];
                UI_Handler.GridInfo.values.ui_elements[0].setValue(Sim_Component.Components_Data[Sim_Component.components[compID].dataID].name);
                UI_Handler.GridInfo.ShowInfo();
            }
            else
                UI_Handler.GridInfo.HideInfo();

            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                sim_effect.Parameters["zoom"].SetValue((float)Math.Pow(2, worldzoom));
                sim_effect.Parameters["coos"].SetValue(worldpos.ToVector2());
                sim_effect.Parameters["mousepos_X"].SetValue(mo_worldposx);
                sim_effect.Parameters["mousepos_Y"].SetValue(mo_worldposy);
                sim_effect.Parameters["selectstate"].SetValue(selectstate);
                if(selectstate >= 1 && selectstate <= 2)
                {
                    Point endpos = new Point(MathHelper.Clamp(mo_worldposx, MINCOO, MAXCOO), MathHelper.Clamp(mo_worldposy, MINCOO, MAXCOO));
                    Point start = new Point(Math.Min(Selection_StartPos.X, endpos.X), Math.Min(Selection_StartPos.Y, endpos.Y));
                    Point end = new Point(Math.Max(Selection_StartPos.X, endpos.X), Math.Max(Selection_StartPos.Y, endpos.Y));
                    if(selectstate == 2)
                    {
                        start = Selection_StartPos;
                        end = Selection_EndPos;
                    }
                    sim_effect.Parameters["selection_startX"].SetValue(start.X);
                    sim_effect.Parameters["selection_startY"].SetValue(start.Y);
                    sim_effect.Parameters["selection_endX"].SetValue(end.X);
                    sim_effect.Parameters["selection_endY"].SetValue(end.Y);
                }
                if(selectstate == 4)
                {
                    sim_effect.Parameters["copywiretex"].SetValue(copyWiretex);
                    sim_effect.Parameters["copycomptex"].SetValue(copyComptex);
                    sim_effect.Parameters["copyposX"].SetValue(copypos.X);
                    sim_effect.Parameters["copyposY"].SetValue(copypos.Y);
                    sim_effect.Parameters["selection_endX"].SetValue(copypos.X + copysize.X - 1);
                    sim_effect.Parameters["selection_endY"].SetValue(copypos.Y + copysize.Y - 1);
                }
                sim_effect.Parameters["mindist"].SetValue(0.13f);
                sim_effect.Parameters["currentlayer"].SetValue(currentlayer);
            }

            sim_comp.Update();

            // Simulate Circuit
            if(IsSimulating)
            {
                sim_inf_dll.SimulateOneStep();
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.End();
            //Check Networks for Drawing
            for(int i = 0; i < Network.CheckForDrawing.Count; ++i)
            {
                if(networks[Network.CheckForDrawing[i].ID] == Network.CheckForDrawing[i])
                    Network.CheckForDrawing[i].Draw();
            }
            if (Network.CheckForDrawing.Count > 0)
                Network.CheckForDrawing.Clear();

            for (int j = -1; j < LAYER_NUM + 1; ++j)
            {
                int i = j;
                if (i == -1)
                    i = LAYER_NUM + 1;
                if (lines2draw_count[i] > 0)
                {

                    Game1.graphics.GraphicsDevice.SetRenderTarget(sec_target);
                    line_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
                    line_effect.Parameters["tex"].SetValue(logic_target);
                    line_effect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines2draw[i], 0, lines2draw_count[i] / 2);

                    Game1.graphics.GraphicsDevice.SetRenderTarget(logic_target);
                    line_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
                    line_effect.Parameters["tex"].SetValue(sec_target);
                    line_effect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines2draw[i], 0, lines2draw_count[i] / 2);

                    Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                  
                }
            }

            for (int j = -1; j < LAYER_NUM + 1; ++j)
            {
                int i = j;
                if (i == -1)
                    i = LAYER_NUM + 1;
                if (lines2draw_count[i] > 0)
                {
                    Game1.graphics.GraphicsDevice.SetRenderTarget(WireCalc_target);
                    iswirerender_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
                    iswirerender_effect.Parameters["tex"].SetValue(logic_target);
                    iswirerender_effect.CurrentTechnique.Passes[0].Apply();
                    Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines2draw[i], 0, lines2draw_count[i] / 2);

                    Game1.graphics.GraphicsDevice.SetRenderTarget(null);
                    lines2draw_count[i] = 0;
           
                }
            }
           

            sim_comp.DrawLineOverlays(spritebatch);
            sim_comp.Draw(spritebatch);
            

         
            sim_effect.Parameters["logictex"].SetValue(logic_target);
            sim_effect.Parameters["wirecalctex"].SetValue(WireCalc_target);
            sim_effect.Parameters["isedgetex"].SetValue(Sim_Component.IsEdgeTex);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, sim_effect, Matrix.Identity);
            spritebatch.Draw(main_target, Vector2.Zero, Color.White);
            spritebatch.End();


            spritebatch.Begin();
           
            sim_comp.DrawCompOverlays(spritebatch);

         
        }
    }
}
