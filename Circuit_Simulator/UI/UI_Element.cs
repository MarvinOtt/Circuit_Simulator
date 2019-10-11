using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circuit_Simulator
{
    public class UI_Element
    {
	    public Point pos, size;
	    public Point absolutpos;
        private bool _GetsDrawn = true, _GetsUpdated = true;
        public bool GetsDrawn { get { return _GetsDrawn; } set { _GetsDrawn = value; } }
        public bool GetsUpdated
        {
            get { return _GetsUpdated; }
            set
            {
                _GetsUpdated = value;
                if(!value)
                    ChangedUpdate2False();
            }
        }
        public bool CanBeSizeRelated = true;
        public UI_Element parent;
        public UI_Element child;
        public List<Action> UpdateFunctions;
	    public List<Action> DrawFunctions;

        public UI_Element(Point pos, Point size)
	    {
		    this.pos = pos;
            this.size = size;
            UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

	    public UI_Element(Point pos, Point size, UI_Element parent)
	    {
		    this.pos = pos;
            this.size = size;
		    this.parent = parent;
		    UpdateFunctions = new List<Action>();
		    DrawFunctions = new List<Action>();
        }

        public virtual void ChangedUpdate2False()
        {
            child?.ChangedUpdate2False();
            // Should be overridden
        }

        public virtual void AlwaysUpdate(bool aaa)
        {
            // Should be overridden
        }

        public virtual void UpdatePos()
        {
            absolutpos = parent == null ? pos : pos + parent.absolutpos;
            if (parent != null && CanBeSizeRelated)
            {
                if (pos.X < 0)
                    absolutpos.X += parent.size.X;
                if (pos.Y < 0)
                    absolutpos.Y += parent.size.Y;
            }
        }

        public virtual void Update()
	    {
            //absolutpos = parent == null ? pos : pos + parent.absolutpos;
            //if (parent != null && CanBeSizeRelated)
            //{
            //    if (pos.X < 0)
            //        absolutpos.X += parent.size.X;
            //    if (pos.Y < 0)
            //        absolutpos.Y += parent.size.Y;
            //}
            UpdatePos();
            if (_GetsUpdated)
            {
                if(!UI_Handler.UI_Element_Pressed)
                    UpdateSpecific();
                child?.Update();
                //AlwaysUpdate(aaa && _GetsUpdated);
                if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position) && (Game1.mo_states.New.LeftButton == ButtonState.Pressed || Game1.mo_states.New.RightButton == ButtonState.Pressed))// && (Game1.mo_states.IsLeftButtonToggleOn() || Game1.mo_states.IsLeftButtonToggleOff()))
                    UI_Handler.UI_Element_Pressed = true;
                if (new Rectangle(absolutpos, size).Contains(Game1.mo_states.New.Position))
                    UI_Handler.UI_Active_State = 1;
            }
		    for (int i = 0; i < UpdateFunctions.Count; ++i)
		    {
			    UpdateFunctions[i]();
		    }
        }

        protected virtual void UpdateSpecific()
        {
            // Should be overridden
        }

	    public void Draw(SpriteBatch spritebatch)
        {
            absolutpos = parent == null ? pos : pos + parent.absolutpos;
            if (parent != null && CanBeSizeRelated)
            {
                if (pos.X < 0)
                    absolutpos.X += parent.size.X;
                if (pos.Y < 0)
                    absolutpos.Y += parent.size.Y;
            }
            if (_GetsDrawn)
            {
                DrawSpecific(spritebatch);
                child?.Draw(spritebatch);
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
