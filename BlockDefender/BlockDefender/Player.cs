using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Fields;

namespace BlockDefender
{
    enum PlayerHeading : byte
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3
    }

    class Player
    {
        public static readonly float MovementSpeed = 5f;

        private static Texture2D[] TexturesByHeading;
        private static float ScalingFactor;
        private static Vector2 TextureCenter;
        private static Vector2[] HeadingVectors;

        public Vector2 Position { get; private set; }
        public PlayerHeading Heading { get; private set; }
        public int Id { get; private set; }

        private Playground Playground;

        private Vector2 HeadingVector
        {
            get
            {
                return HeadingVectors[(int)Heading];
            }
        }

        private Texture2D Texture
        {
            get
            {
                return TexturesByHeading[(int)Heading];
            }
        }

        static Player()
        {
            TexturesByHeading = new Texture2D[4];
            HeadingVectors = new Vector2[4];
            HeadingVectors[(int)PlayerHeading.Up] = -Vector2.UnitY;
            HeadingVectors[(int)PlayerHeading.Down] = Vector2.UnitY;
            HeadingVectors[(int)PlayerHeading.Right] = Vector2.UnitX;
            HeadingVectors[(int)PlayerHeading.Left] = -Vector2.UnitX;
        }

        public Player(int id, Playground playground, Vector2 spawnPosition)
        {
            Id = id;
            Playground = playground;
            Position = spawnPosition;
        }

        public static void LoadAssets(ContentManager content)
        {
            TexturesByHeading[(int)PlayerHeading.Up] = content.Load<Texture2D>("player-up");
            TexturesByHeading[(int)PlayerHeading.Down] = content.Load<Texture2D>("player-down");
            TexturesByHeading[(int)PlayerHeading.Right] = content.Load<Texture2D>("player-right");
            TexturesByHeading[(int)PlayerHeading.Left] = content.Load<Texture2D>("player-left");
            var texture = TexturesByHeading.First();
            ScalingFactor = BlockDefenderGame.FieldSize / (float)texture.Width;
            TextureCenter = new Vector2(texture.Width / 2, texture.Height / 2);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, TextureCenter, ScalingFactor, SpriteEffects.None, 1f);
        }

        public void Move(PlayerHeading movementDirection)
        {
            Heading = movementDirection;
            var nextPosition = Position + Vector2.Multiply(HeadingVector, MovementSpeed);
            var nextField = Playground.FieldAt(Field.CalculateColumn(nextPosition), Field.CalculateRow(nextPosition));
            if (nextField.IsAccessible)
                Position = nextPosition;
        }

        public void Interact()
        {
            var nextPosition = Position + Vector2.Multiply(HeadingVector, BlockDefenderGame.FieldSize);
            var nextField = Playground.FieldAt(Field.CalculateColumn(nextPosition), Field.CalculateRow(nextPosition));
            if (nextField.IsDestructible)
            {
                Playground.replaceFieldAt(Field.CalculateColumn(nextPosition), Field.CalculateRow(nextPosition), new PlainField(nextField));
            }
        }

        public void Update(Vector2 newPosition, PlayerHeading newHeading)
        {
            Position = newPosition;
            Heading = newHeading;
        }
    }
}
