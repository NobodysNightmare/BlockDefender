using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Terrain
{
    abstract class Field
    {
        private static readonly Vector2 CenterOffset = new Vector2(BlockDefenderGame.FieldSize / 2, BlockDefenderGame.FieldSize / 2);
        private Texture2D Texture;

        //top-left corner in scene-coordinates
        private Vector2 Position;

        private float ScalingFactor;

        public Vector2 Center
        {
            get
            {
                return Position + CenterOffset;
            }
        }

        public bool IsAccessible { get; protected set; }
        public bool IsDestructible { get; protected set; }

        public Field(int column, int row)
        {
            Position = new Vector2(column * BlockDefenderGame.FieldSize, row * BlockDefenderGame.FieldSize);
            ScalingFactor = 1f;
            IsAccessible = true;
            IsDestructible = false;
        }

        public Field(Field field)
        {
            this.Position = field.Position;
            IsAccessible = true;
            IsDestructible = false;
        }

        public void Load(ContentManager content)
        {
            Texture = LoadTexture(content);
            ScalingFactor = BlockDefenderGame.FieldSize / (float)Texture.Width;
        }

        protected abstract Texture2D LoadTexture(ContentManager content);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, ScalingFactor, SpriteEffects.None, 1f);
        }

        public static int CalculateColumn(Vector2 position)
        {
            return (int)Math.Floor(position.X / BlockDefenderGame.FieldSize);
        }

        public static int CalculateRow(Vector2 position)
        {
            return (int)Math.Floor(position.Y / BlockDefenderGame.FieldSize);
        }
    }
}
