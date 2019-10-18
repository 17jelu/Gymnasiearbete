using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gymnasiearbete
{
    /// <summary>
    /// Kontrollerar Alla Celler
    /// </summary>
    class CellManager
    {
        public double civilazationTime = 0;
        List<GameObject> objects = new List<GameObject>();

        int spawnTimer = 0;

        public bool pause = false;
        public bool simulationEnd = false;

        public int SectorSize {
            get
            {
                return 100;
            }
        }

        Dictionary<string, List<GameObject>> sectors = new Dictionary<string, List<GameObject>>();

        public void AddObjects(GameObject[] gs)
        {
            objects.AddRange(gs);
        }

        public string DebugSector(float Xposition, float Yposition)
        {
            string output = "";
            int x = (int)Math.Floor(Xposition / SectorSize);
            int y = (int)Math.Floor(Yposition / SectorSize);

            if (!sectors.ContainsKey(new Vector2(x, y).ToString()))
            {
                sectors.Add(new Vector2(x, y).ToString(), new List<GameObject>());
            }

            foreach (GameObject g in sectors[new Vector2(x, y).ToString()])
            {
                if (g.GetType() == typeof(Cell))
                {
                    Cell c = (Cell)g;
                    
                    output += "---" + objects.IndexOf(g) + "| SZ:" + c.size + "| SP:" + c.speed + "| PC:" + c.perception + "| E:" + c.energy + "\n";
                }
            }

            return output;
        }

        public void Update(GameWindow window, Random random, GameTime gameTime)
        {
            if (pause || simulationEnd)
            {
                return;
            }

            civilazationTime += gameTime.ElapsedGameTime.TotalSeconds;

            int foodCount = 0;
            int cellCount = 0;

            //Clearar sectorer
            foreach (KeyValuePair<string, List<GameObject>> s in sectors)
            {
                s.Value.Clear();
            }

            //Ofixerad Updatering
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject g = objects[i];

                if (g.isMarkedForDelete)
                {
                    objects.Remove(g);
                }
                else
                {
                    int px = (int)Math.Floor(g.position.X / SectorSize);
                    int py = (int)Math.Floor(g.position.Y / SectorSize);

                    if (!sectors.ContainsKey(new Vector2(px, py).ToString()))
                    {
                        sectors.Add(new Vector2(px, py).ToString(), new List<GameObject>());
                    }
                    sectors[new Vector2(px, py).ToString()].Add(g);

                    if (g.GetType() == typeof(Cell))
                    {
                        Cell c = (Cell)g;

                        if (c.isMarkForReproduce)
                        {
                            this.AddObjects(new GameObject[1] { c.Reproduce(window, random) });
                        }
                    }
                }
            }

            //FixeradUpdatering
            foreach (GameObject g in objects)
            {
                if (g.GetType() == typeof(Cell))
                {
                    cellCount++;
                    Cell c = (Cell)g;
                    List<GameObject> detectionCheck = new List<GameObject>();

                    int xMax = (int)Math.Floor((c.position.X + c.Detectionrange) / SectorSize);
                    int xMin = (int)Math.Floor((c.position.X - c.Detectionrange) / SectorSize);
                    int yMax = (int)Math.Floor((c.position.Y + c.Detectionrange) / SectorSize);
                    int yMin = (int)Math.Floor((c.position.Y - c.Detectionrange) / SectorSize);

                    for (int x = xMin; x <= xMax; x++)
                    {
                        for (int y = yMin; y <= yMax; y++)
                        {
                            if (!sectors.ContainsKey(new Vector2(x, y).ToString()))
                            {
                                sectors.Add(new Vector2(x, y).ToString(), new List<GameObject>());
                            }
                            detectionCheck.AddRange(sectors[new Vector2(x, y).ToString()]);
                            
                        }
                    }

                    c.Update(detectionCheck, window, random);
                }
            }

            foodCount = objects.Count - cellCount;

            //MatSpawn
            spawnTimer++;
            if (spawnTimer > 60)
            {
                spawnTimer = 0;
                AddObjects(
                    new GameObject[1]
                    {
                        new Food
                        (
                            new Vector2
                            (
                                random.Next(0, window.ClientBounds.Width),
                                random.Next(0, window.ClientBounds.Height)
                            )
                        )
                    }
                    );
            }

            if (cellCount < 0)
            {
                simulationEnd = true;
            }
        }

        // bluegh
        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            foreach (GameObject gameObj in objects)
            {
                if (gameObj.GetType() == typeof(Cell))
                {
                    ((Cell)gameObj).Draw(graphicsDevice, camera);
                }
                if (gameObj.GetType() == typeof(Food))
                {
                    ((Food)gameObj).Draw(graphicsDevice, camera);
                }
            }

            /*
            foreach (GameObject g in objects)
            {
                Color clr = Color.White;
                Circle.UnitCircle uc = Circle.UnitCircle.Point8;

                if (g.GetType() == typeof(Cell))
                {
                    Cell c = (Cell)g;

                    clr = new Color((int) c.size * 10, (int) c.speed * 10, (int) c.perception * 10);
                    uc = Circle.UnitCircle.Point16;
                    new Circle(Circle.UnitCircle.Point8, graphicsDevice, new Color(0.1f, 0.1f, 0.1f, 0.1f), (float)c.Detectionrange, g.position - camera.Position).Render(graphicsDevice);
                }

                if (g.GetType() == typeof(Food))
                {
                    clr = Color.Green;
                    uc = Circle.UnitCircle.Point8;
                }

                new Circle(uc, graphicsDevice, clr, (float)g.size, g.position - camera.Position).Render(graphicsDevice);
            }
            //*/
        }
    }
}
