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

        private Matrix GameTransform;

        private GraphicsDeviceManager Graphics;
        private SpriteBatch GameSprites;
        private SpriteBatch HUDSprites;

        private SpriteFont SystemFont;

        private NetworkClient NetworkClient;

        public BlockDefenderGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            if (AppSettings.Default.EnableServer)
            {
                GameServer server = new GameServer(new MapGenerator().Generate(30, 16), AppSettings.Default.ListenPort);
                server.Start();
            }
            ApplyGraphicsSettings();
            GameTransform = Matrix.CreateScale(1f);
            NetworkClient = new NetworkClient();
            NetworkClient.MapChanged += OnMapChange;
            NetworkClient.EstablishConnection(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);

            base.Initialize();
        }

        void OnMapChange(object source, MapChangedEventArgs e)
        {
            float desiredFieldSize = (float)Graphics.GraphicsDevice.Viewport.Width / (e.Map.ColumnCount + 1);
            float gameScale = desiredFieldSize / FieldSize;
            float emptyVerticalSpace = Graphics.GraphicsDevice.Viewport.Height - (desiredFieldSize * e.Map.RowCount);
            Vector3 gameOffset = new Vector3(desiredFieldSize / 2, emptyVerticalSpace / 2, 0);

            GameTransform = Matrix.CreateScale(gameScale);
            GameTransform.Translation = gameOffset;
        }

        private void ApplyGraphicsSettings()
        {
            Graphics.PreferredBackBufferWidth = AppSettings.Default.ScreenWidth;
            Graphics.PreferredBackBufferHeight = AppSettings.Default.ScreenHeight;
            if (AppSettings.Default.Fullscreen)
                Graphics.ToggleFullScreen();

            Graphics.ApplyChanges();
        }

        protected override void LoadContent()
        {
            GameSprites = new SpriteBatch(GraphicsDevice);
            HUDSprites = new SpriteBatch(GraphicsDevice);
            SystemFont = Content.Load<SpriteFont>("SystemFont");
            Player.LoadAssets(Content);
            PlainField.LoadAssets(Content);
            SolidField.LoadAssets(Content);
            DestructibleField.LoadAssets(Content);
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
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GameSprites.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, GameTransform);
            if (NetworkClient.Visual != null)
            {
                NetworkClient.Visual.Draw(GameSprites);
            }
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
