using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Net;
using Microsoft.Xna.Framework.Input;
using BlockDefender.Camera;

namespace BlockDefender.Layers
{
    class GameLayer : ILayer
    {
        private GraphicsDevice GraphicsDevice;
        private SpriteBatch GameSprites;
        private SpriteBatch HUDSprites;
        
        private ICamera ActiveCamera;
        private ICamera OverviewCamera;
        private ICamera ChaseCamera;

        private NetworkClient NetworkClient;

        private KeyboardState LastKeyState;

        public GameLayer(GraphicsDevice device)
        {
            GraphicsDevice = device;
            GameSprites = new SpriteBatch(device);
            HUDSprites = new SpriteBatch(device);
            ActiveCamera = new SimpleCamera();

            NetworkClient = new NetworkClient();
            NetworkClient.MapChanged += OnMapChange;
            NetworkClient.PlayerAssigned += OnPlayerAssigned;
            NetworkClient.EstablishConnection(AppSettings.Default.ConnectHost, AppSettings.Default.ConnectPort);
        }

        void OnMapChange(object source, MapChangedEventArgs e)
        {
            OverviewCamera = new OverviewCamera(GraphicsDevice, e.Map);
            ActiveCamera = OverviewCamera;
        }

        void OnPlayerAssigned(object source, PlayerAssignedEventArgs e)
        {
            ChaseCamera = new PlayerChasingCamera(GraphicsDevice, e.Player);
            ActiveCamera = ChaseCamera;
        }

        public void Update(GameTime gameTime)
        {
            NetworkClient.Update();
        }

        public void HandleInput()
        {
            var keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(AppSettings.Default.OverviewCamera) && LastKeyState.IsKeyUp(AppSettings.Default.OverviewCamera))
            {
                ActiveCamera = OverviewCamera;
            }
            else if (keyState.IsKeyUp(AppSettings.Default.OverviewCamera) && LastKeyState.IsKeyDown(AppSettings.Default.OverviewCamera))
            {
                ActiveCamera = ChaseCamera;
            }

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

            LastKeyState = keyState;
        }

        public void Draw(GameTime gameTime)
        {
            GameSprites.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap,
                              DepthStencilState.Default, RasterizerState.CullNone, null, ActiveCamera.computeTransformation());
            if (NetworkClient.Visual != null)
            {
                NetworkClient.Visual.Draw(GameSprites);
            }
            GameSprites.End();

            HUDSprites.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
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
