using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Fields
{
    class SolidField : Field
    {
        public static readonly SolidField BorderField = new SolidField(-1, -1);

        public SolidField(int column, int row)
            : base(column, row)
        {
            IsAccessible = false;
        }

        protected override Texture2D LoadTexture(ContentManager content)
        {
            return content.Load<Texture2D>("field1");
        }
    }
}
