using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    public class UI_STRUCTS
    {
        public struct ORIGIN
        {
            public int value;
            public ORIGIN(int value)
            {
                this.value = value;
            }
            public static ORIGIN DEFAULT = new ORIGIN(0);
            public static ORIGIN TOPRIGHT = new ORIGIN(1);
            public static ORIGIN BOTTOMRIGHT = new ORIGIN(2);
            public static ORIGIN BOTTOMLEFT = new ORIGIN(3);

            public static bool operator ==(ORIGIN a, ORIGIN b)
            {
                return a.value == b.value;
            }
            public static bool operator !=(ORIGIN a, ORIGIN b)
            {
                return a.value != b.value;
            }
        }

        public struct Pos
        {
            public int X, Y, X_abs, Y_abs;
            public Point pos
            {
                get { return new Point(X_abs, Y_abs); }
                set
                {
                    X = value.X;
                    Y = value.Y;
                }
            }
            public ORIGIN origin;
            private UI_Element parent;

            public Pos(int XY, ORIGIN origin = default(ORIGIN), UI_Element parent = null)
            {
                X_abs = Y_abs = X = Y = XY;
                this.origin = origin;
                this.parent = parent;
            }
            public Pos(int X, int Y, ORIGIN origin = default(ORIGIN), UI_Element parent = null)
            {
                this.X_abs = this.X = X;
                this.Y_abs = this.Y = Y;
                this.origin = origin;
                this.parent = parent;
            }

            public void SetParentIfNotAlreadySet(UI_Element parent)
            {
                if (this.parent == null)
                    this.parent = parent;
            }

            public void Update()
            {
                if(parent != null && origin != ORIGIN.DEFAULT)
                {
                    if(origin == ORIGIN.TOPRIGHT)
                    {
                        X_abs = parent.size.X + X;
                        Y_abs = Y;
                    }
                    else if(origin == ORIGIN.BOTTOMRIGHT)
                    {
                        X_abs = parent.size.X + X;
                        Y_abs = parent.size.Y + Y;
                    }
                    else if (origin == ORIGIN.BOTTOMLEFT)
                    {
                        X_abs = X;
                        Y_abs = parent.size.Y + Y;
                    }
                }
                else
                {
                    X_abs = X;
                    Y_abs = Y;
                }
            }

            public Vector2 ToVector2()
            {
                return new Vector2(X, Y);
            }
            public Point ToPoint()
            {
                return new Point(X, Y);
            }

            public static Pos operator +(Pos a, Pos b)
            {
                return new Pos(a.X + b.X, a.Y + b.Y);
            }
            public static Pos operator -(Pos a, Pos b)
            {
                return new Pos(a.X - b.X, a.Y - b.Y);
            }
            public static Pos Zero = new Pos(0);
        }
        
    }
}
