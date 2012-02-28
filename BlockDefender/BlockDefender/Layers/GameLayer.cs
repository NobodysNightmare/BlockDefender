using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Net;
using Microsoft.Xna.Framework.Input;

namespace BlockDefender.Layers
{
    class GameLayer : ILayer
    {
        private GraphicsDevice GraphicsDevice;
        private SpriteBatch GameSprites;
        private SpriteBatch HUDSprites;
        private Matrix GameTransform;

        private NetworkClient NetworkClient;

        public GameLayer(GraphicsDevice device)
        {
            GraphicsDevice = device;
            GameSprites = new SpriteBatch(device);
            HUDSprites = new SpriteBatch(device);
            GameTransform = Matrix.CreateScale(1f);

            NetworkClient = new NetworkClient();
            NetworkClient.MapChanged += OnMapChange;
            NetworkClient.EstablishConnection(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);
        }

        void OnMapChange(object source, MapChangedEventArgs e)
        {
            float desiredFieldSize = (float)GraphicsDevice.Viewport.Width / (e.Map.ColumnCount + 1);
            float gameScale = desiredFieldSize / BlockDefenderGame.FieldSize;
            float emptyVerticalSpace = GraphicsDevice.Viewport.Height - (desiredFieldSize * e.Map.RowCount);
            Vector3 gameOffset = new Vector3(desiredFieldSize / 2, emptyVerticalSpace / 2, 0);

            GameTransform = Matrix.CreateScale(gameScale);
            GameTransform.Translation = gameOffset;
        }

        public void Update(GameTime gameTime)
        {
            NetworkClient.Update();
        }

        public void HandleInput()
        {
            var keyState = Keyboard.GetState();

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

        public void Draw(GameTime gameTime)
        {
            GameSprites.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
                              DepthStencilState.Default, RasterizerState.CullNone, null, GameTransform);
            if (NetworkClient.Visual != null)
            {
                NetworkClient.Visual.Draw(GameSprites);
            }
            GameSprites.End();

            HUDSprites.Begin();
            drawHUD(gameTime);
            HUDSprites.End();
        }

        private void drawHUD(GameTime time)
        {
            StringBuilder myFPSText = new StringBuilder(Math.Round(1 / time.ElapsedGameTime.TotalSeconds).ToString()).Append(" FPS");
            HUDSprites.DrawString(BlockDefenderGame.SystemFont, myFPSText, Vector2.One, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            HUDSprites.DrawString(BlockDefenderGame.SystemFont, NetworkClient.State.ToString(), new Vector2(150f, 1f), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}
