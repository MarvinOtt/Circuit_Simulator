﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public struct Point16
    {
        short X, Y;
    }
    public struct Line
    {
        byte layers;
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
    }

    public class Simulator
    {
        public const int SIZEX = 16384;
        public const int SIZEY = 16384;
        public const int LAYER_NUM = 7;

        Effect sim_effect;
        RenderTarget2D main_target;
        Texture2D outputtex, logictex;
        Network CalcNetwork;
        public static Network[] networks;
        public static int highestNetworkID = 3;
        public static int[] emptyNetworkID;
        public static int emptyNetworkID_count;
        public static byte[,] IsWire, CellType, CalcGridData, CalcGridStat;
        int[] CalcOccurNetw;
        byte[] revshiftarray;
        int CalcOccurNetw_Pos;
        int[,,] WireIDs;

        Point worldpos;
        int worldzoom = 0;

        int sim_speed = 1;

        #region INPUT

        int currentlayer;
        int mo_worldposx, mo_worldposy;
        bool IsInGrid;

        #endregion


        public Simulator()
        {
            Game1.GraphicsChanged += Window_Graphics_Changed;
            sim_effect = Game1.content.Load<Effect>("sim_effect");
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            outputtex = new Texture2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            logictex = new Texture2D(Game1.graphics.GraphicsDevice, SIZEX, SIZEY, false, SurfaceFormat.Alpha8);
            IsWire = new byte[SIZEX, SIZEY];
            CellType = new byte[SIZEX, SIZEY];
            CalcGridData = new byte[SIZEX, SIZEY];
            CalcGridStat = new byte[SIZEX, SIZEY];
            CalcOccurNetw = new int[10000];
            WireIDs = new int[SIZEX / 2, SIZEY / 2, LAYER_NUM];
            networks = new Network[10000000];
            emptyNetworkID = new int[10000000];

            revshiftarray = new byte[256];
            for (int i = 0; i < 8; ++i)
            {
                for(int j = 0; j < (1 << i); ++j)
                    revshiftarray[(1 << i) + j] = (byte)i;
            } 

            sim_effect.Parameters["Screenwidth"].SetValue(Game1.Screenwidth);
            sim_effect.Parameters["Screenheight"].SetValue(Game1.Screenheight);
            sim_effect.Parameters["worldsizex"].SetValue(SIZEX);
            sim_effect.Parameters["worldsizey"].SetValue(SIZEY);
        }

        public void Window_Graphics_Changed(object sender, EventArgs e)
        {
            main_target?.Dispose();
            main_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);
            outputtex?.Dispose();
            outputtex = new Texture2D(Game1.graphics.GraphicsDevice, Game1.Screenwidth, Game1.Screenheight);

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

        //public void PlaceWireOnCell(int x, int y)
        //{
        //    CellType[x, y] = 1;
        //    IsWire[x, y] |= (byte)(1 << currentlayer);
        //    WireIDs[x / 2, y / 2, currentlayer] = -1;
        //    CalculateNetworkAt(x, y, currentlayer);
        //}

        public void DoFFIfValid(int x, int y, int curval)
        {
            byte nextval = IsWire[x, y];
            if((curval & nextval) > 0 && CalcGridData[x, y] == 0)
                FloodFillCellAndNeighbours(x, y, (nextval == 255) ? 255 : (curval & nextval));
        }
        public void FloodFillCellAndNeighbours(int x, int y, int mask)
        {
            byte curval = (byte)(IsWire[x, y] & mask);
            CalcGridData[x, y] = curval;


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
                    if(!(xx == 0 && yy == 0) && !((xx == dirvec.X && yy == dirvec.Y) || (xx == -dirvec.X && yy == -dirvec.Y)))
                    {
                        if (CalcGridStat[x + xx, y + yy] != 2 && CalcGridData[x + xx, y + yy] != 0)
                            CalculateLine(x + xx, y + yy);
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
                dirvec = new Point(0, -1);
            if (dir == 2)
                dirvec = new Point(1, -1);
            if (dir == 3)
                dirvec = new Point(1, 0);
            if (dir == 4)
                dirvec = new Point(1, 1);

            // Marching every cell for this line
            int i, j;
            for(i = 0; ;++i)
            {
                int xx = x + dirvec.X * i;
                int yy = y + dirvec.Y * i;
                if (CalcGridStat[xx, yy] != 2 && CalcGridData[xx, yy] == linelayers)
                {
                    CalcGridStat[xx, yy] = 2;
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
                if (CalcGridStat[xx, yy] != 2 && CalcGridData[xx, yy] == linelayers)
                {
                    CalcGridStat[xx, yy] = 2;
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
            //CalcOccurNetw[0] = WireIDs[x / 2, y / 2, revshiftarray[layermask]];

            // Flood Fill CalcGrid
            FloodFillCellAndNeighbours(x, y, layermask);

            // Delete previous networks
            for(int i = 0; i < CalcOccurNetw_Pos; ++i)
            {
                networks[CalcOccurNetw[i]] = null;
                emptyNetworkID[emptyNetworkID_count++] = CalcOccurNetw[i];
            }

            // Main Line Algorithm
            int netID = CalculateNewNetwork(x, y, layermask);

            // Clear Calc Grid
            networks[netID].ClearCalcGrid();



            //if((IsWire[x, y] & (1 << layer)) != 0)
            //{
            //    if((IsWire[x - 1, y] & (1 << layer)) != 0)
            //}
        }

        public void Update()
        {
            screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out mo_worldposx, out mo_worldposy);
            IsInGrid = mo_worldposx >= 0 && mo_worldposy >= 0 && mo_worldposx < SIZEX && mo_worldposy < SIZEY;

            #region INPUT
            if (Game1.kb_states.New.IsKeyDown(Keys.W))
                worldpos.Y += 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.S))
                worldpos.Y -= 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.A))
                worldpos.X += 10;
            if (Game1.kb_states.New.IsKeyDown(Keys.D))
                worldpos.X -= 10;

            if (Game1.mo_states.New.ScrollWheelValue != Game1.mo_states.Old.ScrollWheelValue)
            {
                if (Game1.mo_states.New.ScrollWheelValue < Game1.mo_states.Old.ScrollWheelValue && worldzoom > -8) // Zooming Out
                {
                    worldzoom -= 1;
                    Point diff = Game1.mo_states.New.Position - worldpos;
                    worldpos += new Point(diff.X / 2, diff.Y / 2);
                }
                else if(Game1.mo_states.New.ScrollWheelValue > Game1.mo_states.Old.ScrollWheelValue && worldzoom < 8) // Zooming In
                {
                    worldzoom += 1;
                    Point diff = Game1.mo_states.New.Position - worldpos;
                    worldpos -= diff;
                }
            }
            #endregion

            // Placing Wires
            if(IsInGrid && Game1.mo_states.IsLeftButtonToggleOn())
            {
                IsWire[mo_worldposx, mo_worldposy] = (1 << 4);
                CalculateNetworkAt(mo_worldposx, mo_worldposy, (byte)(1 << 4));
            }

            sim_effect.Parameters["zoom"].SetValue((float)Math.Pow(2, worldzoom));
            sim_effect.Parameters["coos"].SetValue(worldpos.ToVector2());
            sim_effect.Parameters["mousepos_X"].SetValue(mo_worldposx);
            sim_effect.Parameters["mousepos_Y"].SetValue(mo_worldposy);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.End();

            sim_effect.Parameters["logictex"].SetValue(logictex);
            spritebatch.Begin(SpriteSortMode.Deferred, null, null, null, null, sim_effect, Matrix.Identity);
            spritebatch.Draw(outputtex, Vector2.Zero, Color.White);
            spritebatch.End();



            spritebatch.Begin();
            if(IsInGrid && IsWire[mo_worldposx, mo_worldposy] != 0)
                spritebatch.DrawString(Game1.basefont, "Network: " + WireIDs[mo_worldposx / 2, mo_worldposy / 2, 4].ToString(), new Vector2(500, 100), Color.Red);

        }
    }
}
