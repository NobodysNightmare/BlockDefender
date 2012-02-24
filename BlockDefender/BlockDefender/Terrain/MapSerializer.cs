using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlockDefender.Terrain;
using System.IO;

namespace BlockDefender.Terrain
{
    enum FieldType : byte
    {
        None, Plain, Solid, Destructible
    }

    class MapReader
    {
        private BinaryReader Reader;

        public MapReader(BinaryReader reader)
        {
            Reader = reader;
        }

        public Map ReadMap()
        {
            int columnCount = Reader.ReadInt32();
            int rowCount = Reader.ReadInt32();
            var map = new Map(columnCount, rowCount);
            for (int column = 0; column < columnCount; column++)
            {
                for (int row = 0; row < rowCount; row++)
                {
                    map.Fields[column, row] = ReadField(column, row);
                }
            }
            return map;
        }

        private Field ReadField(int column, int row)
        {
            FieldType type = (FieldType)Reader.ReadByte();
            switch (type)
            {
                case FieldType.Plain:
                    return new PlainField(column, row);
                case FieldType.Solid:
                    return new SolidField(column, row);
                case FieldType.Destructible:
                    return new DestructibleField(column, row);
                default:
                    throw new UnsupportedDataException(string.Format("Field-type is unknown: {0}", type));
            }
        }
    }

    class MapWriter
    {
        private BinaryWriter Writer;

        public MapWriter(BinaryWriter writer)
        {
            Writer = writer;
        }

        public void WriteMap(Map map)
        {
            Writer.Write(map.ColumnCount);
            Writer.Write(map.RowCount);

            foreach (var field in map.Fields)
            {
                WriteField(field);
            }
        }

        private void WriteField(Field field)
        {
            if (field is PlainField)
                Writer.Write((byte)FieldType.Plain);
            else if (field is SolidField)
                Writer.Write((byte)FieldType.Solid);
            else if (field is DestructibleField)
                Writer.Write((byte)FieldType.Destructible);
        }
    }
}
