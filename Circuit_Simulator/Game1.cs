using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont basefont;

        #region UI

        UI_Handler UI_handler;

        //Textures
        Texture2D Button_Map;


        #endregion

        #region INPUT

        public static KeyboardState kb_state, kb_state_old;
        public static Mouse_States mo_states;

        #endregion

        public static bool IsSimulating;
        public static Texture2D pixel;

        public static int Screenwidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static int Screenheight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        public Game1()
        {
            // Graphic Initialization
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            this.Window.Position = new Point(0, 0);
            int s = System.Windows.Forms.SystemInformation.CaptionHeight;
            graphics.PreferredBackBufferHeight = Screenheight;
            graphics.PreferredBackBufferWidth = Screenwidth;
            graphics.IsFullScreen = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            Window.IsBorderless = false;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

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

            //Play_Button = new Button_Tex(Button_Map, new Rectangle(268, 0, 48, 48), new Vector2(100, 100));

        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            kb_state = Keyboard.GetState();
            mo_states.New = Mouse.GetState();

            //Play_Button.Update();
            UI_handler.Update();

            base.Update(gameTime);
            kb_state_old = kb_state;
            mo_states.Old = mo_states.New;
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            UI_handler.Draw(spriteBatch);
            spriteBatch.DrawString(basefont, IsSimulating.ToString(), new Vector2(100, 100), Color.Red);
            //Play_Button.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
