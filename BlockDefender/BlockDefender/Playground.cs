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
        private Map Map;

        public int ColumnCount
        {
            get
            {
                return Map.ColumnCount;
            }
        }

        public int RowCount
        {
            get
            {
                return Map.RowCount;
            }
        }

        public Playground(Map map)
        {
            Map = map;
        }

        public void Load(ContentManager Content)
        {
            foreach (var field in Map.Fields)
            {
                field.Load(Content);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var field in Map.Fields)
            {
                field.Draw(spriteBatch);
            }
        }

        public Player SpawnNextPlayer()
        {
            return new Player(this, Map.Fields[1, 1].Center);
        }

        public Field FieldAt(int column, int row)
        {
            return Map.FieldAt(column, row);
        }
    }
}
