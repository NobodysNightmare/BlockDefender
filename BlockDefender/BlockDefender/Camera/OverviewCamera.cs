using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using BlockDefender.Terrain;
using Microsoft.Xna.Framework.Graphics;

namespace BlockDefender.Camera
{
    class OverviewCamera : ICamera
    {
        private Matrix Transformation;

        public OverviewCamera(GraphicsDevice device, Map map)
        {
            float desiredFieldSize = (float)device.Viewport.Width / (map.ColumnCount + 1);
            float gameScale = desiredFieldSize / BlockDefenderGame.FieldSize;
            float emptyVerticalSpace = device.Viewport.Height - (desiredFieldSize * map.RowCount);
            Vector3 gameOffset = new Vector3(desiredFieldSize / 2, emptyVerticalSpace / 2, 0);

            Transformation = Matrix.CreateScale(gameScale);
            Transformation.Translation = gameOffset;
        }

        public Matrix computeTransformation()
        {
            return Transformation;
        }
    }
}
