using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ScintillaNET;
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;

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

        public bool IsMiddleButtonToggleOn()
        {
            return New.MiddleButton == ButtonState.Pressed && Old.MiddleButton == ButtonState.Released;
        }
        public bool IsMiddleButtonToggleOff()
        {
            return New.MiddleButton == ButtonState.Released && Old.MiddleButton == ButtonState.Pressed;
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
            return New.IsKeyUp(key) && Old.IsKeyDown(key);
        }
    }


    public class App : Game
    {
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        public static App ME;
        public static System.Windows.Forms.Form form;
        public static event EventHandler GraphicsChanged;
        public static Simulator simulator;
        public static Matrix[] Render_PreviousMatrix = new Matrix[2];
        public static int Render_PreviousMatrix_Index = 0;
        SpriteBatch spriteBatch;
        public static SpriteFont basefont;

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

        public static Texture2D pixel;

        public static int Screenwidth;
        public static int Screenheight;

        // Gets called everytime the Windows Size gets changed
        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width != 0)
                graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            if (Window.ClientBounds.Height != 0)
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

        void SetToPreserve(object sender, PreparingDeviceSettingsEventArgs eventargs) { eventargs.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents; }

        private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.LWin)
            {
                if(form.WindowState == System.Windows.Forms.FormWindowState.Maximized)
                    form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            }
        }
        void GetsMinimized(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            System.Windows.Forms.CloseReason c = e.CloseReason;
        }
        protected override void OnExiting(Object sender, EventArgs args)
        {
            base.OnExiting(sender, args);
            if (Sim_INF_DLL.SimDLL_Handle != IntPtr.Zero)
                DLL_Methods.FreeLibrary(Sim_INF_DLL.SimDLL_Handle);
            // Stop the threads
        }

        public App()
        {
            // Graphic Initialization
            ME = this;
            GraphicsChanged += UpdateEverythingOfGraphics;
            graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width - 100,
                PreferredBackBufferHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height - 100,
                IsFullScreen = false,
                SynchronizeWithVerticalRetrace = true
                
            };
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(SetToPreserve);
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
            form.FormClosing += GetsMinimized;
            form.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            GraphicsChanged(null, EventArgs.Empty);
            base.Initialize();
        }

        public void ExitApplication()
        {
            Environment.Exit(0);
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

            // Check for write permissions for the simulation files
            bool WriteAccessToMainFolder = Extensions.HasWritePermissions(Directory.GetCurrentDirectory());
            if(!WriteAccessToMainFolder)
            {
                Console.WriteLine("Could not write important simulation files. Run the application in adminstrator mode or check the permissions of the application folder to fix this.");
                System.Windows.Forms.MessageBox.Show("Could not write important simulation files. Run the application in adminstrator mode or check the permissions of the application folder to fix this.", null, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                ExitApplication();
            }

            UI_handler = new UI_Handler(Content);
            UI_handler.Initialize(spriteBatch);

            simulator = new Simulator();
            GraphicsChanged(null, EventArgs.Empty);
            string pathtoexe = Directory.GetCurrentDirectory();
            
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
           
            //----------------//
            // HANDLING INPUT //
            //----------------//
            kb_states.New = Keyboard.GetState();
            mo_states.New = Mouse.GetState();
            if (IsActive)
            {
                //--------------------------------------------//
                // HANDLING EVERYTHING ABOUT GRAPHICS CHANGES //
                //--------------------------------------------//
                if (GraphicsNeedApplyChanges)
                {
                    graphics.ApplyChanges();
                    GraphicsNeedApplyChanges = false;
                    if(Screenwidth != 0 && Screenheight != 0)
                        GraphicsChanged(null, EventArgs.Empty);
                }

                //-------------------------//
                // HANDLING USER INTERFACE //
                //-------------------------//
                UI_handler.Update();

                //----------------------//
                // BEGIN OF MAIN UPDATE //
                //----------------------//

                simulator.Update();

                //--------------------//
                // END OF MAIN UPDATE //
                //--------------------//
            }
            kb_states.Old = kb_states.New;
            mo_states.Old = mo_states.New;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            simulator.Draw(spriteBatch);

            UI_handler.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
            watch.Stop();
        }
    }
}
