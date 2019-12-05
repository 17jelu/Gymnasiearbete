using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Gymnasiearbete
{
    struct StaticGlobal
    {
        public const string ExperimentalFeatureMessage = @"Message:
This method or property is an experimental feature and has thus limited support.
Consider not using it as it is at high risk of getting removed.";

        public static string Lore
        {
            get
            {
                return @"As the story goes;
War. War, always war. The year 2151. 
The nukes erased life as we know it. 
Left are only somewhat mindless blobs of irradiated mass, 
ruled by their primal instinct of, fight, flight, eat or get eaten.
This is survival. This is war.";
            }
        }

        public static int SectorSize
        {
            get
            {
                return 100;
            }
        }

        static Random random = new Random();
        public static Random Random
        {
            get
            {
                return random;
            }
            set
            {
                random = value;
            }
        }

        static CellManager cm;
        public static CellManager CM
        {
            get
            {
                return cm;
            }
            set
            {
                cm = value;
            }
        }

        public float Clamp(float min, float max, float num)
        {
            return Math.Max(Math.Min(num, max), min);
        }

        private static CustomKeyboard keyboard = new CustomKeyboard();
        public static CustomKeyboard Keyboard
        {
            get { return keyboard; }
            set { keyboard = value; }
        }

        static CustomMouse mouse = new CustomMouse();
        public static CustomMouse Mouse
        {
            get { return mouse; }
            set { mouse = value; }
        }

        static sgFamily family = new sgFamily();
        public static sgFamily Family
        {
            get { return family; }
        }
        internal class sgFamily
        {
            Dictionary<string, int> families = new Dictionary<string, int>();
            #region NameList
            string[] nl_color = new string[]
            {
                    "Red",
                    "Green",
                    "Blue",
                    "Cyan",
                    "Magenta",
                    "Yellow",
                    "Black",
                    "White"
            };
            string[] nl_adjective = new string[]
            {
                    "Superior",
                    "Inferior",
                    "Innocent",
                    "Physical",
                    "Yummy",
                    "Distinct",
                    "Hissing",
                    "Sad",
                    "Glorious",
                    "Guarded",
                    "Wild",
                    "Spotless",
                    "Boring",
                    "Imperfect",
                    "Full",
                    "Reflective",
                    "Smart",
                    "Silky",
                    "Flat",
                    "Icy",
                    "Skillfull"
            };
            string[] nl_noun = new string[]
            {
                    "Gangster",
                    "Dragon",
                    "Dolphin",
                    "Ground",
                    "Elbow",
                    "Tray",
                    "Gate",
                    "Crow",
                    "Zinc",
                    "Blade",
                    "Sisters",
                    "Ocean",
                    "Library",
                    "Disease",
                    "Twig",
                    "Bomb",
                    "Nest",
                    "Wind",
                    "Downtown",
                    "Apparatus",
                    "Top",
                    "Magic",
                    "Eggs",
                    "Nest",
                    "Bears",
                    "Coat"
            };
            #endregion

            private string GenerateRandomName()
            {
                string name = string.Empty;
                name += nl_color[random.Next(nl_color.Length)];
                name += " ";
                name += nl_adjective[random.Next(nl_adjective.Length)];
                name += " ";
                name += nl_noun[random.Next(nl_noun.Length)];

                return name;
            }

            public string NewFamily
            {
                get
                {
                    string name = string.Empty;
                    do
                    {
                        name = GenerateRandomName();
                    }
                    while (families.ContainsKey(name));

                    families.Add(name, 1);

                    return name;
                }
            }

            public void KillMember(string family)
            {
                if (families.ContainsKey(family))
                {
                    families[family] -= 1;
                }
            }

            public void AddMember(string family)
            {
                if (families.ContainsKey(family))
                {
                    families[family] += 1;
                }
            }
        }
    }

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
                return (FoodEnergy * (FoodSpawnAmount / 2) * 2) / 60;
            }
        }
        public static double CellEnergyRequirement
        {
            get
            {
                return (FoodSpawnTime * 60 * 2) * 3;
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
