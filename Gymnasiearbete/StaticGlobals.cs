using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Gymnasiearbete
{
    struct EnergyControlls
    {
        public static double FoodEnergy
        {
            get
            {
                return 100;
            }
        }
        public static double FoodSpawnAmount
        {
            get
            {
                return 2;
            }
        }
        public static double FoodSpawnTime
        {
            get
            {
                return (FoodEnergy*(FoodSpawnAmount/2) * 2)/60;
            }
        }
        public static double CellEnergyRequirement
        {
            get
            {
                return (FoodSpawnTime * 60 * 2)*3;
            }
        }
    }

    struct AIControlls
    {
        public static bool NoMemorySave = true;
    }

    struct CellManagerControlls
    {
        public static bool DeleteSectorIfEmpty = false;

        public static int DefaultCellSize = 10;
        public static int DefaultCellSpeed = 20;
        public static int DefaultCellPerception = 100;
        public static int[] DefaultCellDNA = new int[3] { DefaultCellSize, DefaultCellSpeed, DefaultCellPerception };
    }

    struct StaticGlobal
    {
        public const string ExperimentalFeatureMessage = @"Message:
This method or property is an experimental feature and has thus limited support.
Consider not using it as it is at high risk of getting removed.";

        static readonly Random random = new Random();
        public static Random Random
        {
            get { return random; }
        }


        public float Clamp(float min, float max, float num)
        {
            return Math.Max(Math.Min(num, max), min);
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
