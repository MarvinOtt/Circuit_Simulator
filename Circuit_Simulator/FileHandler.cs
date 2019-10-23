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

        }

        public static void SaveAs()
        {
            //if (!Simulator.IsSimulating)//KB_currentstate.IsKeyDown(Keys.LeftControl) && KB_currentstate.IsKeyDown(Keys.LeftShift) && KB_currentstate.IsKeyDown(Keys.S) && KB_oldstate.IsKeyUp(Keys.S))
            //{
            //    using (OpenFileDialog dialog = new OpenFileDialog())
            //    {
            //        try
            //        {
            //            string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
            //            System.IO.Directory.CreateDirectory(savepath);
            //            dialog.InitialDirectory = savepath;
            //        }
            //        catch (Exception exp)
            //        {
            //            Console.WriteLine("Error while trying to create Save folder: {0}", exp);
            //        }
            //        dialog.Multiselect = false;
            //        dialog.CheckPathExists = false;
            //        dialog.CheckFileExists = false;
            //        dialog.Title = "Select or Create File to Save";
            //        dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            //        dialog.FilterIndex = 1;
            //        dialog.RestoreDirectory = true;

            //        if (dialog.ShowDialog() == DialogResult.OK)
            //        {
            //            string filename = dialog.FileName;
            //            try
            //            {
            //                FileStream stream = new FileStream(filename, FileMode.Create);
            //                List<byte> bytestosave = new List<byte>();
            //                uint counter = 0;
                            
            //                // Saves Number of chunks
            //                stream.Write(BitConverter.GetBytes(size_mul16x), 0, 4);
            //                stream.Write(BitConverter.GetBytes(size_mul16y), 0, 4);
            //                stream.Write(BitConverter.GetBytes(counter), 0, 4);
            //                stream.Write(bytestosave.ToArray(), 0, bytestosave.Count);
            //                stream.Close();
            //                stream.Dispose();
            //                Console.WriteLine("Saving suceeded. Filename: {0}", filename);

            //            }
            //            catch (Exception exp)
            //            {
            //                Console.WriteLine("Saving failed: {0}", exp);
            //                System.Windows.Forms.MessageBox.Show("Saving failed", null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }
            //        }
            //    }
            //}
        }

        public static void Open()
        {
            //if (!Simulator.IsSimulating)// KB_currentstate.IsKeyDown(Keys.LeftControl) && KB_currentstate.IsKeyDown(Keys.LeftShift) && KB_currentstate.IsKeyDown(Keys.O) && KB_oldstate.IsKeyUp(Keys.O))
            //{
            //    using (OpenFileDialog dialog = new OpenFileDialog())
            //    {
            //        try
            //        {
            //            string savepath = System.IO.Directory.GetCurrentDirectory() + "\\SAVES";
            //            System.IO.Directory.CreateDirectory(savepath);
            //            dialog.InitialDirectory = savepath;
            //        }
            //        catch (Exception exp)
            //        {
            //            Console.WriteLine("Error while trying to create Save folder: {0}", exp);
            //        }

            //        dialog.Multiselect = false;
            //        dialog.CheckFileExists = true;
            //        dialog.CheckPathExists = true;
            //        dialog.Title = "Select File to Open";
            //        dialog.Filter = "Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            //        dialog.FilterIndex = 1;
            //        dialog.RestoreDirectory = true;

            //        if (dialog.ShowDialog() == DialogResult.OK)
            //        {
            //            string filename = dialog.FileName;
            //            try
            //            {
            //                FileStream stream = new FileStream(filename, FileMode.Open);

            //                byte[] intbuffer = new byte[4];

            //                stream.Read(intbuffer, 0, 4);
            //                size_mul16x = BitConverter.ToInt32(intbuffer, 0);
            //                stream.Read(intbuffer, 0, 4);
            //                size_mul16y = BitConverter.ToInt32(intbuffer, 0);
            //                sizex = size_mul16x * 16;
            //                sizey = size_mul16y * 16;

            //                stream.Read(intbuffer, 0, 4);
            //                int count = BitConverter.ToInt32(intbuffer, 0);

            //                byte[] chunkdata = new byte[16 * 16];
            //                for (int i = 0; i < count; ++i)
            //                {
            //                    // Reading X and Y pos of chunks to load
            //                    stream.Read(intbuffer, 0, 4);
            //                    int xpos = BitConverter.ToInt32(intbuffer, 0);
            //                    stream.Read(intbuffer, 0, 4);
            //                    int ypos = BitConverter.ToInt32(intbuffer, 0);

            //                    // Reading chunk data
            //                    stream.Read(chunkdata, 0, 16 * 16);
            //                    for (int x = 0; x < 16; ++x)
            //                    {
            //                        for (int y = 0; y < 16; ++y)
            //                        {
            //                            Set_CellValues((byte)(chunkdata[x + y * 16] % 17), (byte)(chunkdata[x + y * 16] / 17), xpos * 16 + x, ypos * 16 + y);
            //                        }
            //                    }

            //                }

            //                stream.Close();
            //                stream.Dispose();
            //                Console.WriteLine("Loading suceeded. Filename: {0}", filename);

            //            }
            //            catch (Exception exp)
            //            {
            //                Console.WriteLine("Loading failed: {0}", exp);
            //                System.Windows.Forms.MessageBox.Show("Loading failed: " + exp, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //            }
            //        }
            //    }
            //}
        }
    }
}
