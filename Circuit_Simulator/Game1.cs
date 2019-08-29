using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
//using Circuit_Simulator.UI;

namespace Circuit_Simulator
{
    public struct Mouse_States
    {
        public MouseState New, Old;

        public Mouse_States(MouseState New, MouseState Old)
        {
            this.New = New;
            this.Old = Old;
        }

        public bool IsLeftButtonToggleOn()
        {
            return New.LeftButton == ButtonState.Pressed && Old.LeftButton == ButtonState.Released;
        }
        public bool IsRightButtonToggleOn()
        {
            return New.RightButton == ButtonState.Pressed && Old.RightButton == ButtonState.Released;
        }
        public bool IsLeftButtonToggleOff()
        {
            return New.LeftButton == ButtonState.Released && Old.LeftButton == ButtonState.Pressed;
        }
        public bool IsRightButtonToggleOff()
        {
            return New.RightButton == ButtonState.Released && Old.RightButton == ButtonState.Pressed;
        }
    }
    public struct Keyboard_States
    {
        public KeyboardState New, Old;

        public Keyboard_States(KeyboardState New, KeyboardState Old)
        {
            this.New = New;
            this.Old = Old;
        }
        public bool IsKeyToggleDown(Keys key)
        {
            return New.IsKeyDown(key) && Old.IsKeyUp(key);
        }
        public bool IsKeyToggleUp(Keys key)
        {
            return !IsKeyToggleDown(key);
        }
    }

    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont basefont;
        public static System.Windows.Forms.Form form;
        public static event EventHandler GraphicsChanged;
        

        #region UI

        UI_Handler UI_handler;

        //Textures
        Texture2D Button_Map;


        #endregion

        #region INPUT

        public static Keyboard_States kb_states;
        public static Mouse_States mo_states;

        private bool GraphicsNeedApplyChanges;

        #endregion

        public static bool IsSimulating;
        public static Texture2D pixel;

        public static int Screenwidth;
        public static int Screenheight;

        // Gets called everytime the Windows Size gets changed
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            // Not Applying Graphics here because when resizing happens, ApplyChanges would be called too often which could cause a crash
            // When resizing happens, the Update Method is not going to be called so long until resizing is finished, and therefore Apply Changes gets only called once
            GraphicsNeedApplyChanges = true;
        }

        public void UpdateEverythingOfGraphics(object sender, EventArgs e)
        {
            Screenwidth = graphics.PreferredBackBufferWidth;
            Screenheight = graphics.PreferredBackBufferHeight;
        }

        public Game1()
        {
            // Graphic Initialization
            GraphicsChanged += UpdateEverythingOfGraphics;
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100,
                PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = true
            };
            IsFixedTimeStep = false;
            Window.IsBorderless = false;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            content = Content;
            IsMouseVisible = true;
            form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            form.MaximizeBox = true;
            form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            form.Resize += Window_ClientSizeChanged;

            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            GraphicsChanged(null, EventArgs.Empty);
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            basefont = Content.Load<SpriteFont>("basefont");
            Button_Map = Content.Load<Texture2D>("UI\\Project Spritemap");
            pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Bgra32);
            Color[] colors = new Color[1];
            colors[0] = Color.White;
            pixel.SetData(colors);

            UI_handler = new UI_Handler(Content);
            UI_handler.Initialize();

        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //----------------//
            // HANDLING INPUT //
            //----------------//
            kb_states.New = Keyboard.GetState();
            mo_states.New = Mouse.GetState();

            //--------------------------------------------//
            // HANDLING EVERYTHING ABOUT GRAPHICS CHANGES //
            //--------------------------------------------//
            if (GraphicsNeedApplyChanges)
            {
                graphics.ApplyChanges();
                GraphicsNeedApplyChanges = false;
                GraphicsChanged(null, EventArgs.Empty);
            }
             
            //-------------------------//
            // HANDLING USER INTERFACE //
            //-------------------------//
            UI_handler.Update();

            //----------------------//
            // BEGIN OF MAIN UPDATE //
            //----------------------//



            //--------------------//
            // END OF MAIN UPDATE //
            //--------------------//

            kb_states.Old = kb_states.New;
            mo_states.Old = mo_states.New;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            UI_handler.Draw(spriteBatch);

            spriteBatch.DrawString(basefont, Screenwidth.ToString(), new Vector2(100, 100), Color.Red);
            spriteBatch.DrawString(basefont, Screenheight.ToString(), new Vector2(100, 130), Color.Red);

            //Play_Button.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
