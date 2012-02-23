using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Fields;
using Microsoft.Xna.Framework;

namespace BlockDefender
{
    class Playground
    {
        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }

        private Field[,] Fields;

        public Playground(int columnCount, int rowCount)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;

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

        public Player SpawnNextPlayer()
        {
            return new Player(this, Fields[1, 1].Center);
        }

        public Field FieldAt(int column, int row)
        {
            if (column < 0 || row < 0 || column >= ColumnCount || row >= RowCount)
                return SolidField.BorderField;

            return Fields[column, row];
        }
    }
}
