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
    }
}
