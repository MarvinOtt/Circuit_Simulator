﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Circuit_Simulator
{
    public struct Point16
    {
        short X, Y;
    }
    public struct Line
    {
        public byte layers;
        public int length;
        public Point start, end, dir;

        public Line(Point start, Point end, Point dir, int length, byte layers)
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
        public List<Line> lines;
        public int ID;

        public Network(int ID)
        {
            this.ID = ID;
            if (ID > Simulator.highestNetworkID)
                Simulator.highestNetworkID = ID;
            lines = new List<Line>();
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

        public void Draw()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                if (lines[i].layers == 255)
                {
                    Simulator.lines2draw[Simulator.LAYER_NUM][Simulator.lines2draw_count[Simulator.LAYER_NUM]++] = new VertexPositionLine(lines[i].start, 255);
                    Simulator.lines2draw[Simulator.LAYER_NUM][Simulator.lines2draw_count[Simulator.LAYER_NUM]++] = new VertexPositionLine(lines[i].end + lines[i].dir, 255);
                    continue;
                }
                for (int j = 0; j < Simulator.LAYER_NUM; ++j) // Iterating all Layers
                {
                    if ((lines[i].layers & (1 << j)) > 0)
                    {
                        Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = new VertexPositionLine(lines[i].start, (1 << j));
                        Simulator.lines2draw[j][Simulator.lines2draw_count[j]++] = new VertexPositionLine(lines[i].end + lines[i].dir, (1 << j));
                    }
                }
            }
        }
    }

    public class Component
    {
        int ID;
        int dataID;

        public Component()
        {

        }
    }
    public struct ComponentPixel
    {
        public byte type;
        public Point pos;

        public ComponentPixel(Point pos, byte type)
        {
            this.pos = pos;
            this.type = type;
        }
    }
    public class ComponentData
    {
        public List<ComponentPixel> data;
        string name;
        string catagory;

        public ComponentData(string name, string catagory)
        {
            this.name = name;
            data = new List<ComponentPixel>();
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
        public const int SIZEX = 10240;
        public const int SIZEY = 10240;
        public const int LAYER_NUM = 7;


        BasicEffect basicEffect;
        Effect sim_effect, line_effect;
        RenderTarget2D main_target;
        RenderTarget2D logic_target, sec_target;
        Texture2D componenttex;
        Network CalcNetwork;
        List<ComponentData> Components_Data;
        
        public static Network[] networks;
        public static VertexPositionLine[][] lines2draw;
        public static int[] lines2draw_count;
        public Matrix linedrawingmatrix;
        public static int highestNetworkID = 3;
        public static int[] emptyNetworkID;
        public static int emptyNetworkID_count;
        public static byte[,] IsWire, CalcGridData, CalcGridStat, IsChange;
        HashSet<int> FoundNetworks;
        int[] CalcOccurNetw;
        byte[] revshiftarray;
        int CalcOccurNetw_Pos;
        int[,,] WireIDs;

        Point worldpos;
        int worldzoom = 0;
        public bool IsCompDrag;

        int sim_speed = 1;

        #region INPUT

        int currentlayer;
        int mo_worldposx, mo_worldposy;
        bool IsInGrid;

        #endregion


        public Simulator()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;

            // Loading Effects
            sim_effect = Game1.content.Load<Effect>("sim_effect");
            line_effect = Game1.content.Load<Effect>("line_effect");

            // Initializing Render Targets
            sec_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            logic_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            componenttex = new Texture2D(Game1.graphics.GraphicsDevice, 41, 41, false, SurfaceFormat.Alpha8);

            FoundNetworks = new HashSet<int>();
            IsWire = new byte[SIZEX, SIZEY];
            CalcGridData = new byte[SIZEX, SIZEY];
            CalcGridStat = new byte[SIZEX, SIZEY];
            IsChange = new byte[SIZEX, SIZEY];
            CalcOccurNetw = new int[10000];
            WireIDs = new int[SIZEX / 2, SIZEY / 2, LAYER_NUM];
            networks = new Network[20000000];
            emptyNetworkID = new int[20000000];
            revshiftarray = new byte[256];
            for (int i = 0; i < 8; ++i)
                for(int j = 0; j < (1 << i); ++j)
                    revshiftarray[(1 << i) + j] = (byte)i;
            // Initializing Array for the Line Drawing
            lines2draw_count = new int[LAYER_NUM + 1];
            lines2draw = new VertexPositionLine[LAYER_NUM + 1][];
            for (int i = 0; i < LAYER_NUM + 1; ++i)
                lines2draw[i] = new VertexPositionLine[200000];
            linedrawingmatrix = Matrix.CreateOrthographicOffCenter(0, SIZEX, SIZEY, 0, 0, 1);

            // Basic Components Data
            Components_Data = new List<ComponentData>();
            Components_Data.Add(new ComponentData("AND", "Gates"));
            Components_Data[0].data.Add(new ComponentPixel(new Point(0, -1), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(0, 0), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(0, 1), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(1, -1), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(1, 0), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(1, 1), 1));
            Components_Data[0].data.Add(new ComponentPixel(new Point(-1, -1), 2));
            Components_Data[0].data.Add(new ComponentPixel(new Point(2, 0), 2));
            Components_Data[0].data.Add(new ComponentPixel(new Point(-1, 1), 2));
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

        public void screen2worldcoo_int(Vector2 screencoos, out int x, out int y)
        {
            x = (int)((screencoos.X - worldpos.X) / (float)Math.Pow(2, worldzoom));
            y = (int)((screencoos.Y - worldpos.Y) / (float)Math.Pow(2, worldzoom));
        }


        public void DoFFIfValid(int x, int y, byte curval)
        {
            byte nextval = IsWire[x, y];
            byte finalval = (byte)(curval & nextval);
            if (nextval == 255)
                finalval = 255;
            if(finalval > 0 && ((CalcGridData[x, y] ^ finalval) & finalval) > 0)
                FloodFillCellAndNeighbours(x, y, finalval);

            //if (((curval & nextval) == nextval || nextval == 255) && CalcGridData[x, y] != nextval)
            //    FloodFillCellAndNeighbours(x, y, (nextval == 255) ? 255 : (curval & nextval));
        }
        public void FloodFillCellAndNeighbours(int x, int y, int mask)
        {
            byte curval = (byte)(IsWire[x, y] & mask);
            CalcGridData[x, y] |= curval;


            if (curval != 0)
            {
                DoFFIfValid(x - 1, y, curval);
                DoFFIfValid(x + 1, y, curval);
                DoFFIfValid(x, y - 1, curval);
                DoFFIfValid(x, y + 1, curval);
                DoFFIfValid(x - 1, y - 1, curval);
                DoFFIfValid(x + 1, y - 1, curval);
                DoFFIfValid(x - 1, y + 1, curval);
                DoFFIfValid(x + 1, y + 1, curval);
                if(curval != 255)
                {
                    int netID = WireIDs[x / 2, y / 2, revshiftarray[curval]];
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
            CalcNetwork.lines.Add(new Line(start, end, dirvec, i + j + 1, linelayers));

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
                for (int i = 0; i < LAYER_NUM + 1; ++i)
                {
                    lines2draw[0][lines2draw_count[0]++] = new VertexPositionLine(new Point(rec.Left, rec.Top + y), 0);
                    lines2draw[0][lines2draw_count[0]++] = new VertexPositionLine(new Point(rec.Right, rec.Top + y), 0);
                }
            }

            // Main Network Calculations
            for (int x = Brec.Left; x < Brec.Right; ++x)
            {
                for (int y = Brec.Top; y < Brec.Bottom; ++y)
                {
                    for(int i = 0; i < LAYER_NUM; ++i)
                    {
                        if((IsWire[x, y] & (1 << i)) > 0)
                            CalculateNetworkAt(x, y, (byte)(1 << i));
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

        public void InizializeComponentDrag(int ID)
        {
            if(UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                byte[] data = new byte[41 * 41];
                List<ComponentPixel> datapixel = Components_Data[ID].data;
                for(int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 20) * 41 + (datapixel[i].pos.X + 20)] = datapixel[i].type;
                }

                componenttex.SetData(data);
                sim_effect.Parameters["currenttype"].SetValue(1);
                sim_effect.Parameters["comptex"].SetValue(componenttex);
            }
        }

        public void ComponentDrop(int ID)
        {
            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {

            }
            sim_effect.Parameters["currenttype"].SetValue(0);
        }

        public void Update()
        {
            screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out mo_worldposx, out mo_worldposy);
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
                if (Game1.kb_states.IsKeyToggleDown(Keys.Add))
                    currentlayer = MathHelper.Clamp(++currentlayer, 0, LAYER_NUM - 1);
                if (Game1.kb_states.IsKeyToggleDown(Keys.Subtract))
                    currentlayer = MathHelper.Clamp(--currentlayer, 0, LAYER_NUM - 1);

                if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
                {
                    if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue && worldzoom > -8) // Zooming Out
                    {
                        worldzoom -= 1;
                        Point diff = Game1.mo_states.New.Position - worldpos;
                        worldpos += new Point(diff.X / 2, diff.Y / 2);
                    }
                    else if (Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue && worldzoom < 8) // Zooming In
                    {
                        worldzoom += 1;
                        Point diff = Game1.mo_states.New.Position - worldpos;
                        worldpos -= diff;
                    }
                }
                #endregion
            }

            if (UI_Handler.UI_Active_State == 0)
            {

                // Deleting Wires
                if (IsInGrid && Game1.mo_states.New.RightButton == ButtonState.Pressed && Game1.kb_states.New.IsKeyDown(Keys.LeftAlt))
                {
                    byte[,] data = new byte[10, 10];
                    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 10, 10), data);
                }
                else if (IsInGrid && Game1.mo_states.New.RightButton == ButtonState.Pressed)
                {
                    byte[,] data = new byte[1, 1];
                    PlaceArea(new Rectangle(mo_worldposx, mo_worldposy, 1, 1), data);
                }

                // Placing Wires
                if (IsInGrid && Game1.mo_states.New.LeftButton == ButtonState.Pressed)
                {
                    IsWire[mo_worldposx, mo_worldposy] |= (byte)(1 << currentlayer);
                    CalculateNetworkAt(mo_worldposx, mo_worldposy, (byte)(1 << currentlayer));
                    Network.Delete(FoundNetworks);
                    FoundNetworks.Clear();

                }

                // Placing Via
                if (IsInGrid && Game1.mo_states.IsMiddleButtonToggleOn())
                {
                    IsWire[mo_worldposx, mo_worldposy] = 255;
                    CalculateNetworkAt(mo_worldposx, mo_worldposy, 255);
                    Network.Delete(FoundNetworks);
                    FoundNetworks.Clear();
                }

            }
            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                sim_effect.Parameters["zoom"].SetValue((float)Math.Pow(2, worldzoom));
                sim_effect.Parameters["coos"].SetValue(worldpos.ToVector2());
                sim_effect.Parameters["mousepos_X"].SetValue(mo_worldposx);
                sim_effect.Parameters["mousepos_Y"].SetValue(mo_worldposy);
                sim_effect.Parameters["currentlayer"].SetValue(currentlayer);
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.End();
            //for(int i = 0; i < LAYER_NUM + 1; ++i)
            //{
            //    if (lines2draw_count[i] > 0)
            //    {
            //        Game1.graphics.GraphicsDevice.SetRenderTarget(logic_targets[i]);

            //        line_effect.Parameters["WorldViewProjection"].SetValue(basicEffect.World * basicEffect.View * basicEffect.Projection);
            //        line_effect.CurrentTechnique.Passes[0].Apply();
            //        Game1.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, lines2draw[i], 0, lines2draw_count[i] / 2);

            //        Game1.graphics.GraphicsDevice.SetRenderTarget(null);
            //        lines2draw_count[i] = 0;
            //    }
            //}
            for (int i = 0; i < LAYER_NUM + 1; ++i)
            {
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
                    lines2draw_count[i] = 0;
                }
            }


            //sim_effect.Parameters["logictex_L1"].SetValue(logic_targets[0]);
            //sim_effect.Parameters["logictex_L2"].SetValue(logic_targets[1]);
            //sim_effect.Parameters["logictex_L3"].SetValue(logic_targets[2]);
            //sim_effect.Parameters["logictex_L4"].SetValue(logic_targets[3]);
            //sim_effect.Parameters["logictex_L5"].SetValue(logic_targets[4]);
            //sim_effect.Parameters["logictex_L6"].SetValue(logic_targets[5]);
            //sim_effect.Parameters["logictex_L7"].SetValue(logic_targets[6]);
            //sim_effect.Parameters["logictex_LV"].SetValue(logic_targets[7]);
            sim_effect.Parameters["logictex"].SetValue(logic_target);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, sim_effect, Matrix.Identity);
            spritebatch.Draw(main_target, Vector2.Zero, Color.White);
            spritebatch.End();



            spritebatch.Begin();
            spritebatch.DrawString(Game1.basefont, "Layer: " + currentlayer.ToString(), new Vector2(500, 100), Color.Red);
            if(IsInGrid && (IsWire[mo_worldposx, mo_worldposy] & (1 << currentlayer)) > 0)
                spritebatch.DrawString(Game1.basefont, "Network: " + WireIDs[mo_worldposx / 2, mo_worldposy / 2, currentlayer].ToString(), new Vector2(500, 130), Color.Red);

        }
    }
}
