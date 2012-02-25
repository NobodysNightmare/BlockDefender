using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlockDefender.Terrain
{
    class MapGenerator
    {
        delegate Field SimpleFieldMapper(int column, int row, Field currentValue);

        private Map Map;
        private Random Random;

        public MapGenerator()
            : this(Environment.TickCount)
        { }

        public MapGenerator(int seed)
        {
            Random = new Random(seed);
        }

        public Map Generate(int mapWidth, int mapHeight)
        {
            Map = new Map(mapWidth, mapHeight);
            GenerateBaseTerrain();
            return Map;
        }

        private void GenerateBaseTerrain()
        {
            ReplaceAllFields(MapBaseTerrain);
        }

        private void ReplaceAllFields(SimpleFieldMapper mappingFunction)
        {
            for (int column = 0; column < Map.ColumnCount; column++)
                for (int row = 0; row < Map.RowCount; row++)
                    Map.Fields[column, row] = mappingFunction(column, row, Map.Fields[column, row]);
        }

        private Field MapBaseTerrain(int column, int row, Field currentValue)
        {
            double distanceToMid = Math.Abs(column - ((Map.ColumnCount - 1) / 2f));
            double relativeMidDistance = 2 * distanceToMid / Map.ColumnCount;
            double wallProbability = (0.5 + Math.Cos(relativeMidDistance * Math.PI) / 2);
            if (Random.NextDouble() < wallProbability)
                return new DestructibleField(column, row);

            return currentValue;
        }
    }
}
