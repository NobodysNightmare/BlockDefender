using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Terrain
{
    class DestructibleField : Field
    {
        private static float TextureScale;
        protected override float ScalingFactor
        {
            get { return TextureScale; }
        }

        private static Texture2D Texture;
        protected override Texture2D FieldTexture
        {
            get { return Texture; }
        }

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

        public static void LoadAssets(ContentManager content)
        {
            Texture = content.Load<Texture2D>("field2");
            TextureScale = Field.ComputeScalingFactor(Texture);
        }
    }
}
