using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    /// <summary>
    /// Kontrollerar Alla Celler
    /// </summary>
    class CellManager
    {
        public List<Cell> cells = new List<Cell>();
        public List<Food> food = new List<Food>();

        int sectorSize = 500;
        Dictionary<string, List<GameObject>> sectors = new Dictionary<string, List<GameObject>>();

        public void Update()
        {
            //Clearar sectorer
            foreach (KeyValuePair<string, List<GameObject>> s in sectors)
            {
                s.Value.Clear();
            }

            //Fixerad Updatering
            for (int i = 0; i < cells.Count; i++)
            {
                Cell c = cells[i];
                if (!c.isMarkedForDelete)
                {
                    string sectorPos = new Vector2((float)Math.Floor(c.position.X / sectorSize), (float)Math.Floor(c.position.Y / sectorSize)).ToString();

                    if (!sectors.ContainsKey(sectorPos.ToString()))
                    {
                        sectors.Add(sectorPos, new List<GameObject>());
                    }

                    sectors[sectorPos].Add(c);
                } else
                {
                    cells.Remove(c);
                }
            }

            //OfixeradUpdatering
            foreach (Cell c in cells)
            {
                c.Update();
            }
        }
    }
}
