using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_Configs;

namespace Circuit_Simulator.UI
{
    public class UI_String : UI_Element
    {
        public string value = "Missing";
        public Generic_Conf conf;
        public UI_String( Point pos, Point size, Generic_Conf conf) : base(pos , size)
        {
            this.conf = conf;
        }
        public UI_String( Point pos, Point size, Generic_Conf conf, string value) : base(pos, size)
        {
            this.conf = conf;
            setValue(value);
        }

        public void setValue(string value)
        {
            this.value = value;
            size.X = conf.font.MeasureString(value).ToPoint().X;
            size.Y = conf.font.MeasureString(value).ToPoint().Y;

        }
    }
}
