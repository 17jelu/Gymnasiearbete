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

        bool pause = false;
        public bool SimulationEnd
        {
            get
            {
                return (content.Cells.Count <= 0);
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

        public CellManager()
        {

        }

        public void Initilize()
        {
            Content.Clear();
            civilazationTime = 0;
            pause = false;

            const int starterCells = 5;

            AI.AIType[] starterAI = new AI.AIType[starterCells]
            {
                AI.AIType.PointsTargeting,
                AI.AIType.PointsTargeting,
                AI.AIType.PointsTargeting,
                AI.AIType.PointsTargeting,
                AI.AIType.PointsTargeting
            };

            int[,] starterDNA = new int[starterCells, 3]
            {
                { 10, 20, 100 },
                { 10, 20, 100 },
                { 14, 30, 50 },
                { 14, 30, 50 },
                { 14, 30, 50 }
            };

            for (int i = 0; i < starterCells; i++)
            {
                Cell c = new Cell
                        (this, new Cell(this, null, AI.AIType.NoBrain, Vector2.Zero, 0, 0, 0), starterAI[i],
                        new Vector2(
                            StaticGlobal.Random.Next(-0 * StaticGlobal.SectorSize, starterCells * 2 * StaticGlobal.SectorSize), 
                            StaticGlobal.Random.Next(-0 * StaticGlobal.SectorSize, starterCells * 2 * StaticGlobal.SectorSize)),
                        starterDNA[i, 0], starterDNA[i, 1], starterDNA[i, 2]
                        );

                Content.Add(c);

                for (int f = 0; f < 3; f++)
                {
                    int[] nORp = new int[] { -1, 1 };
                    Content.Add
                        (
                        new Food(
                            new Vector2(
                                c.Position.X + nORp[StaticGlobal.Random.Next(2)] * StaticGlobal.Random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + StaticGlobal.SectorSize),
                                c.Position.Y + nORp[StaticGlobal.Random.Next(2)] * StaticGlobal.Random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + StaticGlobal.SectorSize)
                                )
                            )
                        );
                }
            }
        }

        public void TogglePause()
        {
            pause = !pause;
        }

        public bool IsCell(GameObject g)
        {
            if (g.GetType() == typeof(Cell))
            {
                return true;
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (pause || SimulationEnd)
            {
                return;
            }

            civilazationTime += gameTime.ElapsedGameTime.TotalSeconds;

            //Clearar sectorer
            Point[] sectorKeys = sectors.Keys.ToArray();
            for (int i = 0; i < sectorKeys.Length; i++)
            {
                if (sectors[sectorKeys[i]].All.Count <= 0)
                {
                    if (CellManagerControlls.DeleteSectorIfEmpty)
                    {
                        sectors.Remove(sectorKeys[i]);
                    }
                }
                else
                {
                    sectors[sectorKeys[i]].Clear();
                }
            }

            //Ofixerad Updatering
            for (int i = 0; i < Content.All.Count; i++)
            {
                GameObject g = Content.All[i];

                if (g.isMarkedForDelete)
                {
                    if (IsCell(g))
                    {
                        Cell c = (Cell)g;
                        c.AI.MemoryReward(2, true);
                        c.AI.MemoryReward(-(int)Math.Floor(c.Energy / 100) + 2);
                        c.AI.MemoryFileWrite();
                        //StaticGlobal.Family.KillMember(c.AI.family);
                    }

                    Content.Remove(g);
                }
                else
                {
                    int xMax = (int)Math.Floor((g.Position.X + g.Size) / StaticGlobal.SectorSize);
                    int yMax = (int)Math.Floor((g.Position.Y + g.Size) / StaticGlobal.SectorSize);
                    int xMin = (int)Math.Floor((g.Position.X - g.Size) / StaticGlobal.SectorSize);
                    int yMin = (int)Math.Floor((g.Position.Y - g.Size) / StaticGlobal.SectorSize);

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
                            Content.Add( c.Reproduce() );
                        }
                    }
                    
                }
            }

            //MatSpawn
            spawnTimer++;
            if (spawnTimer > 60 * EnergyControlls.FoodSpawnTime)
            {
                spawnTimer = 0;

                foreach (Cell c in this.Content.Cells)
                {
                    for (int i = 0; i < EnergyControlls.FoodSpawnAmount; i++)
                    {
                        int[] nORp = new int[] { -1, 1 };
                        Content.Add(
                            new Food(
                                new Vector2(
                                    c.Position.X + nORp[StaticGlobal.Random.Next(2)] * StaticGlobal.Random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + StaticGlobal.SectorSize),
                                    c.Position.Y + nORp[StaticGlobal.Random.Next(2)] * StaticGlobal.Random.Next((int)Math.Floor(c.Size), (int)Math.Floor(c.Size) + StaticGlobal.SectorSize)
                                    )
                                )
                            );
                    }
                }
            }

            //FixeradUpdatering
            foreach (Cell c in Content.Cells)
            {
                List<GameObject> detectionCheck = new List<GameObject>();

                int xMax = (int)Math.Floor((c.Position.X + c.Detectionrange) / StaticGlobal.SectorSize);
                int xMin = (int)Math.Floor((c.Position.X - c.Detectionrange) / StaticGlobal.SectorSize);
                int yMax = (int)Math.Floor((c.Position.Y + c.Detectionrange) / StaticGlobal.SectorSize);
                int yMin = (int)Math.Floor((c.Position.Y - c.Detectionrange) / StaticGlobal.SectorSize);

                for (int x = xMin; x <= xMax; x++)
                {
                    for (int y = yMin; y <= yMax; y++)
                    {
                        if (!sectors.ContainsKey(new Point(x, y)))
                        {
                            sectors.Add(new Point(x, y), new SectorContent());
                        }

                        for (int i = 0; i < sectors[new Point(x, y)].All.Count; i++)
                        {
                            if (!detectionCheck.Contains(sectors[new Point(x, y)].All[i]))
                            {
                                detectionCheck.Add(sectors[new Point(x, y)].All[i]);
                            }
                        }
                    }
                }

                c.Update(detectionCheck, StaticGlobal.Random);
            }

            foreach (Food f in Content.Foods)
            {
                f.Update();
            }
        }
    }
}
