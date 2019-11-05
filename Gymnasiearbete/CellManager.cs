using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gymnasiearbete
{
    /// <summary>
    /// Kontrollerar Alla Celler
    /// </summary>
    class CellManager
    {
        public double civilazationTime = 0;
        List<GameObject> objects = new List<GameObject>();

        public static Rectangle simulationArea;

        int spawnTimer = 0;

        public bool pause = false;
        public bool simulationEnd = false;

        public static int SectorSize {
            get
            {
                return 100;
            }
        }

        Dictionary<Point, List<GameObject>> sectors = new Dictionary<Point, List<GameObject>>();

        public CellManager(Rectangle simulationAreaSet)
        {
            simulationArea = new Rectangle(simulationAreaSet.X * SectorSize, simulationAreaSet.Y * SectorSize, simulationAreaSet.Width * SectorSize, simulationAreaSet.Height * SectorSize);
        }

        public void AddObjects(GameObject[] gs)
        {
            objects.AddRange(gs);
        }

        public string DebugSector(float Xposition, float Yposition, int debugType = 0)
        {
            string output = "";
            int x = (int)Math.Floor(Xposition / SectorSize);
            int y = (int)Math.Floor(Yposition / SectorSize);

            if (sectors.ContainsKey(new Point(x, y)))
            {
                foreach (GameObject g in sectors[new Point(x, y)])
                {
                    if (debugType == 1 || debugType == 0)
                    {
                        output += "{" + objects.IndexOf(g) + "-" + g.GetType().ToString().Split('.')[1] + "}";
                    }
                    if (debugType == 2 || debugType == 0)
                    {
                        output += g.DEBUG();
                    }
                    if (debugType == 3 || debugType == 0)
                    {
                        if (g.GetType() == typeof(Cell))
                        {
                            Cell c = (Cell)g;
                            output += c.AI.DEBIUG();
                        }
                    }
                    output += "\n";
                }

                return output;
            }

            return "-";
        }

        public bool IsCell(GameObject g)
        {
            if (g.GetType() == typeof(Cell))
            {
                return true;
            }

            return false;
        }

        public void Update(Random random, GameTime gameTime)
        {
            if (pause || simulationEnd)
            {
                return;
            }

            civilazationTime += gameTime.ElapsedGameTime.TotalSeconds;

            int foodCount = 0;
            int cellCount = 0;

            //Clearar sectorer
            foreach (KeyValuePair<Point, List<GameObject>> s in sectors)
            {
                s.Value.Clear();
            }

            //Ofixerad Updatering
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject g = objects[i];

                if (g.isMarkedForDelete)
                {
                    if (g.GetType() == typeof(Cell))
                    {
                        Cell c = (Cell)g;
                        c.AI.MemoryReward(-2, false);
                        c.AI.MemoryFileWrite();
                    }

                    objects.Remove(g);
                }
                else
                {
                    int xMax = (int)Math.Floor((g.Position.X + g.Size) / SectorSize);
                    int yMax = (int)Math.Floor((g.Position.Y + g.Size) / SectorSize);
                    int xMin = (int)Math.Floor((g.Position.X - g.Size) / SectorSize);
                    int yMin = (int)Math.Floor((g.Position.Y - g.Size) / SectorSize);

                    for (int x = xMin; x <= xMax; x++)
                    {
                        for (int y = yMin; y <= yMax; y++)
                        {
                            if (!sectors.ContainsKey(new Point(x, y)))
                            {
                                sectors.Add(new Point(x, y), new List<GameObject>());
                            }
                            sectors[new Point(x, y)].Add(g);
                        }
                    }
                    
                    if (IsCell(g))
                    {
                        Cell c = (Cell)g;

                        if (c.isMarkForReproduce)
                        {
                            this.AddObjects(new GameObject[1] { c.Reproduce(random) });
                        }
                    }
                }
            }

            //FixeradUpdatering
            foreach (GameObject g in objects)
            {
                if (IsCell(g))
                {
                    cellCount++;
                    Cell c = (Cell)g;
                    List<GameObject> detectionCheck = new List<GameObject>();

                    int xMax = (int)Math.Floor((c.Position.X + c.Detectionrange) / SectorSize);
                    int xMin = (int)Math.Floor((c.Position.X - c.Detectionrange) / SectorSize);
                    int yMax = (int)Math.Floor((c.Position.Y + c.Detectionrange) / SectorSize);
                    int yMin = (int)Math.Floor((c.Position.Y - c.Detectionrange) / SectorSize);

                    for (int x = xMin; x <= xMax; x++)
                    {
                        for (int y = yMin; y <= yMax; y++)
                        {
                            if (!sectors.ContainsKey(new Point(x, y)))
                            {
                                sectors.Add(new Point(x, y), new List<GameObject>());
                            }

                            for (int i = 0; i < sectors[new Point(x, y)].Count; i++)
                            {
                                if (!detectionCheck.Contains(sectors[new Point(x, y)][i]))
                                {
                                    detectionCheck.Add(sectors[new Point(x, y)][i]);
                                }
                            }
                        }
                    }

                    c.Update(detectionCheck, random);
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
                                random.Next(simulationArea.X, simulationArea.X + simulationArea.Width),
                                random.Next(simulationArea.Y, simulationArea.Y + simulationArea.Height)
                            )
                        )
                    }
                    );
            }

            if (cellCount <= 0)
            {
                simulationEnd = true;
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            foreach (GameObject g in objects)
            {
                Color clr = Color.White;
                Circle.UnitCircle uc = Circle.UnitCircle.Point8;

                if (g.GetType() == typeof(Cell))
                {
                    Cell c = (Cell)g;

                    clr = new Color((int) c.Size * 10, (int) c.Speed * 10, (int) c.Detectionrange * 10);
                    uc = Circle.UnitCircle.Point16;
                    new Circle(Circle.UnitCircle.Point8, graphicsDevice, new Color(0.1f, 0.1f, 0.1f, 0.1f), (float)c.Detectionrange, g.Position - camera.Position).Render(graphicsDevice);
                }

                if (g.GetType() == typeof(Food))
                {
                    clr = Color.Green;
                    uc = Circle.UnitCircle.Point8;
                }

                new Circle(uc, graphicsDevice, clr, (float)g.Size, g.Position - camera.Position).Render(graphicsDevice);
            }
        }
    }
}
