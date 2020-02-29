using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public class Network
    {
        /// <remarks>sdfsd</remarks>
        public static List<Network> CheckForDrawing = new List<Network>();
        public List<Line_Netw> lines;
        public int ID;
        public byte state;
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
            foreach (int net in nets)
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
                            for (int k = 0; k < 7; ++k)
                            {
                                if (((linelayers >> k) & 1) > 0)
                                    Simulator.WireIDs[curpos.X / 2, curpos.Y / 2, k] = 0;
                            }
                            if (linelayers >= 128)
                                Simulator.WireIDPs[curpos.X, curpos.Y] = 0;


                            curpos += curnet.lines[i].dir;
                        }

                    }
                    Simulator.emptyNetworkIDs[Simulator.emptyNetworkIDs_count++] = net;
                }
                Simulator.networks[net] = null;
            }
        }

        public void ClearCalcGrid()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                for (int l = 0; l < lines[i].length; ++l)
                {
                    int x = lines[i].start.X + lines[i].dir.X * l;
                    int y = lines[i].start.Y + lines[i].dir.Y * l;
                    Simulator.CalcGridData[x, y] = Simulator.CalcGridStat[x, y] = 0;

                }
            }
        }

        public void PlaceNetwork()
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                Point curpos = lines[i].start;
                for (int j = 0; j < lines[i].length; ++j)
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
                            if (Simulator.lines2draw_count[j] >= 500000)
                                Simulator.DrawLines();
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

                        if (Simulator.lines2draw_count[7] >= 500000)
                            Simulator.DrawLines();
                    }
                }
                NeedsDrawing = false;
            }
        }
    }
}
