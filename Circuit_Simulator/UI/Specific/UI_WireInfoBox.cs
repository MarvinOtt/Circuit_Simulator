using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Circuit_Simulator.UI.Specific
{
    public class UI_WireInfoBox: UI_InfoBox
    {
        bool state;
        string layer;


        public UI_WireInfoBox(Point pos, Point size) : base(pos, size)
        {

        }
    }
}
