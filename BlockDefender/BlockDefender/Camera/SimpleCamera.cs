using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockDefender.Camera
{
    class SimpleCamera : ICamera
    {
        public SimpleCamera()
        {

        }

        public Matrix computeTransformation()
        {
            return Matrix.Identity;
        }
    }
}
