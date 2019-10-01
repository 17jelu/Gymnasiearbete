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

        public void Update()
        {
            foreach (Cell c in cells)
            {
                c.Update();
            }
        }
    }
}
