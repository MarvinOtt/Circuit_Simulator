using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public byte rotation;
        public int[] pinNetworkIDs;
        public int[] totalstates;

        public Component(int dataID, int ID)
        {
            this.dataID = dataID;
            this.ID = ID;
            CompData compdata = Sim_Component.Components_Data[dataID];
            if (compdata.IsOverlay)
                Sim_Component.CompMayneedoverlay.Add(ID);
            if (compdata.totalstate_length > 0)
                totalstates = new int[compdata.totalstate_length];
            pinNetworkIDs = new int[compdata.pin_num];
        }

        public void Clicked()
        {
            if (Sim_Component.Components_Data[dataID].IsClickable)
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

            }
        }

        public static bool IsValidPlacement(int dataID, Point pos, int rotation)
        {
            //Check if component can be placed
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[rotation];
            bool IsPlacementValid = true;
            if (Simulator.cursimframe > 0)
			{
				IsPlacementValid = false;
				UI_Handler.notificationHandler.AddNotification("Cant place components when simulation is not reseted.");
			}
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                if (currentcoo.X >= Simulator.MINCOO && currentcoo.Y >= Simulator.MINCOO && currentcoo.X < Simulator.MAXCOO && currentcoo.Y < Simulator.MAXCOO)
                {
                    if (Sim_Component.CompType[currentcoo.X, currentcoo.Y] != 0)
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

        public void Place(Point pos, byte newrotation, bool SkipNetworkRouting = false)
        {
            FileHandler.IsUpToDate = false;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            this.pos = pos;
            this.rotation = newrotation;
            List<ComponentPixel> datapixel = Sim_Component.Components_Data[dataID].data[newrotation];
            Rectangle area = Sim_Component.Components_Data[dataID].bounds[newrotation];
            area.Location += pos;
            byte[,] data2place = null;
            if (!SkipNetworkRouting)
            {
                data2place = new byte[area.Size.X, area.Size.Y];
                Simulator.IsWire.GetArea(data2place, area);
            }
			if (Sim_Component.Components_Data[dataID].OverlayText.Length > 0)
			{
				Point comppos_grid = new Point(pos.X / 32, pos.Y / 32);
				if (Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y] == null)
					Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y] = new List<int>();
				Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y].Add(ID);

			}
			for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                Sim_Component.CompType[currentcoo.X, currentcoo.Y] = datapixel[i].type;
                Sim_Component.Comp_target.SetPixel(datapixel[i].type, currentcoo);
                Sim_Component.IsEdge_target.SetPixel(datapixel[i].IsEdge, currentcoo);
                if (datapixel[i].type > Sim_Component.PINOFFSET && !SkipNetworkRouting)
                {
                    Point datapos = currentcoo - area.Location;
                    data2place[datapos.X, datapos.Y] |= 128;

                }
                Point gridpos = new Point(currentcoo.X / 32, currentcoo.Y / 32);
                if (Sim_Component.CompGrid[gridpos.X, gridpos.Y] == null)
                {
                    Sim_Component.CompGrid[gridpos.X, gridpos.Y] = new int[256];
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
            if (!SkipNetworkRouting)
            {
                App.simulator.PlaceArea(area, data2place, SkipNetworkRouting);
            }
            else
            {
                for (int i = 0; i < datapixel.Count; ++i)
                {
                    if (datapixel[i].type > Sim_Component.PINOFFSET)
                        Sim_Component.pins2check[Sim_Component.pins2check_length++] = new Point(datapixel[i].pos.X + pos.X, datapixel[i].pos.Y + pos.Y);
                }
            }
            watch.Stop();
            double milis = (1000.0 * watch.ElapsedTicks) / (double)Stopwatch.Frequency;
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
			if(Sim_Component.Components_Data[dataID].OverlayText.Length > 0)
			{
				Point comppos_grid = new Point(pos.X / 32, pos.Y / 32);
				Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y].Remove(ID);
				if (Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y].Count == 0)
					Sim_Component.CompOverlayGrid[comppos_grid.X, comppos_grid.Y] = null;
			}
            for (int i = 0; i < datapixel.Count; ++i)
            {
                Point currentcoo = pos + datapixel[i].pos;
                Sim_Component.Comp_target.SetPixel(0, currentcoo);
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
            App.simulator.PlaceArea(area, data2place);

            Sim_Component.emptyComponentID[Sim_Component.emptyComponentID_count++] = ID;
            Sim_Component.components[ID] = null;
        }
    }
}
