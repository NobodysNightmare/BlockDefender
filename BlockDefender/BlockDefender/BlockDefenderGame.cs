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
using BlockDefender.Terrain;
using BlockDefender.Net;

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
        private NetworkClient NetworkClient;

        public BlockDefenderGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            if (AppSettings.Default.EnableServer)
            {
                GameServer server = new GameServer(new Map(16, 9), AppSettings.Default.ListenPort);
                server.Start();
            }
            NetworkClient = new NetworkClient();
            Playground = NetworkClient.EstablishConnection();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            GameSprites = new SpriteBatch(GraphicsDevice);
            HUDSprites = new SpriteBatch(GraphicsDevice);
            SystemFont = Content.Load<SpriteFont>("SystemFont");
            Playground.Load(Content);
            Player.LoadAssets(Content);

            float desiredFieldSize = (float)graphics.GraphicsDevice.Viewport.Width / (Playground.ColumnCount + 1);
            GameScale = desiredFieldSize / FieldSize;
            float emptyVerticalSpace = graphics.GraphicsDevice.Viewport.Height - (desiredFieldSize * Playground.RowCount);
            GameOffset = new Vector3(desiredFieldSize / 2, emptyVerticalSpace / 2, 0);
        }

        protected override void UnloadContent()
        {
            NetworkClient.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            NetworkClient.Update();
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
                NetworkClient.MovePlayer(PlayerHeading.Up);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveLeft))
            {
                NetworkClient.MovePlayer(PlayerHeading.Left);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveDown))
            {
                NetworkClient.MovePlayer(PlayerHeading.Down);
            }
            else if (keyState.IsKeyDown(AppSettings.Default.MoveRight))
            {
                NetworkClient.MovePlayer(PlayerHeading.Right);
            }

            if (keyState.IsKeyDown(AppSettings.Default.Interact))
            {
                NetworkClient.PlayerInteract();
            }

            for (int i = 0; i < Playground.ColumnCount; i++)
            {
                for (int j = 0; j < Playground.RowCount; j++)
                {
                    Playground.FieldAt(i, j).Load(Content);
                }
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
