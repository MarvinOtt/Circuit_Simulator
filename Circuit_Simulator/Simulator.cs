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
            else// if (start.X > end.X && start.Y > end.Y)
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
        public List<Line_Netw> lines;
        public int ID;
        public byte state;

        public Network(int ID)
        {
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
                    Simulator.emptyNetworkID[Simulator.emptyNetworkID_count++] = net;
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
                    if (Simulator.IsChange[x, y] > 0)
                        Simulator.IsWire[x, y] &= (byte)~(lines[i].layers);
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

                    curpos += lines[i].dir;
                }
            }

            Draw();
        }

        public void Draw()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                //if (lines[i].layers >)
                //{
                //    Simulator.lines2draw[Simulator.LAYER_NUM][Simulator.lines2draw_count[Simulator.LAYER_NUM]++] = new VertexPositionLine(lines[i].start, 255);
                //    Simulator.lines2draw[Simulator.LAYER_NUM][Simulator.lines2draw_count[Simulator.LAYER_NUM]++] = new VertexPositionLine(lines[i].end + lines[i].dir, 255);
                //    continue;
                //}
                for (int j = 0; j < Simulator.LAYER_NUM + 1; ++j) // Iterating all Layers
                {
                    if ((lines[i].layers & (1 << j)) > 0)
                    {
                        Line line = new Line(lines[i].start, lines[i].end);
                        VertexPositionLine line1, line2;
                        line.Convert2LineVertices((1 << j), out line1, out line2);
                        Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = line1;// new VertexPositionLine(lines[i].start, (1 << j));
                        Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = line2;// new VertexPositionLine(lines[i].end + lines[i].dir, (1 << j));
                    }
                }
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
        //public const int TOOL_SELECT = 2;

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
        RenderTarget2D main_target, WireCalc_target;
        public static RenderTarget2D logic_target, sec_target;
        Network CalcNetwork;

        public static Network[] networks;
        public static VertexPositionLine[][] lines2draw;
        public static int[] lines2draw_count;
        public static Matrix linedrawingmatrix;
        public static int highestNetworkID = 3;
        public static int[] emptyNetworkID;
        public static int emptyNetworkID_count;
        public static byte[,] IsWire, CalcGridData, CalcGridStat, IsChange;

        public static HashSet<int> FoundNetworks;
        int[] CalcOccurNetw;
        byte[] revshiftarray;
        int CalcOccurNetw_Pos;
        public static int[,,] WireIDs;

        public static Point worldpos;
        public static int worldzoom = 0;

        public static int toolmode = TOOL_WIRE, oldtoolmode = 0, simspeed, simspeed_count;
        public static bool IsSimulating;

        #region INPUT

        public static int currentlayer;
        int mo_worldposx, mo_worldposy;
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
            //Game1.graphics.GraphicsDevice.SetRenderTarget(sec_target);
            //Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
            //Game1.graphics.GraphicsDevice.SetRenderTarget(logic_target);
            //Game1.graphics.GraphicsDevice.Clear(Color.Transparent);
            //Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            FoundNetworks = new HashSet<int>();
            IsWire = new byte[SIZEX, SIZEY];
            CalcGridData = new byte[SIZEX, SIZEY];
            CalcGridStat = new byte[SIZEX, SIZEY];
            IsChange = new byte[SIZEX, SIZEY];
            CalcOccurNetw = new int[10000];
            WireIDs = new int[SIZEX / 2, SIZEY / 2, LAYER_NUM];
            networks = new Network[10000000];
            //networks[0] = new Network(0);
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

        // DLL functions
        [DllImport("..\\..\\..\\..\\..\\x64\\Debug\\Sim_DLL.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int Test(int a, int b);

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

        public void DoFFIfValid(int x, int y, byte curval, byte curcomptype, int curcompid)
        {
            byte nextval = IsWire[x, y];
            byte finalval = (byte)(curval & nextval & 0x7F);
            //if (nextval >= 128 && finalval != 0)
            //finalval = nextval;
            if (curval > 128)
            {
                byte comptype_next = Sim_Component.CompType[x, y];
                byte gridid = Sim_Component.CompNetwork[x, y];
                int[] arr = Sim_Component.CompGrid[x / 32, y / 32];
                int compid_next = arr[gridid];
                if (curcomptype == comptype_next && curcompid == compid_next)
                    finalval |= 0x80;
            }
            if (finalval > 0 && ((CalcGridData[x, y] ^ finalval) & finalval) > 0)
                FloodFillCellAndNeighbours(x, y, finalval);

            //if (((curval & nextval) == nextval || nextval == 255) && CalcGridData[x, y] != nextval)
            //    FloodFillCellAndNeighbours(x, y, (nextval == 255) ? 255 : (curval & nextval));
        }
        public void FloodFillCellAndNeighbours(int x, int y, int mask)
        {
            byte curval = (byte)(IsWire[x, y] & mask);
            if (IsWire[x, y] >= 128)
                curval = IsWire[x, y];
            CalcGridData[x, y] |= curval;
            byte comptype_cur = Sim_Component.CompType[x, y];
            byte gridid = Sim_Component.CompNetwork[x, y];
            int compid_cur = Sim_Component.CompGrid[x / 32, y / 32][gridid];

            if (curval != 0)
            {
                if (Sim_Component.CompType[x, y] > 1)
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
                    int netID = WireIDs[x / 2, y / 2, revshiftarray[curval & 0x7F]];
                    if ((CalcOccurNetw_Pos == 0 || CalcOccurNetw[CalcOccurNetw_Pos - 1] != netID) && netID > 3)
                        CalcOccurNetw[CalcOccurNetw_Pos++] = netID;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int MakeLineDir(int x, int y)
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
        public void Check4NewLine(int x, int y, Point dirvec)
        {
            for(int xx = -1; xx < 2; ++xx)
            {
                for (int yy = -1; yy < 2; ++yy)
                {
                    if(!(xx == 0 && yy == 0))// && !((xx == dirvec.X && yy == dirvec.Y) || (xx == -dirvec.X && yy == -dirvec.Y)))
                    {
                        if (CalcGridStat[x + xx, y + yy] != CalcGridData[x + xx, y + yy] && CalcGridData[x + xx, y + yy] != 0)
                            CalculateLine(x + xx, y + yy);
                        byte d = CalcGridData[x + xx, y + yy];
                    }
                }
            }
        }
        public void CalculateLine(int x, int y)
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
        public int CalculateNewNetwork(int x, int y, int layermask)
        {
            int curvalue = CalcGridData[x, y] & layermask;
            if (curvalue == 0)
                return -1;
            int ID = highestNetworkID + 1;
            if (emptyNetworkID_count > 0)
                ID = emptyNetworkID[--emptyNetworkID_count];
            CalcNetwork = new Network(ID);
            CalculateLine(x, y);
            networks[ID] = CalcNetwork;
            return ID;
        }

        public void CalculateNetworkAt(int x, int y, byte layermask)
        {
            CalcOccurNetw_Pos = 0;
            Array.Clear(CalcOccurNetw, 0, CalcOccurNetw.Length);

            // Flood Fill CalcGrid
            FloodFillCellAndNeighbours(x, y, layermask);

            // Delete previous networks
            for(int i = 0; i < CalcOccurNetw_Pos; ++i)
            {
                FoundNetworks.Add(CalcOccurNetw[i]);
            }   

            // Main Line Algorithm
            int netID = CalculateNewNetwork(x, y, layermask);
            if (netID == -1)
                return;

            // Clear Calc Grid
            networks[netID].ClearCalcGrid();
            networks[netID].Draw();
        }

        

        public void PlaceArea(Rectangle rec, byte[,] data)
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

            // Removing the Networks
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    for (int i = 0; i < LAYER_NUM; ++i)
                    {
                        if (WireIDs[x / 2, y / 2, i] != 0 && (IsWire[x, y] & (1 << i)) > 0)
                        {
                            nets.Add(WireIDs[x / 2, y / 2, i]);
                            WireIDs[x / 2, y / 2, i] = 0;
                        }
                    }
                }
            }

            // Placing Data
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    IsWire[x, y] = finaldata[(x - Brec.Left), (y - Brec.Top)];
                    IsChange[x, y] = 255;
                }
            }

            //Drawing Area Black
            for (int y = 0; y < rec.Height; ++y)
            {
                Line line = new Line(new Point(rec.Left, rec.Top + y), new Point(rec.Right - 1, rec.Top + y));
                VertexPositionLine line1, line2;
                line.Convert2LineVertices(0, out line1, out line2);

                lines2draw[LAYER_NUM + 1][lines2draw_count[LAYER_NUM + 1]++] = line1;// new VertexPositionLine(new Point(rec.Left, rec.Top + y), 0);
                lines2draw[LAYER_NUM + 1][lines2draw_count[LAYER_NUM + 1]++] = line2;// new VertexPositionLine(new Point(rec.Right, rec.Top + y), 0);
            }

            // Main Network Calculations
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    if (IsWire[x, y] > 128)
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

            // Reset Data
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    IsWire[x, y] = finaldata[(x - Brec.Left), (y - Brec.Top)];
                    IsChange[x, y] = 0;
                }
            }

            Network.Delete(nets);

            Network.Delete(FoundNetworks);
            FoundNetworks.Clear();
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
                OUT |= (byte)(Convert.ToByte(UI_Handler.wire_ddbl.ui_elements[i].IsActivated) << i);
            }
            if (UI_Handler.wire_ddbl.ui_elements[7].IsActivated)
                return 255;
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
            //int r = Test(4, 8);


            Screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out mo_worldposx, out mo_worldposy);
            Point mo_worldpos = new Point(mo_worldposx, mo_worldposy);
            IsInGrid = mo_worldposx > 0 && mo_worldposy > 0 && mo_worldposx < SIZEX - 1 && mo_worldposy < SIZEY - 1;

            if(UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
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
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = true;
                }
                if (Game1.kb_states.IsKeyToggleDown(Keys.Subtract))
                {
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = false;
                    currentlayer = MathHelper.Clamp(--currentlayer, 0, LAYER_NUM - 1);
                    (UI_Handler.LayerSelectHotbar.ui_elements[currentlayer] as UI_TexButton).IsActivated = true;
                }

                UI_Handler.GeneralInfoBox.ui_elements[0].value = "Pos: X: " + mo_worldposx.ToString() + " Y: " + mo_worldposy.ToString();
                UI_Handler.GeneralInfoBox.ui_elements[1].value = "Speed: 2^" + simspeed.ToString() + " (" + Math.Pow(2, simspeed).ToString() + ")";

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

            // Handling Tool Modes
            //if(Game1.kb_states.IsKeyToggleDown(Keys.D1))
                

            if (UI_Handler.UI_Active_State == 0 && toolmode == TOOL_WIRE && !IsSimulating)
            {

                // Deleting Wires
                //if (IsInGrid && Game1.mo_states.New.RightButton == ButtonState.Pressed && Game1.kb_states.New.IsKeyDown(Keys.LeftAlt))
                //{
                //    byte[,] data = new byte[10, 10];
                //    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 10, 10), data);
                //}
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
                    IsWire[mo_worldposx, mo_worldposy] |= layers;
                    if (true)//layers != 255)
                    {
                        for (int i = 0; i < LAYER_NUM + 1; ++i)
                            if ((layers & (1 << i)) > 0)
                                CalculateNetworkAt(mo_worldposx, mo_worldposy, (byte)(layers & (1 << i)));
                    }
                    //else
                    //    CalculateNetworkAt(mo_worldposx, mo_worldposy, 255);
                    Network.Delete(FoundNetworks);
                    FoundNetworks.Clear();

                }

                // Placing Via
                if (IsValidPlacementCoo(mo_worldpos) && Game1.mo_states.IsMiddleButtonToggleOn())
                {
                    FileHandler.IsUpToDate = false;
                    IsWire[mo_worldposx, mo_worldposy] = 128;
                    CalculateNetworkAt(mo_worldposx, mo_worldposy, 128);
                    Network.Delete(FoundNetworks);
                    FoundNetworks.Clear();
                }


            }
            else if(UI_Handler.UI_Active_State == 0 && toolmode == TOOL_SELECT)
            {
                if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0 && Game1.mo_states.IsLeftButtonToggleOff())
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

                    if (IsInGrid && Sim_Component.CompType[mo_worldposx, mo_worldposy] != 0)
                    {
                        int typeID = Sim_Component.CompNetwork[mo_worldposx, mo_worldposy];
                        int compID = Sim_Component.CompGrid[mo_worldposx / 32, mo_worldposy / 32][typeID];
                        UI_Handler.info.values.ui_elements[0].setValue(Sim_Component.Components_Data[Sim_Component.components[compID].dataID].name);
                        UI_Handler.info.ShowInfo();
                    }
                    else
                        UI_Handler.info.HideInfo();
                }

            }

            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                sim_effect.Parameters["zoom"].SetValue((float)Math.Pow(2, worldzoom));
                sim_effect.Parameters["coos"].SetValue(worldpos.ToVector2());
                sim_effect.Parameters["mousepos_X"].SetValue(mo_worldposx);
                sim_effect.Parameters["mousepos_Y"].SetValue(mo_worldposy);
                //sim_effect.Parameters["mindist"].SetValue(MathHelper.Clamp((float)(4.0f / Math.Pow(2, worldzoom)), 0.125f / 2, 0.125f));
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
                    //lines2draw_count[i] = 0;
                    //WireCalc_target
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
                    //WireCalc_target
                }
            }
            //VertexPositionLine[] vertexes = new VertexPositionLine[]
            //{
            //    new VertexPositionLine(new Point(3, 1), 1),
            //    new VertexPositionLine(new Point(1, 3), 1)
            //};

            //Game1.graphics.GraphicsDevice.SetRenderTarget(sec_target);
            //line_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
            //line_effect.Parameters["tex"].SetValue(logic_target);
            //line_effect.CurrentTechnique.Passes[0].Apply();
            //Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertexes, 0, 1);

            //Game1.graphics.GraphicsDevice.SetRenderTarget(logic_target);
            //line_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
            //line_effect.Parameters["tex"].SetValue(sec_target);
            //line_effect.CurrentTechnique.Passes[0].Apply();
            //Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertexes, 0, 1);

            //Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            //Game1.graphics.GraphicsDevice.SetRenderTarget(WireCalc_target);
            //iswirerender_effect.Parameters["WorldViewProjection"].SetValue(linedrawingmatrix);
            //iswirerender_effect.Parameters["tex"].SetValue(logic_target);
            //iswirerender_effect.CurrentTechnique.Passes[0].Apply();
            //Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertexes, 0, 1);

            //Game1.graphics.GraphicsDevice.SetRenderTarget(null);

            sim_comp.DrawLineOverlays(spritebatch);
            sim_comp.Draw(spritebatch);
            

            //sim_effect.Parameters["logictex_L1"].SetValue(logic_targets[0]);
            //sim_effect.Parameters["logictex_L2"].SetValue(logic_targets[1]);
            //sim_effect.Parameters["logictex_L3"].SetValue(logic_targets[2]);
            //sim_effect.Parameters["logictex_L4"].SetValue(logic_targets[3]);
            //sim_effect.Parameters["logictex_L5"].SetValue(logic_targets[4]);
            //sim_effect.Parameters["logictex_L6"].SetValue(logic_targets[5]);
            //sim_effect.Parameters["logictex_L7"].SetValue(logic_targets[6]);
            //sim_effect.Parameters["logictex_LV"].SetValue(logic_targets[7]);
            sim_effect.Parameters["logictex"].SetValue(logic_target);
            sim_effect.Parameters["wirecalctex"].SetValue(WireCalc_target);
            sim_effect.Parameters["isedgetex"].SetValue(Sim_Component.IsEdgeTex);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, sim_effect, Matrix.Identity);
            spritebatch.Draw(main_target, Vector2.Zero, Color.White);
            spritebatch.End();



            spritebatch.Begin();

            sim_comp.DrawCompOverlays(spritebatch);

            //spritebatch.DrawString(Game1.basefont, "Layer: " + currentlayer.ToString(), new Vector2(500, 100), Color.Red);
            if(IsInGrid && (IsWire[mo_worldposx, mo_worldposy] & (1 << currentlayer)) > 0)
                spritebatch.DrawString(Game1.basefont, "Network: " + WireIDs[mo_worldposx / 2, mo_worldposy / 2, currentlayer].ToString(), new Vector2(500, 130), Color.Red);
            if(IsInGrid && (IsWire[mo_worldposx, mo_worldposy] & (1 << currentlayer)) > 0)
            { 
                int state = 0;
                if (IsSimulating)
                    state = Sim_INF_DLL.GetWireState(WireIDs[mo_worldposx / 2, mo_worldposy / 2, currentlayer]);
                else
                {
                    int id = WireIDs[mo_worldposx / 2, mo_worldposy / 2, currentlayer];
                    state = networks[id].state;
                }
                spritebatch.DrawString(Game1.basefont, "State: " + state.ToString(), new Vector2(500, 160), Color.Red);
            }

        }
    }
}
