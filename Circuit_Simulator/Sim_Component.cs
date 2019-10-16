using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public class Component
    {
        int ID;
        public int dataID;
        Point pos;
        public int[] pinNetworkIDs;

        public Component(Point pos, int dataID, int ID)
        {
            this.pos = pos;
            this.dataID = dataID;
            this.ID = ID;
            pinNetworkIDs = new int[Sim_Component.Components_Data[dataID].pin_num];
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
        public Rectangle bounds;
        public int pin_num;

        public ComponentData(string name, string catagory)
        {
            this.name = name;
            data = new List<ComponentPixel>();
        }

        public void addData(ComponentPixel dat)
        {
            if (dat.type > 1)
                pin_num++;
            data.Add(dat);
            bounds.X = data.Min(x => x.pos.X);
            bounds.Y = data.Min(x => x.pos.X);
            bounds.Width = data.Max(x => (x.pos.X - bounds.X) + 1);
            bounds.Height = data.Max(x => (x.pos.Y - bounds.Y) + 1);
        }
    }

    public class Sim_Component
    {
        Simulator sim;
        Effect sim_effect;
        Texture2D placementtex;
        Texture2D CompTex;
        public bool IsCompDrag;

        public static List<ComponentData> Components_Data;
        public static Component[] components;
        public static int nextComponentID = 1;
        public static byte[,] CompType;
        public static int[,][] CompGrid;
        public static byte[,] CompNetwork;
        public static Point[] pins2check;
        public static int pins2check_length;

        public Sim_Component(Simulator sim, Effect sim_effect)
        {
            this.sim = sim;
            this.sim_effect = sim_effect;
            placementtex = new Texture2D(Game1.graphics.GraphicsDevice, 41, 41, false, SurfaceFormat.Alpha8);
            CompType = new byte[Simulator.SIZEX, Simulator.SIZEY];
            CompTex = new Texture2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8);
            CompGrid = new int[Simulator.SIZEX / 32, Simulator.SIZEY / 32][];
            CompNetwork = new byte[Simulator.SIZEX, Simulator.SIZEY];
            components = new Component[100000];
            pins2check = new Point[100000];

            // Basic Components Data
            Components_Data = new List<ComponentData>();
            Components_Data.Add(new ComponentData("AND", "Gates"));
            Components_Data[0].addData(new ComponentPixel(new Point(0, -1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(0, 0), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(0, 1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, -1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, 0), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, 1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(-1, -1), 2));
            Components_Data[0].addData(new ComponentPixel(new Point(2, 0), 4));
            Components_Data[0].addData(new ComponentPixel(new Point(-1, 1), 3));
        }

        public void InizializeComponentDrag(int ID)
        {
            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                byte[] data = new byte[41 * 41];
                List<ComponentPixel> datapixel = Components_Data[ID].data;
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 20) * 41 + (datapixel[i].pos.X + 20)] = datapixel[i].type;
                }

                placementtex.SetData(data);
                sim_effect.Parameters["currenttype"].SetValue(1);
                sim_effect.Parameters["placementtex"].SetValue(placementtex);
            }
        }

        public void ComponentDrop(int ID)
        {
            Point pos = Point.Zero;
            Game1.simulator.screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out pos.X, out pos.Y);
            ComponentDropAtPos(ID, pos);
        }
        public void ComponentDropAtPos(int ID, Point pos)
        {
            if (UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                //Check if component can be placed
                List<ComponentPixel> datapixel = Components_Data[ID].data;
                bool IsPlacementValid = true;
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    Point currentcoo = pos + datapixel[i].pos;
                    if (currentcoo.X >= Simulator.MINCOO && currentcoo.Y >= Simulator.MINCOO && currentcoo.X < Simulator.MAXCOO && currentcoo.Y < Simulator.MAXCOO)
                    {
                        if (Simulator.IsWire[currentcoo.X, currentcoo.Y] != 0 || CompType[currentcoo.X, currentcoo.Y] != 0)
                            IsPlacementValid = false;
                    }
                    else
                        IsPlacementValid = false;
                }
                if(IsPlacementValid)
                {
                    // Generate new Component
                    components[nextComponentID++] = new Component(pos, ID, nextComponentID - 1);

                    Rectangle area = Components_Data[ID].bounds;
                    area.Location += pos;
                    byte[,] data2place = new byte[area.Size.X, area.Size.Y];
                    Simulator.IsWire.GetArea(data2place, area);
                    for (int i = 0; i < datapixel.Count; ++i)
                    {
                        Point currentcoo = pos + datapixel[i].pos;
                        CompType[currentcoo.X, currentcoo.Y] = datapixel[i].type;
                        CompTex.SetPixel(datapixel[i].type, currentcoo);
                        if(datapixel[i].type > 1)
                        {
                            Point datapos = datapixel[i].pos - Components_Data[ID].bounds.Location;
                            data2place[datapos.X, datapos.Y] = 255;
                            Point gridpos = new Point(currentcoo.X / 32, currentcoo.Y / 32);
                            if (CompGrid[gridpos.X, gridpos.Y] == null)
                            {
                                CompGrid[gridpos.X, gridpos.Y] = new int[200];
                                CompGrid[gridpos.X, gridpos.Y][0] = nextComponentID - 1;
                                CompNetwork[currentcoo.X, currentcoo.Y] = 0;
                            }
                            else
                            {
                                int[] curNetworks = CompGrid[gridpos.X, gridpos.Y];
                                int Index = Array.IndexOf(curNetworks, nextComponentID - 1);
                                if (Index < 0)
                                {
                                    int j = 0;
                                    for (; ; ++j)
                                    {
                                        if (curNetworks[j] == 0)
                                        {
                                            curNetworks[j] = nextComponentID - 1;
                                            break;
                                        }
                                    }
                                    CompNetwork[currentcoo.X, currentcoo.Y] = (byte)j;
                                }
                                else
                                    CompNetwork[currentcoo.X, currentcoo.Y] = (byte)Index;
                            }
                        }
                    }
                    Game1.simulator.PlaceArea(area, data2place);
                }
            }
            sim_effect.Parameters["currenttype"].SetValue(0);
        }

        public int GetComponentID(Point pos)
        {
            Point gridpos = new Point(pos.X / 32, pos.Y / 32);
            int gridid = CompNetwork[pos.X, pos.Y];
            int[] arr = CompGrid[gridpos.X, gridpos.Y];
            if (arr != null)
                return arr[gridid];
            else
                return -1;
        }

        public void Update()
        {
            if(pins2check_length > 0)
            {
                for (int i = 0; i < pins2check_length; ++i)
                {
                    Point pos = pins2check[i];
                    Component cur_comp = components[CompGrid[pos.X / 32, pos.Y / 32][CompNetwork[pos.X, pos.Y]]];
                    int wireID = Simulator.WireIDs[pos.X / 2, pos.Y / 2, 0];
                    cur_comp.pinNetworkIDs[CompType[pos.X, pos.Y] - 2] = wireID;
                }

                pins2check_length = 0;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            sim_effect.Parameters["comptex"].SetValue(CompTex);
        }
    }
}
