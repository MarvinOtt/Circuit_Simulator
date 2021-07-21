using Microsoft.Xna.Framework;
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
        public static byte[] rottable_ROT = { 1, 2, 3, 0, 5, 6, 7, 4 };
        public static byte[] rottable_FLIPX = { 6, 5, 4, 7, 2, 1, 0, 3};
        public static byte[] rottable_FLIPY = { 4, 7, 6, 5, 0, 3, 2, 1};
        public static SpriteFont overlayfont = App.content.Load<SpriteFont>("UI\\overlayfont");
        public List<ComponentPixel>[] data;
        public List<string> parameters;
        public string name;
        public string OverlayText;
        public float[] OverlayTextSize;
        public Vector2[] OverlayTextPos;
        public string catagory = "Other";
        public string  Code_Sim = "";
        public string Code_Sim_FuncName = "";
        public Rectangle[] bounds;
        public byte currentrotation;
        public int pin_num, totalstate_length;
        private int _internalstate_length, _overlayseg_length, _valuebox_length;
        public int internalstate_length { get { return _internalstate_length; } set { _internalstate_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public int OverlaySeg_length { get { return _overlayseg_length; } set { _overlayseg_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public int valuebox_length { get { return _valuebox_length; } set { _valuebox_length = value; totalstate_length = _internalstate_length + _overlayseg_length + _valuebox_length; } }
        public bool IsOverlay;
        public bool IsClickable;
        public int ClickAction_Type;
        public Action<Component> ClickAction;
        //public delegate void AfterSimAction_Prototype(int[] internalstates, IntPtr CompInfos, int compindex);
        //public AfterSimAction_Prototype AfterSimAction;
        public List<Line>[][] overlaylines;
        public List<VertexPositionLine>[][] overlaylines_vertices;
        private static Action<Component>[] AllClickActions = new Action<Component>[]
        {
            delegate (Component comp)
            {
                if(App.mo_states.IsLeftButtonToggleOn())
                {
                    int segid = Sim_Component.Components_Data[comp.dataID].internalstate_length;
                    comp.totalstates[segid] ^= 1;

                    byte state = (byte)comp.totalstates[segid];

                    if (Simulator.cursimframe == 0)
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
                comp.totalstates[segid] = (App.mo_states.New.LeftButton == ButtonState.Pressed) ? 1 : 0;

                byte state = (byte)comp.totalstates[segid];

                if (Simulator.cursimframe == 0)
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

        public CompData(string name, string catagory, bool IsOverlay, bool IsClickable)
        {
            this.name = name;
            this.catagory = catagory;
            this.IsClickable = IsClickable;
            this.IsOverlay = IsOverlay;
            InitializeLineOverlays(1);
            //for (int i = 0; i < 4; ++i)
            //    overlaylines[i] = new List<Line>();
            //overlaylines_vertices = new List<VertexPositionLine>[4];
            //for (int i = 0; i < 4; ++i)
            //    overlaylines_vertices[i] = new List<VertexPositionLine>();
            OverlayText = "";
            bounds = new Rectangle[8];
            OverlayTextSize = new float[8];
            OverlayTextPos = new Vector2[8];
            data = new List<ComponentPixel>[8];
            for (int i = 0; i < 8; ++i)
                data[i] = new List<ComponentPixel>();
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
            if (count > 0)
            {
                overlaylines = new List<Line>[count][];
                overlaylines_vertices = new List<VertexPositionLine>[count][];
                for (int i = 0; i < count; ++i)
                {
                    overlaylines[i] = new List<Line>[8];
                    overlaylines_vertices[i] = new List<VertexPositionLine>[8];
                    for (int j = 0; j < 8; ++j)
                    {
                        overlaylines[i][j] = new List<Line>();
                        overlaylines_vertices[i][j] = new List<VertexPositionLine>();
                    }
                }
            }
        }

        public void addData(ComponentPixel dat)
        {
            if (dat.type > Sim_Component.PINOFFSET && dat.type - Sim_Component.PINOFFSET > pin_num)
                pin_num = dat.type - Sim_Component.PINOFFSET;
            data[(0) % 8].Add(dat);
            data[(1) % 8].Add(new ComponentPixel(new Point(-dat.pos.Y, dat.pos.X), dat.type));
            data[(2) % 8].Add(new ComponentPixel(new Point(-dat.pos.X, -dat.pos.Y), dat.type));
            data[(3) % 8].Add(new ComponentPixel(new Point(dat.pos.Y, -dat.pos.X), dat.type));
            data[(4) % 8].Add(new ComponentPixel(new Point(dat.pos.X, -dat.pos.Y), dat.type));
            data[(5) % 8].Add(new ComponentPixel(new Point(dat.pos.Y, dat.pos.X), dat.type));
            data[(6) % 8].Add(new ComponentPixel(new Point(-dat.pos.X, dat.pos.Y), dat.type));
            data[(7) % 8].Add(new ComponentPixel(new Point(-dat.pos.Y, -dat.pos.X), dat.type));
            for (int i = 0; i < 8; ++i)
                CalculateBounds(i);
        }

        public void ClearAllPixel()
        {
            for (int i = 0; i < 8; ++i)
                data[i].Clear();
        }

        public void addOverlayLine(Line line, float layers, int OverlayIndex)
        {
            VertexPositionLine Vline1, Vline2;
            Line line1 = new Line(new Point(line.start.X, line.start.Y), new Point(line.end.X, line.end.Y));
            line1.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][0].Add(line1);
            overlaylines_vertices[OverlayIndex][0].Add(Vline1);
            overlaylines_vertices[OverlayIndex][0].Add(Vline2);
            Line line2 = new Line(new Point(-line.start.Y, line.start.X), new Point(-line.end.Y, line.end.X));
            line2.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][1].Add(line2);
            overlaylines_vertices[OverlayIndex][1].Add(Vline1);
            overlaylines_vertices[OverlayIndex][1].Add(Vline2);
            Line line3 = new Line(new Point(-line2.start.Y, line2.start.X), new Point(-line2.end.Y, line2.end.X));
            line3.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][2].Add(line3);
            overlaylines_vertices[OverlayIndex][2].Add(Vline1);
            overlaylines_vertices[OverlayIndex][2].Add(Vline2);
            Line line4 = new Line(new Point(-line3.start.Y, line3.start.X), new Point(-line3.end.Y, line3.end.X));
            line4.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][3].Add(line4);
            overlaylines_vertices[OverlayIndex][3].Add(Vline1);
            overlaylines_vertices[OverlayIndex][3].Add(Vline2);


            line = new Line(new Point(line1.start.X, -line1.start.Y), new Point(line1.end.X, -line1.end.Y));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][4].Add(line);
            overlaylines_vertices[OverlayIndex][4].Add(Vline1);
            overlaylines_vertices[OverlayIndex][4].Add(Vline2);
            line = new Line(new Point(-line2.start.X, line2.start.Y), new Point(-line2.end.X, line2.end.Y));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][5].Add(line);
            overlaylines_vertices[OverlayIndex][5].Add(Vline1);
            overlaylines_vertices[OverlayIndex][5].Add(Vline2);
            line = new Line(new Point(line3.start.X, -line3.start.Y), new Point(line3.end.X, -line3.end.Y));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][6].Add(line);
            overlaylines_vertices[OverlayIndex][6].Add(Vline1);
            overlaylines_vertices[OverlayIndex][6].Add(Vline2);
            line = new Line(new Point(-line4.start.X, line4.start.Y), new Point(-line4.end.X, line4.end.Y));
            line.Convert2LineVertices(layers, out Vline1, out Vline2);
            overlaylines[OverlayIndex][7].Add(line);
            overlaylines_vertices[OverlayIndex][7].Add(Vline1);
            overlaylines_vertices[OverlayIndex][7].Add(Vline2);
            //overlaylines_vertices[1].Add(new VertexPositionLine(new Point((int)-Math.Floor(line.Position.Y), (int)Math.Floor(line.Position.X)), line.layers));
            //overlaylines_vertices[2].Add(new VertexPositionLine(new Point(-(int)Math.Floor(line.Position.X), (int)-Math.Floor(line.Position.Y)), line.layers));
            //overlaylines_vertices[3].Add(new VertexPositionLine(new Point((int)Math.Floor(line.Position.Y), (int)-Math.Floor(line.Position.X)), line.layers));

        }

        public void Finish()
        {
            for (int i = 0; i < 8; ++i)
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
            byte[] bytearray = name.GetBytesFromString();
            stream.Write(bytearray, 0, bytearray.Length);
            bytearray = catagory.GetBytesFromString();
            stream.Write(bytearray, 0, bytearray.Length);
            stream.Write(BitConverter.GetBytes(IsOverlay), 0, 1);
            stream.Write(BitConverter.GetBytes(IsClickable), 0, 1);
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

            bytearray = OverlayText.GetBytesFromString();
            stream.Write(bytearray, 0, bytearray.Length);
            for(int i = 0; i < 8; ++i)
            {
                stream.Write(BitConverter.GetBytes(OverlayTextPos[i].X), 0, 4);
                stream.Write(BitConverter.GetBytes(OverlayTextPos[i].Y), 0, 4);
                stream.Write(BitConverter.GetBytes(OverlayTextSize[i]), 0, 4);
            }

            stream.Write(BitConverter.GetBytes(internalstate_length), 0, 4);
            stream.Write(BitConverter.GetBytes(valuebox_length), 0, 4);
            for(int i = 0; i < valuebox_length; ++i)
            {
                bytearray = parameters[i].GetBytesFromString();
                stream.Write(bytearray, 0, bytearray.Length);
            }

            stream.Write(BitConverter.GetBytes(OverlaySeg_length), 0, 4);
            stream.Write(BitConverter.GetBytes(ClickAction_Type), 0, 4);
            bytearray = Code_Sim.GetBytesFromString();
            stream.Write(bytearray, 0, bytearray.Length);
            bytearray = Code_Sim_FuncName.GetBytesFromString();
            stream.Write(bytearray, 0, bytearray.Length);
        }
    }
}
