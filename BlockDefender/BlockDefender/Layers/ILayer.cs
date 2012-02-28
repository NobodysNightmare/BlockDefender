using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockDefender.Layers
{
    interface ILayer
    {
        void Update(GameTime gameTime);
        void HandleInput();
        void Draw(GameTime gameTime);
    }
}
