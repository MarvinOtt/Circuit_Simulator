using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Circuit_Simulator.UI;

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

        #region UI

        UI_Handler UI_handler;

        //Textures
        Texture2D Button_Map;

        //Entities
        Button_Tex Play_Button;

        #endregion

        #region INPUT

        public static KeyboardState kb_state, kb_state_old;
        public static Mouse_States mo_states;

        #endregion

        public static int Screenwidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
        public static int Screenheight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

        public Game1()
        {
            // Graphic Initialization
            graphics = new GraphicsDeviceManager(this);
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredBackBufferHeight = Screenheight;
            graphics.PreferredBackBufferWidth = Screenwidth;
            graphics.IsFullScreen = false;
            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            Window.IsBorderless = true;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Button_Map = Content.Load<Texture2D>("UI\\Project Spritemap");

            UI_handler = new UI_Handler(Content);
            UI_handler.Initialize();

            Play_Button = new Button_Tex(Button_Map, new Rectangle(268, 0, 48, 48), new Vector2(100, 100));

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

            //Play_Button.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
