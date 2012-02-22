using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender
{
    class Field
    {
        private Texture2D Texture;

        //top-left corner in scene-coordinates
        private Vector2 Position;

        private float ScalingFactor;

        public Field(int column, int row)
        {
            Position = new Vector2(column * BlockDefenderGame.FieldSize, row * BlockDefenderGame.FieldSize);
            ScalingFactor = 1f;
        }

        public void Load(ContentManager content)
        {
            Texture = content.Load<Texture2D>("field1");
            ScalingFactor = BlockDefenderGame.FieldSize / (float)Texture.Width;
        }

        internal void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, null, Color.White, 0, Vector2.Zero, ScalingFactor, SpriteEffects.None, 0);
        }
    }
}
