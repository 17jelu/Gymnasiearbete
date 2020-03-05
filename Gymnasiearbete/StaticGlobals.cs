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

        static readonly CellManager cm = new CellManager();
        public static CellManager CM
        {
            get
            {
                return cm;
            }
        }

        public static float Clamp(float num, float min, float max)
        {
            return Math.Max(Math.Min(num, max), min);
        }

        static bool save = false;
        /// <summary>
        /// Determines wether the game saves stuff while Shutdown() is called
        /// </summary>
        public static bool SaveModeON
        {
            get { return save; }
            set { save = value; }
        }

        static bool exit = false;
        public static bool ExitInProgress => exit;
        /// <summary>
        /// Exits the game the correct way
        /// </summary>
        public static void Shutdown()
        {
            exit = true;
        }

        public static bool IsCell(GameObject g)
        {
            if (g.GetType() == typeof(Cell))
            {
                return true;
            }
            return false;
        }

        public static bool IsFood(GameObject g)
        {
            if (g.GetType() == typeof(Food))
            {
                return true;
            }
            return false;
        }

        static Random random = new Random();
        public static Random Random => random;

        static CustomKeyboard keyboard = new CustomKeyboard();
        public static CustomKeyboard Keyboard => keyboard;

        static CustomMouse mouse = new CustomMouse();
        public static CustomMouse Mouse => mouse;

        #region Family
        static sgFamily family = new sgFamily();
        public static sgFamily Family => family;
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
            #endregion NameList

            private string GenerateRandomName()
            {
                /*
                string name = string.Empty;
                name += nl_color[random.Next(nl_color.Length)];
                name += " ";
                name += nl_adjective[random.Next(nl_adjective.Length)];
                name += " ";
                name += nl_noun[random.Next(nl_noun.Length)];

                return name;
                //*/
                string name = nl_color[random.Next(nl_color.Length)];
                return name;
            }

            /*
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
            */

            public string NewFamily()
            {

                string name = GenerateRandomName();

                if (!families.ContainsKey(name))
                {
                    families.Add(name, 1);
                }

                return name;
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

            public int FamilyCount(string family)
                => families.ContainsKey(family) ? families[family] : 0;
        }
        #endregion Family

        #region Screen
        private static sgScreen screen = new sgScreen();
        public static sgScreen Screen => screen;

        internal class sgScreen
        {
            private Point location;
            public Point Location
            {
                get { return location; }
                set { location = value; }
            }

            private Rectangle area;
            public Rectangle Area
            {
                get { return area; }
                set { area = value; }
            }

            private Rectangle beforeFullscreen;

            public void Initialize(Rectangle ClientBounds)
            {
                beforeFullscreen = new Rectangle();
                area = new Rectangle();
                area.Width = ClientBounds.Width;
                area.Height = ClientBounds.Height;
                location = ClientBounds.Location;
            }

            /// <summary>
            /// Sets the Screen area to current window dimensions
            /// </summary>
            /// <param name="ClientBounds">The Game Windows bounds</param>
            public void Resize(Rectangle ClientBounds)
            {
                area.Width = ClientBounds.Width;
                area.Height = ClientBounds.Height;
                location = ClientBounds.Location;
            }

            /// <summary>
            /// Switches the game between Fullscreen mode and window mode & saving the current the size of the window before entering fullscreen
            /// </summary>
            /// <param name="graphics"></param>
            /// <param name="graphicsDevice"></param>
            public void ToggleFullScreen(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice)
            {
                if (!graphics.IsFullScreen)
                {
                    beforeFullscreen = new Rectangle
                    {
                        Width = area.Width,
                        Height = area.Height
                    };

                    graphics.PreferredBackBufferWidth = graphicsDevice.DisplayMode.Width;
                    graphics.PreferredBackBufferHeight = graphicsDevice.DisplayMode.Height;
                    graphics.IsFullScreen = true;
                    graphics.ApplyChanges();
                }
                else
                {
                    graphics.PreferredBackBufferWidth = beforeFullscreen.Width;
                    graphics.PreferredBackBufferHeight = beforeFullscreen.Height;
                    graphics.IsFullScreen = false;
                    graphics.ApplyChanges();
                }
            }
        }
        #endregion Screen

        #region BasicEffect
        private static sgBasicEffect effect = new sgBasicEffect();
        public static sgBasicEffect BasicEffect => effect;
        internal class sgBasicEffect
        {
            BasicEffect effect;
            // public BasicEffect Object => effect;

            public void Initialize(GraphicsDevice GraphicsDevice)
            {
                effect = new BasicEffect(GraphicsDevice);
                effect.VertexColorEnabled = true;
                effect.Projection = Matrix.CreateOrthographicOffCenter
                    (0, GraphicsDevice.Viewport.Width,     // left, right
                    GraphicsDevice.Viewport.Height, 0,    // bottom, top
                    0, 1);
                effect.CurrentTechnique.Passes[0].Apply();
            }

            public void ApplyCurrentTechnique()
            {
                effect.CurrentTechnique.Passes[0].Apply();
            }

            public void Resize(GraphicsDevice GraphicsDevice)
            {
                effect.Projection = Matrix.CreateOrthographicOffCenter
                    (0, GraphicsDevice.Viewport.Width,     // left, right
                    GraphicsDevice.Viewport.Height, 0,    // bottom, top
                    0, 1);
            }
        }
        #endregion BasicEffect
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
        public static bool NoMemorySave = false;
    }

    struct CellManagerControlls
    {
        public static bool DeleteSectorIfEmpty = false;

        public static int starterCellsNum = 5;
        public static int DefaultCellSize = 10;
        public static int DefaultCellSpeed = 20;
        public static int DefaultCellPerception = 100;
        public static int[] DefaultCellDNA = new int[3] { DefaultCellSize, DefaultCellSpeed, DefaultCellPerception };
    }
}
