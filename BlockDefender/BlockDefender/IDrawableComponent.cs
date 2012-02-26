using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace BlockDefender
{
    interface IDrawableComponent
    {
        void Draw(SpriteBatch spriteBatch);
    }
}
