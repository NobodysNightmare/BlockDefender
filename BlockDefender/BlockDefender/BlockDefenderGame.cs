using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;

namespace BlockDefender
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BlockDefenderGame : Microsoft.Xna.Framework.Game
    {
        public const int FieldSize = 100;
        public const int PlaygroundWidth = 10;

        private float GlobalScale;

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private SpriteFont SystemFont;

        private Field f1;
        private Field f2;

        public BlockDefenderGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            f1 = new Field(0, 0);
            f2 = new Field(9, 1);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            SystemFont = Content.Load<SpriteFont>("SystemFont");
            GlobalScale = ((float)graphics.GraphicsDevice.Viewport.Width / PlaygroundWidth) / FieldSize;
            f1.Load(Content);
            f2.Load(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Matrix.CreateScale(GlobalScale));
            drawFPS(gameTime);
            f1.Draw(spriteBatch);
            f2.Draw(spriteBatch);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        private void drawFPS(GameTime time)
        {
            StringBuilder myFPSText = new StringBuilder(Math.Round(1 / time.ElapsedGameTime.TotalSeconds).ToString()).Append(" FPS");
            spriteBatch.DrawString(SystemFont, myFPSText, Vector2.One, Color.White);
        }
    }
}
