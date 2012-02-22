using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Fields
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

        public Field(int column, int row)
        {
            Position = new Vector2(column * BlockDefenderGame.FieldSize, row * BlockDefenderGame.FieldSize);
            ScalingFactor = 1f;
        }

        public void Load(ContentManager content)
        {
            Texture = LoadTexture(content);
            ScalingFactor = BlockDefenderGame.FieldSize / (float)Texture.Width;
        }

        protected abstract Texture2D LoadTexture(ContentManager content);

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, ScalingFactor, SpriteEffects.None, 1f);
        }
    }
}
