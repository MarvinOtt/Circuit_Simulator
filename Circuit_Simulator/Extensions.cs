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
        public static bool AreKeysDown(this KeyboardState kbs, params Keys[] keys)
        {
            for(int i = 0; i < keys.Length; ++i)
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

        //Spritebatch
        public static void DrawFilledRectangle(this SpriteBatch sb, Rectangle rec, Color col)
        {
            sb.Draw(Game1.pixel, rec, col);
        }
    }
}
