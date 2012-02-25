using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace BlockDefender.Terrain
{
    class PlainField : Field
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

        public PlainField(int column, int row)
            : base(column, row)
        { }

        public PlainField(Field field)
            : base(field)
        { }

        public static void LoadAssets(ContentManager content)
        {
            Texture = content.Load<Texture2D>("plainfield");
            TextureScale = Field.ComputeScalingFactor(Texture);
        }
    }
}
