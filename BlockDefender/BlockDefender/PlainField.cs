using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender
{
    class PlainField : Field
    {
        public PlainField(int column, int row)
            : base(column, row)
        { }

        protected override Texture2D LoadTexture(ContentManager content)
        {
            return content.Load<Texture2D>("field3");
        }
    }
}
