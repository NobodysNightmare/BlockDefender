using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Fields;

namespace BlockDefender
{
    class Playground
    {
        public const int ColumnCount = 16;
        public const int RowCount = 9;

        private Field[,] Fields;

        public Playground()
        {
            Fields = new Field[ColumnCount, RowCount];
            PopulateFields();
        }

        private void PopulateFields()
        {
            for (int column = 0; column < ColumnCount; column++)
            {
                for (int row = 0; row < RowCount; row++)
                {
                    if((column % 4 != 2) || (row % 3 != 2))
                        Fields[column, row] = new PlainField(column, row);
                    else
                        Fields[column, row] = new SolidField(column, row);
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
