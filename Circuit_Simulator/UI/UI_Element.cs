using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Circuit_Simulator.UI.UI_STRUCTS;

namespace Circuit_Simulator
{
    public class UI_Element
    {
        public Pos pos;
        public Point size;
	    public Point absolutpos;
        public string ID_Name;
        public string Sort_Name;
		public string Description;
        private bool _GetsDrawn = true, _GetsUpdated = true;
        public bool GetsDrawn { get { return _GetsDrawn; } set { _GetsDrawn = value; } }
        public bool GetsUpdated
        {
            get { return _GetsUpdated; }
            set
            {
                bool old = _GetsUpdated;
                _GetsUpdated = value;
                if (!value && old)
                    ChangedUpdate2False();
                else if(value && !old)
                    ChangedUpdate2True();
            }
        }
        public bool CanBeSizeRelated = true, IsTypeOfWindow, UpdateAndDrawChild;
        private UI_Element _parent;
        private UI_Element _child;

        public UI_Element parent
        {
            get { return _parent; }
            set
            {
                SetParent(value);
            }
        }
        public UI_Element child
        {
            get { return _child; }
            set
            {
                SetChild(value);
            }
        }

        public List<Action> UpdateFunctions;
	    public List<Action> DrawFunctions;

        public UI_Element(Pos pos, Point size)
	    {
		    this.pos = pos;
            this.size = size;
            this.pos.ego = this;
            UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public UI_Element(Pos pos, Point size, UI_Element parent)
	    {
		    this.pos = pos;
            this.size = size;
		    this.parent = parent;
            this.pos.ego = this;
		    UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

        public virtual void ChangedUpdate2False()
        {
            child?.ChangedUpdate2False();
            // Should be overridden
        }
        public virtual void ChangedUpdate2True()
        {
            child?.ChangedUpdate2True();
            // Should be overridden
        }

        public void SetChild(UI_Element child)
        {
            _child = child;
            if(_child != null)
                _child.parent = this;
        }
        public void SetParent(UI_Element parent)
        {
            _parent = parent;
            pos.SetParentIfNotAlreadySet(parent);
        }

        public virtual void UpdatePos()
        {
            pos.Update();
            absolutpos = pos.pos;
            _child?.UpdatePos();
        }

        public void UpdateMain()
        {
            pos.ego = this;
            UpdatePos();
            Update();
        }

        private bool IsValid4Scroll()
        {
            return (!UI_Handler.IsInScrollable || (UI_Handler.IsInScrollable && UI_Handler.IsInScrollable_Bounds.Contains(App.mo_states.New.Position)));
        }

        public virtual void Update()
	    {
            if (_GetsUpdated && !(IsTypeOfWindow && UI_Handler.UI_IsWindowHide))
            {
                bool IsValidScroll = IsValid4Scroll();
                bool IsValidUpdate = IsValidScroll && (UI_Handler.ZaWarudo == null || UI_Handler.ZaWarudo == this);
                if (!UI_Handler.UI_AlreadyActivated && IsValidUpdate)
                    UpdateSpecific();
                UpdateAlways();
                if (new Rectangle(absolutpos, size).Contains(App.mo_states.New.Position) && IsValidScroll)
                {
                    UI_Handler.UI_AlreadyActivated = true;
                    UI_Handler.UI_Active_State = 1;
                }
            }
		    for (int i = 0; i < UpdateFunctions.Count; ++i)
		    {
			    UpdateFunctions[i]();
		    }
        }

        public virtual void UpdateSpecific()
        {
			if(Description != null & new Rectangle(absolutpos, size).Contains(App.mo_states.New.Position))
			{
				if(UI_Handler.Hover_InfoBox_Element != this && App.mo_timenomovement > 750.0f)
				{
					UI_Handler.Hover_InfoBox.pos.pos = absolutpos + new Point(0, size.Y + 5);
					UI_Handler.Hover_InfoBox.ui_elements[0].setValue(Description);
					UI_Handler.Hover_InfoBox.UpdateSize(new Point(2));
					UI_Handler.Hover_InfoBox_Element = this;
					UI_Handler.Hover_InfoBox.GetsUpdated = UI_Handler.Hover_InfoBox.GetsDrawn = true;
				}
			}
            // Should be overridden
        }

        protected virtual void UpdateAlways()
        {
			if(UI_Handler.Hover_InfoBox_Element == this)
			{
				if(!new Rectangle(absolutpos, size).Contains(App.mo_states.New.Position) || UI_Handler.UI_AlreadyActivated)
				{
					UI_Handler.Hover_InfoBox.GetsUpdated = UI_Handler.Hover_InfoBox.GetsDrawn = false;
					UI_Handler.Hover_InfoBox_Element = null;
				}
			}
            // Should be overridden
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (_GetsDrawn)
            {
				UpdatePos();
                DrawSpecific(spritebatch);
                for (int i = 0; i < DrawFunctions.Count; ++i)
                {
                    DrawFunctions[i]();
                }
            }

	    }

        protected virtual void DrawSpecific(SpriteBatch spritebatch)
        {
            // Should be overridden

        }
    }
}
