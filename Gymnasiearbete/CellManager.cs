using System;
using System.Collections.Generic;
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
        List<GameObject> objects = new List<GameObject>();

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

        public void Update(GameWindow window, Random random)
        {
            //Clearar sectorer
            foreach (KeyValuePair<string, List<GameObject>> s in sectors)
            {
                s.Value.Clear();
            }

            //Fixerad Updatering
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject c = objects[i];
                if (!c.isMarkedForDelete)
                {
                    //får sectorposition
                    string sectorPos = new Vector2((float)Math.Floor(c.position.X / SectorSize), (float)Math.Floor(c.position.Y / SectorSize)).ToString();

                    if (!sectors.ContainsKey(sectorPos))
                    {
                        sectors.Add(sectorPos, new List<GameObject>());
                    }

                    sectors[sectorPos].Add(c);
                } else
                {
                    objects.Remove(c);
                }
            }

            //OfixeradUpdatering
            foreach (GameObject g in objects)
            {
                if (g.GetType() == typeof(Cell))
                {
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
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            foreach (GameObject g in objects)
            {
                Color clr = Color.White;

                if (g.GetType() == typeof(Cell))
                {
                    clr = Color.Red;
                    Cell c = (Cell)g;
                    new Circle(Circle.UnitCircle.Point16, graphicsDevice, new Color(0.5f, 0.5f, 0.5f, 0.3f), (float)c.Detectionrange, g.position - camera.Position).Render(graphicsDevice);
                }

                if (g.GetType() == typeof(Food))
                {
                    clr = Color.Green;
                }

                new Circle(Circle.UnitCircle.Point16, graphicsDevice, clr, (float)g.size, g.position - camera.Position).Render(graphicsDevice);
            }
        }
    }
}
