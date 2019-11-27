using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Circuit_Simulator.COMP
{
    public class CompLibrary
    {
        private const int NOT_LOADED = 0;
        private const int LOAD_FAILED = 1;
        private const int LOADED = 2;

        public static List<CompLibrary> AllLibraries = new List<CompLibrary>();
        public List<CompData> Components;
        public string name;
        public string SaveFile;
        public bool IsFold = true;
        public int STATE;

        public CompLibrary(string name, string SaveFile)
        {
            STATE = 0;
            this.name = name;
            this.SaveFile = SaveFile;
            Components = new List<CompData>();
            AllLibraries.Add(this);
        }

        public void AddComponent(CompData comp)
        {
            comp.library = this;
            Components.Add(comp);
            Sim_Component.Components_Data.Add(comp);
        }

        public void Save()
        {
            if (SaveFile == null)
                SaveAs();
            else
                SaveToPath(SaveFile);
        }

        public void SaveToPath(string path)
        {
            try
            {
                FileStream stream = new FileStream(path, FileMode.Create);
                List<byte> bytestosave = new List<byte>();
                int compcount = 0;

                byte[] bytearray = name.GetBytes();
                stream.Write(bytearray, 0, bytearray.Length);

                #region Save ComponentData
                compcount = Components.Count;
                stream.Write(BitConverter.GetBytes(compcount), 0, 4);

                for(int i = 0; i <compcount; ++i)
                {
                    Components[i].Save(stream);
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

        public void SaveAs()
        {
            if (!Simulator.IsSimulating)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    try
                    {
                        string savepath = System.IO.Directory.GetCurrentDirectory() + "\\LIBRARYS";
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
                    dialog.Filter = "CDL files (*.cdl)|*.cdl|All files (*.*)|*.*";
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


        public void Load()
        {
            try
            {
                byte[] intbuffer = new byte[4];

                FileStream stream = new FileStream(SaveFile, FileMode.Open);
                StreamReader streamreader = new StreamReader(stream);
                string libraryname = stream.ReadNullTerminated();
                name = libraryname;

                stream.Read(intbuffer, 0, 4);
                int compcount = BitConverter.ToInt32(intbuffer, 0);
                for (int j = 0; j < compcount; ++j)
                {
                    string name = stream.ReadNullTerminated();
                    string category = stream.ReadNullTerminated();
                    stream.Read(intbuffer, 0, 1);
                    bool IsOverlay = BitConverter.ToBoolean(intbuffer, 0);
                    stream.Read(intbuffer, 0, 1);
                    bool IsClickable = BitConverter.ToBoolean(intbuffer, 0);
                    stream.Read(intbuffer, 0, 1);
                    bool IsUpdateAfterSim = BitConverter.ToBoolean(intbuffer, 0);
                    stream.Read(intbuffer, 0, 1);
                    bool ShowOverlay = BitConverter.ToBoolean(intbuffer, 0);

                    CompData newcomp = new CompData(name, category, IsOverlay, IsClickable, IsUpdateAfterSim);
                    newcomp.ShowOverlay = ShowOverlay;
                    stream.Read(intbuffer, 0, 4);
                    int Pixel_Num = BitConverter.ToInt32(intbuffer, 0);
                    for (int k = 0; k < Pixel_Num; ++k)
                    {
                        Point pos = Point.Zero;
                        stream.Read(intbuffer, 0, 4);
                        pos.X = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        pos.Y = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 1);
                        byte type = intbuffer[0];
                        newcomp.addData(new ComponentPixel(pos, type));
                    }
                    stream.Read(intbuffer, 0, 4);
                    int OverlayLine_Num = BitConverter.ToInt32(intbuffer, 0);
                    for (int k = 0; k < OverlayLine_Num; ++k)
                    {
                        Point pos = Point.Zero;
                        stream.Read(intbuffer, 0, 4);
                        pos.X = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        pos.Y = BitConverter.ToInt32(intbuffer, 0);

                        Point pos2 = Point.Zero;
                        stream.Read(intbuffer, 0, 4);
                        pos2.X = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        pos2.Y = BitConverter.ToInt32(intbuffer, 0);
                        stream.Read(intbuffer, 0, 4);
                        float layers = BitConverter.ToSingle(intbuffer, 0);
                        newcomp.addOverlayLine(new Line(pos, pos2), layers);
                    }
                    stream.Read(intbuffer, 0, 4);
                    newcomp.internalstate_length = BitConverter.ToInt32(intbuffer, 0);
                    stream.Read(intbuffer, 0, 4);
                    newcomp.ClickAction_Type = BitConverter.ToInt32(intbuffer, 0);
                    if(IsUpdateAfterSim)
                        newcomp.Code_AfterSimAction = stream.ReadNullTerminated();
                    newcomp.Code_Sim = stream.ReadNullTerminated();
                    if (IsUpdateAfterSim)
                        newcomp.Code_AfterSimAction_FuncName = stream.ReadNullTerminated();
                    newcomp.Code_Sim_FuncName = stream.ReadNullTerminated();

                    newcomp.Finish();
                    AddComponent(newcomp);
                }
                STATE = LOADED;
            }
            catch (Exception exp)
            {
                STATE = LOAD_FAILED;
                Console.WriteLine("Loading Library failed: {0}", exp);
                System.Windows.Forms.MessageBox.Show("Loading Library failed", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
