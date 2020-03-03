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
        public static bool IsUpToDate = true;

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
            FileStream s = null;
            try
            {
                FileStream stream = new FileStream(path, FileMode.Create);
                s = stream;
                List<byte> bytestosave = new List<byte>();
                byte[] bytearray2 = new byte[1];
                int compcount = 0;
                int wirecount = 0;
                byte[] bytearray;

                stream.Write(BitConverter.GetBytes(Simulator.ProjectSizeX), 0, 4);
                stream.Write(BitConverter.GetBytes(Simulator.ProjectSizeY), 0, 4);

                #region Save Library & Comp Table


                stream.Write(BitConverter.GetBytes(CompLibrary.AllUsedLibraries.Count), 0, 4);
                string workingPath = SaveFile;
                int index = 0;
                for (int i = workingPath.Length - 1; i >= 0; --i)
                {
                    if (workingPath[i] == '\\')
                    {
                        index = i;
                        break;
                    }
                }
                workingPath = workingPath.Remove(index, workingPath.Length - (index));

                for (int i = 0; i < CompLibrary.AllUsedLibraries.Count; ++i)
                {
                    CompLibrary curlib = CompLibrary.AllUsedLibraries[i];
                    string relPath = Extensions.MakeRelativePath(workingPath, curlib.SaveFile);
                    bytearray = relPath.GetBytes();
                    stream.Write(bytearray, 0, bytearray.Length);
                }

                HashSet<int> compdatatypes = new HashSet<int>();
                Sim_Component.components.ForEach(x => { if (x != null) { compdatatypes.Add(x.dataID); } });
                int[] compdatatypes_array = compdatatypes.ToArray();
                stream.Write(BitConverter.GetBytes(Sim_Component.Components_Data.Count), 0, 4);
                stream.Write(BitConverter.GetBytes(compdatatypes_array.Length), 0, 4);
                for (int i = 0; i < Sim_Component.Components_Data.Count; ++i)
                {
                    bytearray = Sim_Component.Components_Data[i].name.GetBytes();
                    stream.Write(bytearray, 0, bytearray.Length);
                }
                for (int i = 0; i < compdatatypes_array.Length; ++i)
                {
                    stream.Write(BitConverter.GetBytes(compdatatypes_array[i]), 0, 4);
                }
                //for (int i = 0; i < Sim_Component.Components_Data.Count; ++i)
                //{
                //    int LibraryID = CompLibrary.AllLibraries.IndexOf(Sim_Component.Components_Data[i].library);
                //    stream.Write(BitConverter.GetBytes(LibraryID), 0, 4);
                //}

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
                            stream.Write(new byte[1] { (byte)(lines[j].layers) }, 0, 1);
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
                        bytearray2[0] = Sim_Component.components[i].rotation;
                        stream.Write(bytearray2, 0, 1);
                        CompData compdata = Sim_Component.Components_Data[Sim_Component.components[i].dataID];
                        stream.Write(BitConverter.GetBytes(compdata.valuebox_length), 0, 4);
                        for (int j = 0; j < compdata.valuebox_length; ++j)
                        {
                            stream.Write(BitConverter.GetBytes(Sim_Component.components[i].totalstates[compdata.internalstate_length + compdata.OverlaySeg_length + j]), 0, 4);
                        }
                    }
                }
                #endregion


                stream.Close();
                stream.Dispose();
                Console.WriteLine("Saving suceeded. Filename: {0}", path);
                IsUpToDate = true;

            }
            catch (Exception exp)
            {
                Console.WriteLine("Saving failed: {0}", exp);
                if (s != null)
                    s.Close();
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
                    dialog.Filter = "DCE file (*.dce)|*.dce|All files (*.*)|*.*";
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

        public static void OpenCurrent()
        {
            if (SaveFile != null)
            {
                OpenAs(SaveFile);
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
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        string filename = dialog.FileName;
                        OpenAs(filename);
                    }
                    dialog.Dispose();
                }


            }
        }

        public static void OpenAs(string filename)
        {
            if (!Simulator.IsSimulating)
            {
                try
                {
                    FileStream stream = new FileStream(filename, FileMode.Open);

                    byte[] intbuffer = new byte[4];
                    stream.Read(intbuffer, 0, 4);
                    int XGridSize = BitConverter.ToInt32(intbuffer, 0);
                    Simulator.SIZEX = Simulator.ProjectSizeX = XGridSize;
                    Simulator.MAXCOO = Simulator.SIZEX - Simulator.BORDERSIZE;

                    stream.Read(intbuffer, 0, 4);
                    int YGridSize = BitConverter.ToInt32(intbuffer, 0);
                    Simulator.SIZEY = Simulator.ProjectSizeY = YGridSize;
                    Simulator.linedrawingmatrix = Matrix.CreateOrthographicOffCenter(0, Simulator.SIZEX + 0.01f, Simulator.SIZEY + 0.01f, 0, 0, 1);
                    Simulator.sim_effect.Parameters["worldsizex"].SetValue(Simulator.SIZEX);
                    Simulator.sim_effect.Parameters["worldsizey"].SetValue(Simulator.SIZEY);

                    Sim_Component.components = new Component[Sim_Component.components.Length];
                    Simulator.networks = new Network[Simulator.networks.Length];

                    Simulator.IsWire = new byte[Simulator.SIZEX, Simulator.SIZEY];
                    Simulator.WireIDs = new int[Simulator.SIZEX / 2, Simulator.SIZEY / 2, Simulator.LAYER_NUM];
                    Simulator.WireIDPs = new int[Simulator.SIZEX, Simulator.SIZEY];
                    Simulator.CalcGridData = new byte[Simulator.SIZEX, Simulator.SIZEY];
                    Simulator.CalcGridStat = new byte[Simulator.SIZEX, Simulator.SIZEY];
                    Simulator.networks = new Network[10000000];
                    Simulator.emptyNetworkIDs = new int[10000000];

                    Sim_Component.CompGrid = new int[Simulator.SIZEX / 32, Simulator.SIZEY / 32][];
                    Sim_Component.CompNetwork = new byte[Simulator.SIZEX, Simulator.SIZEY];
                    Sim_Component.components = new Component[10000000];
                    Sim_Component.pins2check = new Point[20000000];
                    Sim_Component.overlaylines = new VertexPositionLine[1000000];
                    Sim_Component.emptyComponentID = new int[10000000];
                    Sim_Component.CompType = new byte[Simulator.SIZEX, Simulator.SIZEY];

                    Simulator.emptyNetworkIDs_count = 0;
                    Sim_Component.emptyComponentID_count = 0;
                    Sim_Component.CompMayneedoverlay.Clear();
                    
                    Sim_Component.pins2check_length = 0;
                    Simulator.cursimframe = 0;

                    Simulator.seclogic_target.Dispose();
                    Simulator.logic_target.Dispose();
                    Simulator.WireCalc_target.Dispose();
                    Sim_Component.Comp_target.Dispose();
                    Sim_Component.Highlight_target.Dispose();
                    Sim_Component.IsEdge_target.Dispose();

                    Simulator.seclogic_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    Simulator.logic_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    Simulator.WireCalc_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    Sim_Component.Comp_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    Sim_Component.Highlight_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
                    Sim_Component.IsEdge_target = new RenderTarget2D(App.graphics.GraphicsDevice, Simulator.SIZEX, Simulator.SIZEY, false, SurfaceFormat.HalfSingle, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

                    Simulator.highestNetworkID = 4;

                    #region Load Tables and Check for Components

                    stream.Read(intbuffer, 0, 4);
                    int library_count = BitConverter.ToInt32(intbuffer, 0);
                    string workingPath = filename;
                    int pathindex = 0;
                    for (int i = workingPath.Length - 1; i >= 0; --i)
                    {
                        if (workingPath[i] == '\\')
                        {
                            pathindex = i;
                            break;
                        }
                    }
                    workingPath = workingPath.Remove(pathindex + 1, workingPath.Length - (pathindex + 1));
                    //CompLibrary.AllUsedLibraries.Clear();
                    //Sim_Component.Components_Data.Clear();
                    List<string> Libraries2Load = new List<string>();
                    for (int i = 0; i < library_count; ++i)
                    {
                        string relPath = stream.ReadNullTerminated();
                        string absolutepath = workingPath + relPath;
                        Libraries2Load.Add(absolutepath);
                        //CompLibrary newlib = new CompLibrary(null, absolutepath);
                        //newlib.Load();
                        //CompLibrary.AllUsedLibraries.Add(newlib);
                    }
                    bool AllLibrarysLoaded = Sim_INF_DLL.LoadLibrarys(Libraries2Load.ToArray());
                    if (!AllLibrarysLoaded)
                        throw new Exception("Project could not be loaded: Libraries missing");

                    stream.Read(intbuffer, 0, 4);
                    int compdata_count = BitConverter.ToInt32(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    int compdatatypes_count = BitConverter.ToInt32(intbuffer, 0);
                    string[] compdata_names = new string[compdata_count];
                    List<int> compdatatypes = new List<int>();
                    for (int i = 0; i < compdata_count; ++i)
                    {
                        compdata_names[i] = stream.ReadNullTerminated();
                    }
                    for (int i = 0; i < compdatatypes_count; ++i)
                    {
                        stream.Read(intbuffer, 0, 4);
                        compdatatypes.Add(BitConverter.ToInt32(intbuffer, 0));
                    }
                    // Check if all Components are loaded
                    bool AllLoaded = true;
                    List<string> NotFoundComponents = new List<string>();
                    int[] compdata_index = new int[compdata_count];
                    for (int i = 0; i < compdata_count; ++i)
                    {
                        CompData curcomp = Sim_Component.Components_Data.Find(x => x.name == compdata_names[i]);
                        if (curcomp == null && compdatatypes.Exists(x => x == i))
                        {
                            AllLoaded = false;
                            NotFoundComponents.Add(compdata_names[i]);
                        }
                        else
                        {
                            int index = Sim_Component.Components_Data.IndexOf(curcomp);
                            compdata_index[i] = index;
                        }
                    }

                    if (!AllLoaded)
                    {
                        string message = "Following Components not loaded: ";
                        for (int i = 0; i < NotFoundComponents.Count; ++i)
                            message += NotFoundComponents[i] + " ";
                        throw new Exception(message);
                    }

                    #endregion

                    #region LoadWires 

                    stream.Read(intbuffer, 0, 4);
                    int wirecount = BitConverter.ToInt32(intbuffer, 0);
                    Simulator.highestNetworkID = 4 + wirecount;
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
                            networkbuffer.lines.Add(linebuffer);

                        }
                        networkbuffer.PlaceNetwork();

                    }
                    #endregion


                    #region LoadComp

                    stream.Read(intbuffer, 0, 4);
                    int compcount = BitConverter.ToInt32(intbuffer, 0);
                    Sim_Component.nextComponentID = compcount + 1;
                    for (int i = 1; i < compcount + 1; ++i)
                    {

                        stream.Read(intbuffer, 0, 4);
                        Component buffercomp = new Component(compdata_index[BitConverter.ToInt32(intbuffer, 0)], i);
                        stream.Read(intbuffer, 0, 4);
                        int posX = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        int posY = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 1);
                        int rotation = intbuffer[0];
                        stream.Read(intbuffer, 0, 4);
                        int valuebox_length = BitConverter.ToInt32(intbuffer, 0);

                        CompData compdata = Sim_Component.Components_Data[compdata_index[buffercomp.dataID]];
                        for (int j = 0; j < valuebox_length; ++j)
                        {
                            stream.Read(intbuffer, 0, 4);
                            int curval = BitConverter.ToInt32(intbuffer, 0);
                            buffercomp.totalstates[compdata.internalstate_length + compdata.OverlaySeg_length + j] = curval;
                        }
						
                        Sim_Component.components[i] = buffercomp;
                        buffercomp.Place(new Point(posX, posY), (byte)rotation, true);
                    }
                    Network.Delete(Simulator.FoundNetworks);
                    Simulator.FoundNetworks.Clear();
                    #endregion

                    stream.Close();
                    stream.Dispose();
                    

                    Console.WriteLine("Loading suceeded. Filename: {0}", filename);
                    IsUpToDate = true;
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Loading failed:\n{0}", exp);
                    System.Windows.Forms.MessageBox.Show("Loading failed:\n" + exp.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
