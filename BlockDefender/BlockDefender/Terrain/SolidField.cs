using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Terrain
{
    class SolidField : Field
    {
        public static readonly SolidField BorderField = new SolidField(-1, -1);

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

        public SolidField(int column, int row)
            : base(column, row)
        {
            IsAccessible = false;
        }

        public SolidField(Field field)
            : base(field)
        {
            IsAccessible = false;
        }

        public static void LoadAssets(ContentManager content)
        {
            Texture = content.Load<Texture2D>("field1");
            TextureScale = Field.ComputeScalingFactor(Texture);
        }
    }
}
