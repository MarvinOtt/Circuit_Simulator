using Circuit_Simulator.COMP;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Circuit_Simulator
{
    public class FileHandler
    {
        public static string SaveFile;

        public FileHandler()
        {

        }



        public static void Save()
        {
            if (SaveFile == null)
                SaveAs();
            else
                SaveToPath(SaveFile);
        }

        public static void SaveToPath(string path)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Create);
                List<byte> bytestosave = new List<byte>();
                int compcount = 0;
                int wirecount = 0;
                byte[] bytearray;

                #region Save Library & Comp Table

                stream.Write(BitConverter.GetBytes(CompLibrary.AllLibrarys.Count), 0, 4);
                for (int i = 0; i < CompLibrary.AllLibrarys.Count; ++i)
                {
                    bytearray = CompLibrary.AllLibrarys[i].name.GetBytes();
                    stream.Write(bytearray, 0, bytearray.Length);
                }
                stream.Write(BitConverter.GetBytes(Sim_Component.Components_Data.Count), 0, 4);
                for (int i = 0; i < Sim_Component.Components_Data.Count; ++i)
                {
                    bytearray = Sim_Component.Components_Data[i].name.GetBytes();
                    stream.Write(bytearray, 0, bytearray.Length);
                }
                for (int i = 0; i < Sim_Component.Components_Data.Count; ++i)
                {
                    int LibraryID = CompLibrary.AllLibrarys.IndexOf(Sim_Component.Components_Data[i].library);
                    stream.Write(BitConverter.GetBytes(LibraryID), 0, 4);
                }

                #endregion

                #region Save Wires
                wirecount = Simulator.networks.Count(x => x != null);
                stream.Write(BitConverter.GetBytes(wirecount), 0, 4);

                for (int i = 0; i < Simulator.networks.Length; ++i)
                {
                    if (Simulator.networks[i] != null)
                    {
                        List<Line_Netw> lines = Simulator.networks[i].lines;
                        stream.Write(BitConverter.GetBytes(lines.Count), 0, 4);

                        for (int j = 0; j < lines.Count; ++j)
                        {
                            stream.Write(new byte[1] { lines[j].layers }, 0, 1);
                            stream.Write(BitConverter.GetBytes(lines[j].start.X), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].start.Y), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].end.X), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].end.Y), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].dir.X), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].dir.Y), 0, 4);
                            stream.Write(BitConverter.GetBytes(lines[j].length), 0, 4);

                        }

                    }
                }
                #endregion

                #region Save Components
                compcount = Sim_Component.components.Count(x => x != null);
                stream.Write(BitConverter.GetBytes(compcount), 0, 4);


                for (int i = 0; i < Sim_Component.components.Length; ++i)
                {
                    if (Sim_Component.components[i] != null)
                    {
                        stream.Write(BitConverter.GetBytes(Sim_Component.components[i].dataID), 0, 4);
                        stream.Write(BitConverter.GetBytes(Sim_Component.components[i].pos.X), 0, 4);
                        stream.Write(BitConverter.GetBytes(Sim_Component.components[i].pos.Y), 0, 4);
                        stream.Write(BitConverter.GetBytes(Sim_Component.components[i].rotation), 0, 4);
                    }
                }
                #endregion
                

                stream.Close();
                stream.Dispose();
                Console.WriteLine("Saving suceeded. Filename: {0}", path);

            }
            catch (Exception exp)
            {
                Console.WriteLine("Saving failed: {0}", exp);
                System.Windows.Forms.MessageBox.Show("Saving failed", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SaveAs()
        {
            if (!Simulator.IsSimulating)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    try
                    {
                        string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                        System.IO.Directory.CreateDirectory(savepath);
                        dialog.InitialDirectory = savepath;
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Error while trying to create Save folder: {0}", exp);
                    }
                    dialog.CheckPathExists = false;
                    dialog.CheckFileExists = false;
                    dialog.Title = "SaveAs";
                    dialog.Filter = "DCE files (*.dce)|*.dce|All files (*.*)|*.*";
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;
                    

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = dialog.FileName;
                        SaveFile = filename;
                        SaveToPath(filename);
                    }
                }
            }
        }

        public static void Open()
        {
            if (!Simulator.IsSimulating)
            {
                using (OpenFileDialog dialog = new OpenFileDialog())
                {
                    try
                    {
                        string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
                        System.IO.Directory.CreateDirectory(savepath);
                        dialog.InitialDirectory = savepath;
                    }
                    catch (Exception exp)
                    {
                        Console.WriteLine("Error while trying to create Save folder: {0}", exp);
                    }

                    dialog.Multiselect = false;
                    dialog.CheckFileExists = true;
                    dialog.CheckPathExists = true;
                    dialog.Title = "Select File to Open";
                    dialog.Filter = "DCE files (*.dce)|*.dce|All files (*.*)|*.*";
                    dialog.FilterIndex = 1;
                    dialog.RestoreDirectory = true;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = dialog.FileName;
                        try
                        {
                            FileStream stream = new FileStream(filename, FileMode.Open);

                            byte[] intbuffer = new byte[4];

                            #region Load Tables and Check for Librarys and Components

                            stream.Read(intbuffer, 0, 4);
                            int librarycount = BitConverter.ToInt32(intbuffer, 0);
                            string[] librarynames = new string[librarycount];
                            for (int i = 0; i < librarycount; ++i)
                            {
                                librarynames[i] = stream.ReadNullTerminated();
                            }
                            stream.Read(intbuffer, 0, 4);
                            int libcompcount = BitConverter.ToInt32(intbuffer, 0);
                            string[] libcompnames = new string[libcompcount];
                            int[] complibraryIDs = new int[libcompcount];
                            for (int i = 0; i < libcompcount; ++i)
                            {
                                libcompnames[i] = stream.ReadNullTerminated();
                            }
                            for (int i = 0; i < libcompcount; ++i)
                            {
                                stream.Read(intbuffer, 0, 4);
                                complibraryIDs[i] = BitConverter.ToInt32(intbuffer, 0);
                            }
                            bool AllLibrarysExist = true, AllComponentExist = true;
                            for(int i = 0; i < librarycount; ++i)
                            {
                                if (CompLibrary.AllLibrarys.Find(x => x.name == librarynames[i]) == null)
                                    AllLibrarysExist = false;
                            }
                            if(AllLibrarysExist)
                            {
                                for (int i = 0; i < libcompcount; ++i)
                                {
                                    CompLibrary curlibrary = CompLibrary.AllLibrarys.Find(x => x.name == librarynames[complibraryIDs[i]]);
                                    if (curlibrary.Components.Find(x => x.name == libcompnames[i]) == null)
                                        AllComponentExist = false;
                                }
                            }

                            if (!AllLibrarysExist || !AllComponentExist)
                                throw new Exception("Missing Library/s for this Save File!");

                            int[] compmask = new int[libcompcount];
                            for(int i = 0; i < libcompcount; ++i)
                            {
                                CompLibrary library = CompLibrary.AllLibrarys.Find(x => x.name == librarynames[complibraryIDs[i]]);
                                int compdataID = Sim_Component.Components_Data.FindIndex(x => x.library == library && x.name == libcompnames[i]);
                                compmask[i] = compdataID;
                            }

                            #endregion



                            Array.Clear(Sim_Component.components, 0, Sim_Component.components.Length);
                            Array.Clear(Simulator.networks, 0, Simulator.networks.Length);
                            Array.Clear(Simulator.IsWire, 0, Simulator.IsWire.Length);
                            Array.Clear(Simulator.WireIDs, 0, Simulator.WireIDs.Length);
                            Array.Clear(Simulator.emptyNetworkID, 0, Simulator.emptyNetworkID.Length);
                            Simulator.emptyNetworkID_count = 0;
                            Array.Clear(Sim_Component.emptyComponentID, 0, Sim_Component.emptyComponentID.Length);
                            Sim_Component.emptyComponentID_count = 0;
                            Sim_Component.CompMayneedoverlay.Clear();
                            Array.Clear(Sim_Component.CompType, 0, Sim_Component.CompType.Length);
                            Array.Clear(Sim_Component.CompGrid, 0, Sim_Component.CompGrid.Length);
                            Array.Clear(Sim_Component.CompNetwork, 0, Sim_Component.CompNetwork.Length);
                            Array.Clear(Sim_Component.pins2check, 0, Sim_Component.pins2check.Length);
                            Sim_Component.pins2check_length = 0;
                            Sim_INF_DLL.Comp2UpdateAfterSim_count = 0;

                            Simulator.sec_target.Dispose();
                            Simulator.logic_target.Dispose();

                            Simulator.sec_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                            Simulator.logic_target = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                            Sim_Component.CompTex = new RenderTarget2D(Game1.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.Alpha8, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                            
                            #region LoadWires 

                            stream.Read(intbuffer, 0, 4);
                            int wirecount = BitConverter.ToInt32(intbuffer, 0);
                            Simulator.highestNetworkID = wirecount + 3;
                            for (int i = 4; i < wirecount + 4; ++i)
                            {
                                Network networkbuffer = new Network(i);
                                Simulator.networks[i] = networkbuffer;
                                stream.Read(intbuffer, 0, 4);
                                int linecount = BitConverter.ToInt32(intbuffer, 0);
                                for (int j = 0; j < linecount; ++j)
                                {
                                    stream.Read(intbuffer, 0, 1);
                                    byte layers = intbuffer[0];
                                    stream.Read(intbuffer, 0, 4);
                                    int startx = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int starty = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int endx = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int endy = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int dirx = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int diry = BitConverter.ToInt32(intbuffer, 0);
                                    stream.Read(intbuffer, 0, 4);
                                    int length = BitConverter.ToInt32(intbuffer, 0);


                                    Line_Netw linebuffer = new Line_Netw(new Point(startx, starty), new Point(endx, endy), new Point(dirx, diry), length, layers);
                                    Simulator.networks[i].lines.Add(linebuffer);
                                 
                                }
                                networkbuffer.PlaceNetwork();
                                
                            }
                            Network.Delete(Simulator.FoundNetworks);
                            Simulator.FoundNetworks.Clear();
                            #endregion


                            #region LoadComp

                            stream.Read(intbuffer, 0, 4);
                            int compcount = BitConverter.ToInt32(intbuffer, 0);
                            Sim_Component.nextComponentID = compcount + 1;
                            for (int i = 1; i < compcount + 1; ++i)
                            {

                                stream.Read(intbuffer, 0, 4);
                                Component buffercomp = new Component(compmask[BitConverter.ToInt32(intbuffer, 0)], i);
                                stream.Read(intbuffer, 0, 4);
                                int posX = BitConverter.ToInt32(intbuffer, 0);
                                stream.Read(intbuffer, 0, 4);
                                int posY = BitConverter.ToInt32(intbuffer, 0);
                                stream.Read(intbuffer, 0, 4);
                                int rotation = BitConverter.ToInt32(intbuffer, 0);
                                buffercomp.Place(new Point(posX, posY), rotation);
                                Sim_Component.components[i] = buffercomp;
                            }
                            Network.Delete(Simulator.FoundNetworks);
                            Simulator.FoundNetworks.Clear();
                            #endregion

                            stream.Close();
                            stream.Dispose();


                            Console.WriteLine("Loading suceeded. Filename: {0}", filename);

                        }
                        catch (Exception exp)
                        {
                            Console.WriteLine("Loading failed: {0}", exp);
                            System.Windows.Forms.MessageBox.Show("Loading failed: " + exp, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
        }
    }
}
