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
            public static ORIGIN TR = new ORIGIN(1);
            public static ORIGIN BR = new ORIGIN(2);
            public static ORIGIN BL = new ORIGIN(3);

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
            public ORIGIN dest_origin, src_origin;
            public UI_Element parent;
            public UI_Element ego;

            public Pos(int XY, ORIGIN dest_origin = default(ORIGIN), ORIGIN src_origin = default(ORIGIN), UI_Element parent = null)
            {
                X_abs = Y_abs = X = Y = XY;
                this.dest_origin = dest_origin;
                this.src_origin = src_origin;
                this.parent = parent;
                ego = null;
            }
            public Pos(int X, int Y, ORIGIN dest_origin = default(ORIGIN), ORIGIN src_origin = default(ORIGIN), UI_Element parent = null)
            {
                this.X_abs = this.X = X;
                this.Y_abs = this.Y = Y;
                this.dest_origin = dest_origin;
                this.src_origin = src_origin;
                this.parent = parent;
                ego = null;
            }

            public void SetParentIfNotAlreadySet(UI_Element parent)
            {
                if (this.parent == null)
                    this.parent = parent;
            }

            public void Update()
            {
                if(parent != null && dest_origin != ORIGIN.DEFAULT)
                {
                    if(dest_origin == ORIGIN.TR)
                    {
                        X_abs = parent.size.X + X;
                        Y_abs = Y;
                    }
                    else if(dest_origin == ORIGIN.BR)
                    {
                        X_abs = parent.size.X + X;
                        Y_abs = parent.size.Y + Y;
                    }
                    else if (dest_origin == ORIGIN.BL)
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
                if (src_origin != ORIGIN.DEFAULT)
                {
                    if (src_origin == ORIGIN.TR)
                    {
                        X_abs -= ego.size.X;
                    }
                    else if (src_origin == ORIGIN.BR)
                    {
                        X_abs -= ego.size.X;
                        Y_abs -= ego.size.Y;
                    }
                    else if (src_origin == ORIGIN.BL)
                    {
                        Y_abs -= ego.size.Y;
                    }
                }
            }

            public Vector2 ToVector2()
            {
                return new Vector2(X_abs, Y_abs);
            }
            public Point ToPoint()
            {
                return new Point(X_abs, Y_abs);
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
