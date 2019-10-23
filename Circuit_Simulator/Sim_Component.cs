using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
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
            if (Sim_Component.Components_Data[dataID].IsOverlay)
                Sim_Component.CompMayneedoverlay.Add(ID);
            if(Sim_Component.Components_Data[dataID].internalstate_length > 0)
                internalstates = new int[Sim_Component.Components_Data[dataID].internalstate_length];
            pinNetworkIDs = new int[Sim_Component.Components_Data[dataID].pin_num];
        }

        public void Clicked()
        {
            if(Sim_Component.Components_Data[dataID].CanBeClicked)
            {
                Sim_Component.Components_Data[dataID].ClickAction(this);
            }
        }

        public static bool IsValidPlacement(int dataID, Point pos)
        {
            //Check if component can be placed
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[Sim_Component.Components_Data[dataID].currentrotation];
            bool IsPlacementValid = true;
            if (Simulator.IsSimulating)
                IsPlacementValid = false;
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                if (currentcoo.X >= Simulator.MINCOO && currentcoo.Y >= Simulator.MINCOO && currentcoo.X < Simulator.MAXCOO && currentcoo.Y < Simulator.MAXCOO)
                {
                    if (Simulator.IsWire[currentcoo.X, currentcoo.Y] != 0 || Sim_Component.CompType[currentcoo.X, currentcoo.Y] != 0)
                        IsPlacementValid = false;
                }
                else
                    IsPlacementValid = false;
            }
            return IsPlacementValid;
        }

        public void Place(Point pos, int newrotation)
        {
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
                if (datapixel[i].type > Sim_Component.PINOFFSET)
                {
                    Point datapos = datapixel[i].pos - Sim_Component.Components_Data[dataID].bounds[newrotation].Location;
                    data2place[datapos.X, datapos.Y] = 255;

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
            Game1.simulator.PlaceArea(area, data2place);
        }

        public void Delete()
        {
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[rotation];
            Rectangle area = Sim_Component.Components_Data[dataID].bounds[rotation];
            area.Location += pos;
            byte[,] data2place = new byte[area.Size.X, area.Size.Y];
            Simulator.IsWire.GetArea(data2place, area);

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
                    data2place[datapos.X, datapos.Y] = 0;
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

        public ComponentPixel(Point pos, byte type)
        {
            this.pos = pos;
            this.type = type;
        }
    }
    public class ComponentData
    {
        public List<ComponentPixel>[] data;
        string name;
        string catagory;
        public Rectangle[] bounds;
        public int currentrotation;
        public int pin_num, OverlayStateID, internalstate_length;
        public bool IsOverlay;
        public bool CanBeClicked;
        public Action<Component> ClickAction;
        public List<VertexPositionLine> overlaylines;

        public ComponentData(string name, string catagory, bool IsOverlay, bool IsClickable)
        {
            this.name = name;
            this.CanBeClicked = IsClickable;
            this.IsOverlay = IsOverlay;
            if (IsOverlay)
                overlaylines = new List<VertexPositionLine>();
            data = new List<ComponentPixel>[4];
            bounds = new Rectangle[4];
            for (int i = 0; i < 4; ++i)
                data[i] = new List<ComponentPixel>();
        }

        public void CalculateBounds(int rotation)
        {
            bounds[rotation].X = data[rotation].Min(x => x.pos.X);
            bounds[rotation].Y = data[rotation].Min(x => x.pos.Y);
            bounds[rotation].Width = data[rotation].Max(x => (x.pos.X - bounds[rotation].X) + 1);
            bounds[rotation].Height = data[rotation].Max(x => (x.pos.Y - bounds[rotation].Y) + 1);
        }

        public void addData(ComponentPixel dat)
        {
            if (dat.type > Sim_Component.PINOFFSET)
                pin_num++;
            data[0].Add(dat);
            data[1].Add(new ComponentPixel(new Point(-dat.pos.Y, dat.pos.X), dat.type));
            data[2].Add(new ComponentPixel(new Point(-dat.pos.X, -dat.pos.Y), dat.type));
            data[3].Add(new ComponentPixel(new Point(dat.pos.Y, -dat.pos.X), dat.type));
            for (int i = 0; i < 4; ++i)
                CalculateBounds(i);
        }

        public void addOverlayLine(VertexPositionLine line)
        {
            overlaylines.Add(line);
        }
    }

    public class Sim_Component
    {
        public static int PINOFFSET = 3;

        Simulator sim;
        Effect sim_effect;
        Texture2D placementtex;
        public static RenderTarget2D CompTex;
        public bool IsCompDrag;

        public static List<ComponentData> Components_Data;
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
            placementtex = new Texture2D(Game1.graphics.GraphicsDevice, 41, 41, false, SurfaceFormat.Alpha8);
            CompType = new byte[Simulator.SIZEX, Simulator.SIZEY];
            CompTex = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            CompGrid = new int[Simulator.SIZEX / 32, Simulator.SIZEY / 32][];
            CompNetwork = new byte[Simulator.SIZEX, Simulator.SIZEY];
            components = new Component[1000000];
            pins2check = new Point[40000];
            overlaylines = new VertexPositionLine[1000000];
            emptyComponentID = new int[1000000];
            CompMayneedoverlay = new List<int>();

            // Basic Components Data
            Components_Data = new List<ComponentData>();
            Components_Data.Add(new ComponentData("AND", "Gates", false, false));
            Components_Data[0].addData(new ComponentPixel(new Point(0, -1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(0, 0), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(0, 1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, -1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, 0), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(1, 1), 1));
            Components_Data[0].addData(new ComponentPixel(new Point(-1, -1), 4));
            Components_Data[0].addData(new ComponentPixel(new Point(2, 0), 6));
            Components_Data[0].addData(new ComponentPixel(new Point(-1, 1), 5));

            Components_Data.Add(new ComponentData("Button", "Input", true, true));
            Components_Data[1].addData(new ComponentPixel(new Point(0, -1), 2));
            Components_Data[1].addData(new ComponentPixel(new Point(0, 0), 2));
            Components_Data[1].addData(new ComponentPixel(new Point(0, 1), 2));
            Components_Data[1].addData(new ComponentPixel(new Point(-1, 0), 2));
            Components_Data[1].addData(new ComponentPixel(new Point(1, 0), 2));
            Components_Data[1].addData(new ComponentPixel(new Point(-1, -1), 4));
            Components_Data[1].addData(new ComponentPixel(new Point(1, -1), 5));
            Components_Data[1].addData(new ComponentPixel(new Point(1, 1), 6));
            Components_Data[1].addData(new ComponentPixel(new Point(-1, 1), 7));
            Components_Data[1].addOverlayLine(new VertexPositionLine(new Point(-1, 0), 200));
            Components_Data[1].addOverlayLine(new VertexPositionLine(new Point(2, 0), 200));
            Components_Data[1].addOverlayLine(new VertexPositionLine(new Point(0, -1), 200));
            Components_Data[1].addOverlayLine(new VertexPositionLine(new Point(0, 2), 200));
            Components_Data[1].internalstate_length = 1;
            Components_Data[1].ClickAction += delegate (Component comp)
            {
                comp.internalstates[0] ^= 1;
                byte state = (byte)comp.internalstates[0];
                for (int i = 0; i < comp.pinNetworkIDs.Length; ++i)
                {
                    Simulator.networks[comp.pinNetworkIDs[i]].state = state;
                    Sim_INF_DLL.SetState(comp.pinNetworkIDs[i], state);
                }
            };
            Components_Data[1].OverlayStateID = 0;
        }

        public void InizializeComponentDrag(int ID)
        {
            if (true)//UI_Handler.UI_Active_State != UI_Handler.UI_Active_Main)
            {
                UI_Handler.UI_IsWindowHide = true;
                ((UI_TexButton)UI_Handler.QuickHotbar.ui_elements[6]).IsActivated = false;
                UI_Handler.wire_ddbl.GetsUpdated = UI_Handler.wire_ddbl.GetsDrawn = false;
                //UI_Handler.wire_ddbl.GetsUpdated = UI_Handler.wire_ddbl.GetsDrawn = false;
                Game1.simulator.ChangeToolmode(Simulator.TOOL_COMPONENT);
                IsCompDrag = true;
                byte[] data = new byte[41 * 41];
                List<ComponentPixel> datapixel = Components_Data[ID].data[Components_Data[ID].currentrotation];
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    data[(datapixel[i].pos.Y + 20) * 41 + (datapixel[i].pos.X + 20)] = datapixel[i].type;
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
            Game1.simulator.screen2worldcoo_int(Game1.mo_states.New.Position.ToVector2(), out pos.X, out pos.Y);
            ComponentDropAtPos(dataID, pos);
        }
        public void ComponentDropAtPos(int dataID, Point pos)
        {
            if (UI_Handler.UI_Active_State == UI_Handler.UI_Active_CompDrag)
            {
                if(Component.IsValidPlacement(dataID, pos))
                {
                    Component newcomp;// = new Component(ID, nextComponentID);
                    if(emptyComponentID_count > 0)
                        newcomp = new Component(dataID, emptyComponentID[--emptyComponentID_count]);
                    else
                        newcomp = new Component(dataID, nextComponentID++);
                    components[newcomp.ID] = newcomp;
                    newcomp.Place(pos, Components_Data[dataID].currentrotation);

                }
            }
        }

        public void DeactivateDrop()
        {
            UI_Handler.UI_IsWindowHide = false;
            IsCompDrag = false;
            sim_effect.Parameters["currenttype"].SetValue(0);
            Game1.simulator.ChangeToolmode(Simulator.oldtoolmode);
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

            if (pins2check_length > 0)
            {
                for (int i = 0; i < pins2check_length; ++i)
                {
                    Point pos = pins2check[i];
                    Component cur_comp = components[CompGrid[pos.X / 32, pos.Y / 32][CompNetwork[pos.X, pos.Y]]];
                    int wireID = Simulator.WireIDs[pos.X / 2, pos.Y / 2, 0];
                    cur_comp.pinNetworkIDs[CompType[pos.X, pos.Y] - (PINOFFSET + 1)] = wireID;
                }

                pins2check_length = 0;
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            sim_effect.Parameters["comptex"].SetValue(CompTex);
        }

        public void DrawOverlays(SpriteBatch spritebatch)
        {
            int count = 0;
            for(int i = 0; i < CompMayneedoverlay.Count; ++i)
            {
                int compID = CompMayneedoverlay[i];
                int ovstateID = Components_Data[components[compID].dataID].OverlayStateID;
                int ovstate = components[compID].internalstates[ovstateID];
                List<VertexPositionLine> CompOverlaylines = Components_Data[components[compID].dataID].overlaylines;
                for (int j = 0; j < CompOverlaylines.Count; ++j)
                {
                    overlaylines[count] = CompOverlaylines[j];
                    overlaylines[count].layers = 2 + ovstate;
                    overlaylines[count].Position += new Vector3(components[compID].pos.X, components[compID].pos.Y, 0);
                    count++;
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


        }
    }
}
