using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator
{
    public static class Extensions
    {
        // Keyboard
        public static bool AreKeysDown(this KeyboardState kbs, params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                if (kbs.IsKeyUp(keys[i]))
                    return false;
            }
            return true;
        }
        public static bool AreKeysUp(this KeyboardState kbs, params Keys[] keys)
        {
            for (int i = 0; i < keys.Length; ++i)
            {
                if (kbs.IsKeyDown(keys[i]))
                    return false;
            }
            return true;
        }

        public static bool TryConvertKeyboardInput(Keys keys, bool shift, out char key)
        {

            if (true)
            {
                switch (keys)
                {
                    //Alphabet keys
                    case Keys.A: if (shift) { key = 'A'; } else { key = 'a'; } return true;
                    case Keys.B: if (shift) { key = 'B'; } else { key = 'b'; } return true;
                    case Keys.C: if (shift) { key = 'C'; } else { key = 'c'; } return true;
                    case Keys.D: if (shift) { key = 'D'; } else { key = 'd'; } return true;
                    case Keys.E: if (shift) { key = 'E'; } else { key = 'e'; } return true;
                    case Keys.F: if (shift) { key = 'F'; } else { key = 'f'; } return true;
                    case Keys.G: if (shift) { key = 'G'; } else { key = 'g'; } return true;
                    case Keys.H: if (shift) { key = 'H'; } else { key = 'h'; } return true;
                    case Keys.I: if (shift) { key = 'I'; } else { key = 'i'; } return true;
                    case Keys.J: if (shift) { key = 'J'; } else { key = 'j'; } return true;
                    case Keys.K: if (shift) { key = 'K'; } else { key = 'k'; } return true;
                    case Keys.L: if (shift) { key = 'L'; } else { key = 'l'; } return true;
                    case Keys.M: if (shift) { key = 'M'; } else { key = 'm'; } return true;
                    case Keys.N: if (shift) { key = 'N'; } else { key = 'n'; } return true;
                    case Keys.O: if (shift) { key = 'O'; } else { key = 'o'; } return true;
                    case Keys.P: if (shift) { key = 'P'; } else { key = 'p'; } return true;
                    case Keys.Q: if (shift) { key = 'Q'; } else { key = 'q'; } return true;
                    case Keys.R: if (shift) { key = 'R'; } else { key = 'r'; } return true;
                    case Keys.S: if (shift) { key = 'S'; } else { key = 's'; } return true;
                    case Keys.T: if (shift) { key = 'T'; } else { key = 't'; } return true;
                    case Keys.U: if (shift) { key = 'U'; } else { key = 'u'; } return true;
                    case Keys.V: if (shift) { key = 'V'; } else { key = 'v'; } return true;
                    case Keys.W: if (shift) { key = 'W'; } else { key = 'w'; } return true;
                    case Keys.X: if (shift) { key = 'X'; } else { key = 'x'; } return true;
                    case Keys.Y: if (shift) { key = 'Y'; } else { key = 'y'; } return true;
                    case Keys.Z: if (shift) { key = 'Z'; } else { key = 'z'; } return true;

                    //Decimal keys
                    case Keys.D0: if (shift) { key = '='; } else { key = '0'; } return true;
                    case Keys.D1: if (shift) { key = '!'; } else { key = '1'; } return true;
                    case Keys.D2: if (shift) { key = '"'; } else { key = '2'; } return true;
                    case Keys.D3: if (shift) { key = (char)0; return false; } else { key = '3'; } return true;
                    case Keys.D4: if (shift) { key = '$'; } else { key = '4'; } return true;
                    case Keys.D5: if (shift) { key = '%'; } else { key = '5'; } return true;
                    case Keys.D6: if (shift) { key = '&'; } else { key = '6'; } return true;
                    case Keys.D7: if (shift) { key = '/'; } else { key = '7'; } return true;
                    case Keys.D8: if (shift) { key = '('; } else { key = '8'; } return true;
                    case Keys.D9: if (shift) { key = ')'; } else { key = '9'; } return true;

                    //Decimal numpad keys
                    case Keys.NumPad0: key = '0'; return true;
                    case Keys.NumPad1: key = '1'; return true;
                    case Keys.NumPad2: key = '2'; return true;
                    case Keys.NumPad3: key = '3'; return true;
                    case Keys.NumPad4: key = '4'; return true;
                    case Keys.NumPad5: key = '5'; return true;
                    case Keys.NumPad6: key = '6'; return true;
                    case Keys.NumPad7: key = '7'; return true;
                    case Keys.NumPad8: key = '8'; return true;
                    case Keys.NumPad9: key = '9'; return true;

                    //Special keys
                    case Keys.OemPlus: if (shift) { key = '*'; } else { key = '+'; } return true;
                    case Keys.OemPeriod: if (shift) { key = ':'; } else { key = '.'; } return true;
                    case Keys.OemMinus: if (shift) { key = '_'; } else { key = '-'; } return true;
                    case Keys.OemComma: if (shift) { key = ';'; } else { key = ','; } return true;
                    case Keys.Space: key = ' '; return true;
                }
            }

            key = (char)0;
            return false;
        }

        // Spritebatch
        public static void DrawFilledRectangle(this SpriteBatch sb, Rectangle rec, Color col)
        {
            sb.Draw(Game1.pixel, rec, col);
        }

        public static void DrawLine(this SpriteBatch sb, int x1, int y1, int x2, int y2, Color color)
        {
            DrawLine(sb, new Point(x1, y1), new Point(x2, y2), color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, int x1, int y1, int x2, int y2, Color color, float thickness)
        {
            DrawLine(sb, new Point(x1, y1), new Point(x2, y2), color, thickness);
        }
        public static void DrawLine(this SpriteBatch sb, Point point1, Point point2, Color color)
        {
            DrawLine(sb, point1, point2, color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, Point point1, Point point2, Color color, float thickness)
        {
            // calculate the distance between the two vectors
            float distance = Vector2.Distance(point1.ToVector2(), point2.ToVector2());

            // calculate the angle between the two vectors
            float angle = (float)System.Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);

            DrawLine(sb, point1, distance, angle, color, thickness);
        }
        public static void DrawLine(this SpriteBatch sb, Point point, float length, float angle, Color color)
        {
            DrawLine(sb, point, length, angle, color, 1.0f);
        }
        public static void DrawLine(this SpriteBatch sb, Point point, float length, float angle, Color color, float thickness)
        {
            sb.Draw(Game1.pixel, point.ToVector2(), null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void DrawHollowRectangle(this SpriteBatch sb, Rectangle rec, Color col, int strokewidth)
        {
            rec.Size -= new Point(strokewidth, strokewidth);
            sb.DrawLine(rec.Location, rec.Location + new Point(rec.Size.X + strokewidth, 0), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(strokewidth, 0), rec.Location + new Point(strokewidth, rec.Size.Y + strokewidth), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(rec.Size.X + strokewidth, strokewidth), rec.Location + rec.Size + new Point(strokewidth, strokewidth), col, strokewidth);
            sb.DrawLine(rec.Location + new Point(strokewidth, rec.Size.Y), rec.Location + rec.Size + new Point(strokewidth, 0), col, strokewidth);
            //Game1.spriteBatch.Draw(Game1.pixel, pos, new Rectangle((int)pos.X, (int)pos.Y, (int)size.X, (int)size.Y), col, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }

        // LINQ
        public static void ForEach<T>(this IEnumerable<T> list, Action<T> block)
        {
            foreach (var item in list)
            {
                block(item);
            }
        }

        // Textures
        private static byte[] pixelbyte = new byte[4];
        public static void SetPixel(this Texture2D tex, byte data, Point pos)
        {
            pixelbyte[3] = data;
            int mulpos = pos.Y * tex.Width + pos.X;
            tex.SetData(0, new Rectangle(pos, new Point(1)), pixelbyte, 0, 4);
        }

        // Arrays
        public static void GetArea<T>(this T[,] arr, T[,] dest, Rectangle source)
        {
            for(int x = 0; x < source.Width; ++x)
            {
                for (int y = 0; y < source.Height; ++y)
                {
                    dest[x, y] = arr[source.X + x, source.Y + y];
                }
            }
        }

        public static void GetAreaWithMask(this byte[,] arr, byte[] dest, Rectangle source, byte mask)
        {
            for (int x = 0; x < source.Width; ++x)
            {
                for (int y = 0; y < source.Height; ++y)
                {
                    dest[x + y * source.Width] = (byte)(arr[source.X + x, source.Y + y] & mask);
                }
            }
        }

        // Reading String from File
        public static string ReadNullTerminated(this System.IO.FileStream rdr)
        {
            var bldr = new System.Text.StringBuilder();
            int nc;
            while ((nc = rdr.ReadByte()) > 0)
                bldr.Append((char)nc);

            return bldr.ToString();
        }

        public static void CMD_Execute(string cmd, string args)
        {
            Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = cmd;
            p.StartInfo.Arguments = args;
            p.StartInfo.CreateNoWindow = false;

            //required to capture standard output 
            p.StartInfo.RedirectStandardOutput = false;
            p.StartInfo.UseShellExecute = false;
            p.Start();
            p.WaitForExit();

            ////read the command line output 
            //StreamReader sr = p.StandardOutput;
            //return (sr.ReadToEnd());
        }

        public static string MakeRelativePath(string workingDirectory, string fullPath)
        {
            string result = string.Empty;
            int offset;

            // this is the easy case.  The file is inside of the working directory.
            if (fullPath.StartsWith(workingDirectory))
            {
                return fullPath.Substring(workingDirectory.Length + 1);
            }

            // the hard case has to back out of the working directory
            string[] baseDirs = workingDirectory.Split(new char[] { ':', '\\', '/' });
            string[] fileDirs = fullPath.Split(new char[] { ':', '\\', '/' });

            // if we failed to split (empty strings?) or the drive letter does not match
            if (baseDirs.Length <= 0 || fileDirs.Length <= 0 || baseDirs[0] != fileDirs[0])
            {
                // can't create a relative path between separate harddrives/partitions.
                return fullPath;
            }

            // skip all leading directories that match
            for (offset = 1; offset < baseDirs.Length; offset++)
            {
                if (baseDirs[offset] != fileDirs[offset])
                    break;
            }

            // back out of the working directory
            for (int i = 0; i < (baseDirs.Length - offset); i++)
            {
                result += "..\\";
            }

            // step into the file path
            for (int i = offset; i < fileDirs.Length - 1; i++)
            {
                result += fileDirs[i] + "\\";
            }

            // append the file
            result += fileDirs[fileDirs.Length - 1];

            return result;
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length + 1];
            System.Buffer.BlockCopy(Encoding.ASCII.GetBytes(str), 0, bytes, 0, bytes.Length - 1);
            bytes[bytes.Length - 1] = 0;
            return bytes;
        }

        //Move Item in List
        public static void Move<T>(this List<T> list, int oldindex, int newindex)
        {
            var item = list[oldindex];

            list.RemoveAt(oldindex);

            if (newindex > oldindex) newindex--;

            list.Insert(newindex, item);
        }
    }
}
