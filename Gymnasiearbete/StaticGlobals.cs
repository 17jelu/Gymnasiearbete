using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gymnasiearbete
{
    struct StaticGlobal
    {
        public const string ExperimentalFeatureMessage = @"Message:
This method or property is an experimental feature and has thus limited support.
Consider not using it as it is at high risk of getting removed.";

        static Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }

        static CustomKeyboard keyboard = new CustomKeyboard();
        public static CustomKeyboard Keyboard
        {
            get { return keyboard; }
        }

        static CustomMouse mouse = new CustomMouse();
        public static CustomMouse Mouse
        {
            get { return mouse; }
        }
    }

    struct SGScreen
    {
        static Rectangle area;
        public static Rectangle Area
        {
            get { return area; }
            set { area = value; }
        }

        public static void Initialize(Rectangle ClientBounds)
        {
            area = new Rectangle();
            area.Width = ClientBounds.Width;
            area.Height = ClientBounds.Height;
        }

        public static void Resize(Rectangle ClientBounds)
        {
            area.Width = ClientBounds.Width;
            area.Height = ClientBounds.Height;
        }
    }

    struct SGBasicEffect
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
