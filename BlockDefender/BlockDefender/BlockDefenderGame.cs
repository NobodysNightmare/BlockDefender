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
using BlockDefender.Fields;
using BlockDefender.Networking;

namespace BlockDefender
{
    public class BlockDefenderGame : Microsoft.Xna.Framework.Game
    {
        public const int FieldSize = 100;

        private float GameScale;
        private Vector3 GameOffset;

        private GraphicsDeviceManager graphics;
        private SpriteBatch GameSprites;
        private SpriteBatch HUDSprites;

        private SpriteFont SystemFont;

        private Playground Playground;
        private Player PlayerOne;

        public BlockDefenderGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            GameServer server = new GameServer(new Map(16, 9));
            server.Start();
            var client = new NetworkClient();
            Playground = client.EstablishConnection();
            PlayerOne = Playground.SpawnNextPlayer();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameSprites = new SpriteBatch(GraphicsDevice);
            HUDSprites = new SpriteBatch(GraphicsDevice);
            SystemFont = Content.Load<SpriteFont>("SystemFont");
            Playground.Load(Content);
            PlayerOne.Load(Content);

            float desiredFieldSize = (float)graphics.GraphicsDevice.Viewport.Width / (Playground.ColumnCount + 1);
            GameScale = desiredFieldSize / FieldSize;
            float emptyVerticalSpace = graphics.GraphicsDevice.Viewport.Height - (desiredFieldSize * Playground.RowCount);
            GameOffset = new Vector3(desiredFieldSize / 2, emptyVerticalSpace / 2, 0);
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            UpdateInput();

            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(AppSettings.Default.Exit))
                Exit();

            if (keyState.IsKeyDown(AppSettings.Default.MoveUp))
            {
                PlayerOne.Move(PlayerHeading.Up);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveLeft))
            {
                PlayerOne.Move(PlayerHeading.Left);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveDown))
            {
                PlayerOne.Move(PlayerHeading.Down);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveRight))
            {
                PlayerOne.Move(PlayerHeading.Right);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            Matrix gameTransform = Matrix.CreateScale(GameScale);
            gameTransform.Translation = GameOffset;
            GameSprites.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, gameTransform);
            Playground.Draw(GameSprites);
            PlayerOne.Draw(GameSprites);
            GameSprites.End();

            HUDSprites.Begin();
            drawFPS(gameTime);
            HUDSprites.End();
            
            base.Draw(gameTime);
        }

        private void drawFPS(GameTime time)
        {
            StringBuilder myFPSText = new StringBuilder(Math.Round(1 / time.ElapsedGameTime.TotalSeconds).ToString()).Append(" FPS");
            HUDSprites.DrawString(SystemFont, myFPSText, Vector2.One, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
