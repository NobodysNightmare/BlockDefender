using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockDefender.Fields;

namespace BlockDefender
{
    class Map
    {
        public int ColumnCount { get; private set; }
        public int RowCount { get; private set; }

        public Field[,] Fields { get; private set; }

        public Map(int columnCount, int rowCount)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;

            Fields = new Field[ColumnCount, RowCount];
            InitializeFields();
        }

        private void InitializeFields()
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

        public Field FieldAt(int column, int row)
        {
            if (column < 0 || row < 0 || column >= ColumnCount || row >= RowCount)
                return SolidField.BorderField;

            return Fields[column, row];
        }

        public void SetFieldAt(int logicalId, Field newValue)
        {
            Fields[logicalId % ColumnCount, logicalId / ColumnCount] = newValue;
        }
    }
}
