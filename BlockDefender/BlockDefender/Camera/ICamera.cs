using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace BlockDefender.Camera
{
    interface ICamera
    {
        Matrix computeTransformation();
    }
}
