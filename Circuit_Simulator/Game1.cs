using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Circuit_Simulator
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        #region INPUT

        KeyboardState kb_state, kb_state_old;
        MouseState mo_state, mo_state_old;

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


        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            kb_state = Keyboard.GetState();
            mo_state = Mouse.GetState();



            kb_state_old = kb_state;
            mo_state_old = mo_state;
            base.Update(gameTime);
        }

        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            

            base.Draw(gameTime);
        }
    }
}
