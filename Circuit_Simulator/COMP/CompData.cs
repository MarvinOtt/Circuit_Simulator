﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.COMP
{
    public class CompData
    {
        public List<ComponentPixel>[] data;
        public List<string> parameters;
        public string name;
        public string catagory = "Other";
        public string Code_AfterSim = "", Code_Sim = "";
        public string Code_Sim_FuncName = "", Code_AfterSim_FuncName = "";
        public Rectangle[] bounds;
        public int currentrotation;
        public int pin_num, OverlayStateID, totalstate_length;
        private int _internalstate_length, _overlayseg_length, _valuebox_length;
        public int internalstate_length { get { return _internalstate_length; } set { _internalstate_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public int OverlaySeg_length { get { return _overlayseg_length; } set { _overlayseg_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public int valuebox_length { get { return _valuebox_length; } set { _valuebox_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public bool IsOverlay, IsUpdateAfterSim, ShowOverlay;
        public bool IsClickable;
        public Point overlaysize;
        public int ClickAction_Type;
        public Action<Component> ClickAction;
        public delegate void AfterSimAction_Prototype(int[] internalstates, IntPtr CompInfos, int compindex);
        public AfterSimAction_Prototype AfterSimAction;
        public List<Line>[][] overlaylines;
        public List<VertexPositionLine>[][] overlaylines_vertices;
        public Texture2D overlaytex;
        public FRectangle[] overlay_bounds;
        public Rectangle overlaytex_bounds;
        public CompLibrary library;
        private static Action<Component>[] AllClickActions = new Action<Component>[]
        {
            delegate (Component comp)
            {
                if(Game1.mo_states.IsLeftButtonToggleOn())
                {
                    int segid = Sim_Component.Components_Data[comp.dataID].internalstate_length;
                    comp.internalstates[segid] ^= 1;

                    byte state = (byte)comp.internalstates[segid];

                    if (!Simulator.IsSimulating)
                    {
                        for (int i = 0; i < comp.pinNetworkIDs.Length; ++i)
                        {
                            if(Simulator.networks[comp.pinNetworkIDs[i]] != null)
                                Simulator.networks[comp.pinNetworkIDs[i]].state = state;
                        }
                    }
                    else
                        Sim_INF_DLL.SetIntState(comp.ID, segid);
                }
            },
            delegate (Component comp)
            {
                int segid = Sim_Component.Components_Data[comp.dataID].internalstate_length;
                comp.internalstates[segid] = (Game1.mo_states.New.LeftButton == ButtonState.Pressed) ? 1 : 0;

                byte state = (byte)comp.internalstates[segid];

                if (!Simulator.IsSimulating)
                {
                    for (int i = 0; i < comp.pinNetworkIDs.Length; ++i)
                    {
                        if(Simulator.networks[comp.pinNetworkIDs[i]] != null)
                            Simulator.networks[comp.pinNetworkIDs[i]].state = state;
                    }
                }
                else
                    Sim_INF_DLL.SetIntState(comp.ID, segid);
            }
        };

        public CompData(string name, string catagory, bool IsOverlay, bool IsClickable, bool IsUpdateAfterSim)
        {
            this.name = name;
            this.catagory = catagory;
            this.IsClickable = IsClickable;
            this.IsOverlay = IsOverlay;
            this.IsUpdateAfterSim = IsUpdateAfterSim;
            InitializeLineOverlays(1);
            //for (int i = 0; i < 4; ++i)
            //    overlaylines[i] = new List<Line>();
            //overlaylines_vertices = new List<VertexPositionLine>[4];
            //for (int i = 0; i < 4; ++i)
            //    overlaylines_vertices[i] = new List<VertexPositionLine>();
            bounds = new Rectangle[4];
            data = new List<ComponentPixel>[4];
            for (int i = 0; i < 4; ++i)
                data[i] = new List<ComponentPixel>();
            overlay_bounds = new FRectangle[4];
            overlaysize = Game1.basefont.MeasureString(name).ToPoint();
            parameters = new List<string>();
        }


        public void CalculateBounds(int rotation)
        {
            bounds[rotation].X = data[rotation].Min(x => x.pos.X);
            bounds[rotation].Y = data[rotation].Min(x => x.pos.Y);
            bounds[rotation].Width = data[rotation].Max(x => (x.pos.X - bounds[rotation].X) + 1);
            bounds[rotation].Height = data[rotation].Max(x => (x.pos.Y - bounds[rotation].Y) + 1);
        }

        public void InitializeLineOverlays(int count)
        {
            OverlaySeg_length = count;
            overlaylines = new List<Line>[count][];
            overlaylines_vertices = new List<VertexPositionLine>[count][];
            for(int i = 0; i < count; ++i)
            {
                overlaylines[i] = new List<Line>[4];
                overlaylines_vertices[i] = new List<VertexPositionLine>[4];
                for(int j = 0; j < 4; ++j)
                {
                    overlaylines[i][j] = new List<Line>();
                    overlaylines_vertices[i][j] = new List<VertexPositionLine>();
                }
            }
        }

        public void addData(ComponentPixel dat, int offset = 0)
        {
            if (dat.type > Sim_Component.PINOFFSET && dat.type - Sim_Component.PINOFFSET > pin_num)
                pin_num = dat.type - Sim_Component.PINOFFSET;
            data[(0 + offset) % 4].Add(dat);
            data[(1 + offset) % 4].Add(new ComponentPixel(new Point(-dat.pos.Y, dat.pos.X), dat.type));
            data[(2 + offset) % 4].Add(new ComponentPixel(new Point(-dat.pos.X, -dat.pos.Y), dat.type));
            data[(3 + offset) % 4].Add(new ComponentPixel(new Point(dat.pos.Y, -dat.pos.X), dat.type));
            for (int i = 0; i < 4; ++i)
                CalculateBounds(i);
        }

        public void ClearAllPixel()
        {
            for (int i = 0; i < 4; ++i)
                data[i].Clear();
        }

        public void addOverlayLine(Line line, float layers, int OverlayIndex)
        {
            VertexPositionLine Vline1, Vline2;
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][0].Add(line);
            overlaylines_vertices[OverlayIndex][0].Add(Vline1);
            overlaylines_vertices[OverlayIndex][0].Add(Vline2);
            line = new Line(new Point(-line.start.Y, line.start.X), new Point(-line.end.Y, line.end.X));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][1].Add(line);
            overlaylines_vertices[OverlayIndex][1].Add(Vline1);
            overlaylines_vertices[OverlayIndex][1].Add(Vline2);
            line = new Line(new Point(-line.start.Y, line.start.X), new Point(-line.end.Y, line.end.X));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][2].Add(line);
            overlaylines_vertices[OverlayIndex][2].Add(Vline1);
            overlaylines_vertices[OverlayIndex][2].Add(Vline2);
            line = new Line(new Point(-line.start.Y, line.start.X), new Point(-line.end.Y, line.end.X));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][3].Add(line);
            overlaylines_vertices[OverlayIndex][3].Add(Vline1);
            overlaylines_vertices[OverlayIndex][3].Add(Vline2);
            //overlaylines_vertices[1].Add(new VertexPositionLine(new Point((int)-Math.Floor(line.Position.Y), (int)Math.Floor(line.Position.X)), line.layers));
            //overlaylines_vertices[2].Add(new VertexPositionLine(new Point(-(int)Math.Floor(line.Position.X), (int)-Math.Floor(line.Position.Y)), line.layers));
            //overlaylines_vertices[3].Add(new VertexPositionLine(new Point((int)Math.Floor(line.Position.Y), (int)-Math.Floor(line.Position.X)), line.layers));

        }

        public void Finish()
        {
            for (int i = 0; i < 4; ++i)
            {
                for (int j = 0; j < data[i].Count; ++j)
                {
                    if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X - 1) && x.pos.Y == (data[i][j].pos.Y)))
                        data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 0)));
                    if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X) && x.pos.Y == (data[i][j].pos.Y - 1)))
                        data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 1)));
                    if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X + 1) && x.pos.Y == (data[i][j].pos.Y)))
                        data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 2)));
                    if (data[i].Exists(x => x.pos.X == (data[i][j].pos.X) && x.pos.Y == (data[i][j].pos.Y + 1)))
                        data[i][j] = new ComponentPixel(data[i][j].pos, data[i][j].type, (byte)(data[i][j].IsEdge | (1 << 3)));
                }
            }

            ClickAction = AllClickActions[ClickAction_Type];
        }

        public void Save(FileStream stream)
        {
            byte[] bytearray = name.GetBytes();
            stream.Write(bytearray, 0, bytearray.Length);
            bytearray = catagory.GetBytes();
            stream.Write(bytearray, 0, bytearray.Length);
            stream.Write(BitConverter.GetBytes(IsOverlay), 0, 1);
            stream.Write(BitConverter.GetBytes(IsClickable), 0, 1);
            stream.Write(BitConverter.GetBytes(IsUpdateAfterSim), 0, 1);
            stream.Write(BitConverter.GetBytes(ShowOverlay), 0, 1);
            stream.Write(BitConverter.GetBytes(data[0].Count), 0, 4);
            for(int j = 0; j < data[0].Count; ++j)
            {
                stream.Write(BitConverter.GetBytes(data[0][j].pos.X), 0, 4);
                stream.Write(BitConverter.GetBytes(data[0][j].pos.Y), 0, 4);
                stream.Write(BitConverter.GetBytes(data[0][j].type), 0, 1);
            }
            stream.Write(BitConverter.GetBytes(overlaylines.Length), 0, 4);
            for (int i = 0; i < overlaylines.Length; ++i)
            {
                stream.Write(BitConverter.GetBytes(overlaylines[i][0].Count), 0, 4);
                for (int j = 0; j < overlaylines[i][0].Count; ++j)
                {
                    stream.Write(BitConverter.GetBytes(overlaylines[i][0][j].start.X), 0, 4);
                    stream.Write(BitConverter.GetBytes(overlaylines[i][0][j].start.Y), 0, 4);
                    stream.Write(BitConverter.GetBytes(overlaylines[i][0][j].end.X), 0, 4);
                    stream.Write(BitConverter.GetBytes(overlaylines[i][0][j].end.Y), 0, 4);
                    stream.Write(BitConverter.GetBytes(overlaylines_vertices[i][0][j * 2].layers), 0, 4);
                }
            }
            
            stream.Write(BitConverter.GetBytes(internalstate_length), 0, 4);
            stream.Write(BitConverter.GetBytes(valuebox_length), 0, 4);
            for(int i = 0; i < valuebox_length; ++i)
            {
                bytearray = parameters[i].GetBytes();
                stream.Write(bytearray, 0, bytearray.Length);
            }

            stream.Write(BitConverter.GetBytes(OverlaySeg_length), 0, 4);
            stream.Write(BitConverter.GetBytes(ClickAction_Type), 0, 4);
            if (IsUpdateAfterSim)
            {
                bytearray = Code_AfterSim.GetBytes();
                stream.Write(bytearray, 0, bytearray.Length);
            }
            bytearray = Code_Sim.GetBytes();
            stream.Write(bytearray, 0, bytearray.Length);
            if (IsUpdateAfterSim)
            {
                bytearray = Code_AfterSim_FuncName.GetBytes();
                stream.Write(bytearray, 0, bytearray.Length);
            }
            bytearray = Code_Sim_FuncName.GetBytes();
            stream.Write(bytearray, 0, bytearray.Length);
        }
    }
}
