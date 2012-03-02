using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BlockDefender.Camera
{
    class PlayerChasingCamera : ICamera
    {
        private static float ZoomFactor = 0.7f;

        private Player Player;

        private Matrix Transformation;
        private Vector3 CenterTranslation;

        public PlayerChasingCamera(GraphicsDevice device, Player player)
        {
            CenterTranslation = new Vector3(device.Viewport.Width / 2, device.Viewport.Height / 2, 0f);
            Player = player;
            Transformation = Matrix.CreateScale(ZoomFactor);
            Transformation.Translation = calculateTargetTranslation();
        }

        public Matrix computeTransformation()
        {
            Vector3 targetTranslation = calculateTargetTranslation();
            Vector3 translationDifference = targetTranslation - Transformation.Translation;
            Transformation.Translation += translationDifference * 0.05f;
            return Transformation;
        }

        private Vector3 calculateTargetTranslation()
        {
            return Vector3.Multiply(new Vector3(-Player.Position, 0f), ZoomFactor) + CenterTranslation;
        }
    }
}
