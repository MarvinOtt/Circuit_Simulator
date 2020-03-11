using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator.UI
{
	public class UI_NotificationHandler : UI_MultiElement<UI_Box<UI_String>>
	{
		List<float> timeleft;
		public UI_NotificationHandler(Pos pos) : base(pos)
		{
			timeleft = new List<float>();
		}

		public void AddNotification(string text)
		{
			if(timeleft.Count == 0 || ui_elements[ui_elements.Count - 1].ui_elements[0].value != text)
			{
				UI_Box<UI_String> newbox = new UI_Box<UI_String>(new Pos(0), new Point(300, 25));
				newbox.Add_UI_Elements(new UI_String(new Pos(5), Point.Zero, UI_Handler.componentconf, text));
				ui_elements.Add(newbox);
				newbox.UpdateSize(new Point(5));
				timeleft.Add(2500.0f);
			}
			else
			{
				timeleft[ui_elements.Count - 1] = 2500.0f;
			}
			CalcAllPositions();
			UpdatePos();
		}
		private void CalcAllPositions()
		{
			for (int i = 0; i < timeleft.Count; ++i)
			{
				ui_elements[i].pos.pos = new Point(App.Screenwidth - ui_elements[i].size.X - 10, (App.Screenheight - 24 - (ui_elements[i].size.Y + 10) * (timeleft.Count - (i))));
			}
		}

		protected override void UpdateAlways()
		{
			for(int i = 0; i < timeleft.Count; ++i)
			{
				timeleft[i] -= App.lastgametime;

				if(timeleft[i] <= 0.0f)
				{
					timeleft.RemoveAt(i);
					ui_elements.RemoveAt(i);
					i--;
				}
			}
			CalcAllPositions();
		}
	}
}
