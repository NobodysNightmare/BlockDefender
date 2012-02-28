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
using BlockDefender.Layers;

namespace BlockDefender
{
    public class BlockDefenderGame : Microsoft.Xna.Framework.Game
    {
        public const int FieldSize = 100;

        private GraphicsDeviceManager Graphics;

        public static SpriteFont SystemFont { get; private set; }

        private Stack<ILayer> Layers;

        public BlockDefenderGame()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Layers = new Stack<ILayer>();
        }

        protected override void Initialize()
        {
            if (AppSettings.Default.EnableServer)
            {
                GameServer server = new GameServer(new MapGenerator().Generate(30, 16), AppSettings.Default.ListenPort);
                server.Start();
            }
            ApplyGraphicsSettings();

            base.Initialize();
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
            SystemFont = Content.Load<SpriteFont>("SystemFont");
            Player.LoadAssets(Content);
            PlainField.LoadAssets(Content);
            SolidField.LoadAssets(Content);
            DestructibleField.LoadAssets(Content);

            Layers.Push(new GameLayer(GraphicsDevice));
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            foreach (var layer in Layers)
            {
                layer.Update(gameTime);
            }

            UpdateInput();
            base.Update(gameTime);
        }

        private void UpdateInput()
        {
            var keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(AppSettings.Default.Exit))
                Exit();

            Layers.Peek().HandleInput();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            foreach (var layer in Layers)
            {
                layer.Draw(gameTime);
            }
            
            base.Draw(gameTime);
        }
    }
}
