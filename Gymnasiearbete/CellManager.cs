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
        SectorContent content = new SectorContent();
        public SectorContent Content
        {
            get
            {
                return content;
            }
        }

        int spawnTimer = 0;

        public bool pause = false;
        public bool simulationEnd = false;

        public static int SectorSize {
            get
            {
                return 100;
            }
        }

        Dictionary<Point, SectorContent> sectors = new Dictionary<Point, SectorContent>();
        public Dictionary<Point, SectorContent> Sectors
        {
            get
            {
                return sectors;
            }
        }

        public CellManager(Rectangle simulationAreaSet, Random random)
        {
            const int starterCells = 5;

            AI.AIType[] starterAI = new AI.AIType[starterCells]
            {
                AI.AIType.Player,
                AI.AIType.CloseTargeting,
                AI.AIType.CloseTargeting,
                AI.AIType.CloseTargeting,
                AI.AIType.CloseTargeting
            };

            int[,] starterDNA = new int[starterCells, 3]
            {
                { 10, 2, 30 },
                { 10, 2, 30 },
                { 10, 2, 30 },
                { 10, 2, 30 },
                { 10, 2, 30 }
            };

            for (int i = 0; i < starterCells; i++)
            {

                Cell c = new Cell
                        (this, new Cell(this, null, AI.AIType.NoBrain, Vector2.Zero, 0,0,0), starterAI[i],
                        new Vector2(random.Next(-0 * CellManager.SectorSize, starterCells * 2 * CellManager.SectorSize), random.Next(-0 * CellManager.SectorSize, starterCells * 2 * CellManager.SectorSize)),
                        starterDNA[i, 0], starterDNA[i, 1], starterDNA[i, 2]
                        );

                Content.Add(c);

                for (int f = 0; f < 3; f++) {
                    int[] nORp = new int[] { -1, 1 };
                    Content.Add
                        (
                        new Food(
                            new Vector2(
                                c.Position.X + nORp[random.Next(2)] * random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + SectorSize),
                                c.Position.Y + nORp[random.Next(2)] * random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + SectorSize)
                                )
                            )
                        );
                }
            }
        }

        public string DebugSector(float Xposition, float Yposition, int debugType = 0)
        {
            string output = "";
            int x = (int)Math.Floor(Xposition / SectorSize);
            int y = (int)Math.Floor(Yposition / SectorSize);

            if (sectors.ContainsKey(new Point(x, y)))
            {
                foreach (GameObject g in sectors[new Point(x, y)].All())
                {
                    if (debugType == 1 || debugType == 0)
                    {
                        output += "{" + Content.All().IndexOf(g) + "-" + g.GetType().ToString().Split('.')[1] + "}";
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

            //Clearar sectorer
            foreach (KeyValuePair<Point, SectorContent> s in sectors)
            {
                s.Value.Clear();
            }

            //Ofixerad Updatering
            for (int i = 0; i < Content.All().Count; i++)
            {
                GameObject g = Content.All()[i];

                if (g.isMarkedForDelete)
                {
                    if (g.GetType() == typeof(Cell))
                    {
                        Cell c = (Cell)g;
                        c.AI.MemoryReward(-2, false);
                        c.AI.MemoryFileWrite();
                    }

                    Content.Remove(g);
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
                                sectors.Add(new Point(x, y), new SectorContent());
                            }
                            sectors[new Point(x, y)].Add(g);
                        }
                    }
                    
                    if (IsCell(g))
                    {
                        Cell c = (Cell)g;

                        if (c.isMarkForReproduce)
                        {
                            Content.AddRange(new GameObject[1] { c.Reproduce(random) });
                        }
                    }
                }
            }

            //FixeradUpdatering
            foreach (GameObject g in Content.All())
            {
                if (IsCell(g))
                {
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
                                sectors.Add(new Point(x, y), new SectorContent());
                            }

                            for (int i = 0; i < sectors[new Point(x, y)].All().Count; i++)
                            {
                                if (!detectionCheck.Contains(sectors[new Point(x, y)].All()[i]))
                                {
                                    detectionCheck.Add(sectors[new Point(x, y)].All()[i]);
                                }
                            }
                        }
                    }

                    c.Update(detectionCheck, random);
                }
            }

            //MatSpawn
            spawnTimer++;
            if (spawnTimer > 60 * 3)
            {
                spawnTimer = 0;

                foreach (Cell c in this.Content.Cells())
                {
                    int[] nORp = new int[] { -1, 1 };
                    Content.Add(
                        new Food(
                            new Vector2(
                                c.Position.X + nORp[random.Next(2)] * random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + SectorSize),
                                c.Position.Y + nORp[random.Next(2)] * random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + SectorSize)
                                )
                            )
                        );
                }
            }

            if (content.Cells.Count() <= 0)
            {
                simulationEnd = true;
            }
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            foreach (GameObject g in Content.All())
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
