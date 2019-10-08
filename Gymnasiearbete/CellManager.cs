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

        public void Update()
        {
            Vector2[] cellPos = new Vector2[cells.Count];
            for (int i = 0; i < cells.Count; i++)
            {
                Cell c = cells[i];
                cellPos[i] = new Vector2(c.position.X, c.position.Y);
            }

            foreach (Cell c in cells)
            {
                c.Update();
            }
        }
    }
}
