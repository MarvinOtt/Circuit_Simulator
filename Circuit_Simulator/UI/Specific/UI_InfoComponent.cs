using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI
{
    class UI_InfoComponent : UI_InfoBox
    {
        bool state;
        string layer;


        public UI_InfoComponent(Point pos, Point size) : base(pos, size)
        {

        }
    }
}

