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

        private List<Player> Players;

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
            Players = new List<Player>();
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

            foreach (var player in Players)
            {
                player.Draw(spriteBatch);
            }
        }

        public Player SpawnNextPlayer()
        {
            var p = new Player(this, Map.Fields[1, 1].Center);
            AddPlayer(p);
            return p;
        }

        public void AddPlayer(Player p)
        {
            Players.Add(p);
        }

        public Field FieldAt(int column, int row)
        {
            return Map.FieldAt(column, row);
        }
    }
}
