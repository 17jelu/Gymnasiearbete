using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gymnasiearbete
{
    class SGBasicEffect
    {
        static BasicEffect effect;
        public static BasicEffect Effect
        {
            get { return effect; }
        }

        public static void Initialize(GraphicsDevice GraphicsDevice)
        {
            effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);
            effect.CurrentTechnique.Passes[0].Apply();
        }

        public static void ApplyCurrentTechnique()
        {
            effect.CurrentTechnique.Passes[0].Apply();
        }

        public static void Resize(GraphicsDevice GraphicsDevice)
        {
            effect.Projection = Matrix.CreateOrthographicOffCenter
                (0, GraphicsDevice.Viewport.Width,     // left, right
                GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);
        }
    }
}
