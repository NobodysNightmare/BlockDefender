using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Fields
{
    class DestructibleField : Field
    {
        public DestructibleField(int column, int row)
            : base(column, row)
        {
            IsDestructible = true;
            IsAccessible = false;
        }

        public DestructibleField(Field field)
            : base(field)
        {
            IsDestructible = true;
            IsAccessible = false;
        }

        protected override Texture2D LoadTexture(ContentManager content)
        {
            return content.Load<Texture2D>("field2");
        }
    }
}
