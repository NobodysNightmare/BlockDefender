using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BlockDefender
{
    class Playground
    {
        public const int Width = 16;
        public const int Height = 9;

        private Field[,] Fields;

        public Playground()
        {
            Fields = new Field[Width, Height];
            PopulateFields();
        }

        private void PopulateFields()
        {
            var r = new Random();
            for (int column = 0; column < Width; column++)
            {
                for (int row = 0; row < Height; row++)
                {
                    switch (r.Next(2))
                    {
                        case 0:
                            Fields[column, row] = new SolidField(column, row);
                            break;
                        default:
                            Fields[column, row] = new PlainField(column, row);
                            break;
                    }
                }
            }
        }

        public void Load(ContentManager Content)
        {
            foreach (var field in Fields)
            {
                field.Load(Content);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var field in Fields)
            {
                field.Draw(spriteBatch);
            }
        }
    }
}
