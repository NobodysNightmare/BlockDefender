using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BlockDefender
{
    enum PlayerHeading
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3
    }

    class Player
    {
        public Vector2 Position { get; private set; }
        public PlayerHeading Heading { get; private set; }

        private float ScalingFactor;
        private Vector2 TextureCenter;

        private Texture2D[] TexturesByHeading;
        private Texture2D Texture
        {
            get
            {
                return TexturesByHeading[(int)Heading];
            }
        }

        public Player(Vector2 spawnPosition)
        {
            Position = spawnPosition;
            TexturesByHeading = new Texture2D[4];
        }

        public void Load(ContentManager content)
        {
            TexturesByHeading[(int)PlayerHeading.Up] = content.Load<Texture2D>("player-up");
            TexturesByHeading[(int)PlayerHeading.Down] = content.Load<Texture2D>("player-down");
            TexturesByHeading[(int)PlayerHeading.Right] = content.Load<Texture2D>("player-right");
            TexturesByHeading[(int)PlayerHeading.Left] = content.Load<Texture2D>("player-left");
            ScalingFactor = BlockDefenderGame.FieldSize / (float)Texture.Width;
            TextureCenter = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, TextureCenter, ScalingFactor, SpriteEffects.None, 1f);
        }
    }
}
