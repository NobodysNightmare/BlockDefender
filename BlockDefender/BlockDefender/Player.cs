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
    enum PlayerHeading
    {
        Down = 0,
        Right = 1,
        Up = 2,
        Left = 3
    }

    class Player
    {
        public static readonly float MovementSpeed = 5f;

        public Vector2 Position { get; private set; }
        public PlayerHeading Heading { get; private set; }
        public int Id { get; private set; }

        private Playground Playground;

        private float ScalingFactor;
        private Vector2 TextureCenter;

        private Vector2[] HeadingVectors;
        private Vector2 HeadingVector
        {
            get
            {
                return HeadingVectors[(int)Heading];
            }
        }

        private Texture2D[] TexturesByHeading;
        private Texture2D Texture
        {
            get
            {
                return TexturesByHeading[(int)Heading];
            }
        }

        public Player(int id, Playground playground, Vector2 spawnPosition)
        {
            Id = id;
            Playground = playground;
            Position = spawnPosition;
            TexturesByHeading = new Texture2D[4];
            InitializeHeadingVectors();
        }

        private void InitializeHeadingVectors()
        {
            HeadingVectors = new Vector2[4];
            HeadingVectors[(int)PlayerHeading.Up] = -Vector2.UnitY;
            HeadingVectors[(int)PlayerHeading.Down] = Vector2.UnitY;
            HeadingVectors[(int)PlayerHeading.Right] = Vector2.UnitX;
            HeadingVectors[(int)PlayerHeading.Left] = -Vector2.UnitX;
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
    }
}
