using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gymnasiearbete
{
    struct AI_Controlls
    {
        public static bool NoMemorySave = true;
    }

    struct CellManagerControlls
    {
        public static bool DeleteSectorIfEmpty = false;
    }

    struct StaticGlobal
    {
        static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        public float Clamp(float min, float max, float num)
        {
            return Math.Max(Math.Min(num, max), min);
        }
    }

    static class SGScreen
    {
        static Rectangle area;
        public static Rectangle Area
        {
            get { return area; }
            set { area = value; }
        }

        public static void Initialize(Rectangle ClientBounds)
        {
            area = ClientBounds;
        }

        public static void Resize(Rectangle ClientBounds)
        {
            area = ClientBounds;
        }
    }
    static class SGBasicEffect
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
