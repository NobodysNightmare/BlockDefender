using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using BlockDefender.Fields;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace BlockDefender
{
    class Playground
    {
        private Map Map;

        private List<Player> PlayerList;

        public IEnumerable<Player> Players
        {
            get
            {
                return new ReadOnlyCollection<Player>(PlayerList);
            }
        }

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
            PlayerList = new List<Player>();
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

            foreach (var player in PlayerList)
            {
                player.Draw(spriteBatch);
            }
        }

        public Player SpawnNextPlayer()
        {
           return SpawnPlayerAt(Map.Fields[1, 1].Center);
        }

        public Player SpawnPlayerAt(Vector2 position)
        {
            var p = new Player(this, position);
            AddPlayer(p);
            return p;
        }

        public void AddPlayer(Player p)
        {
            PlayerList.Add(p);
        }

        public Field FieldAt(int column, int row)
        {
            return Map.FieldAt(column, row);
        }

        public void replaceFieldAt(int column, int row, Field field)
        {
            Map.Fields[column, row] = field;
        }
    }
}
