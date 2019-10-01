using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    /// <summary>
    /// Grund Cell klassen
    /// </summary>
    class Cell
    {
        CellManager CM;

        Vector2 position = Vector2.Zero;
        double rotation = 0;

        int curiosity = 0;
        int energy = 0;
        int speed = 0;
        int size = 0;
        int perception = 0;

        int preformancepoints = 0;

        int energyRequirement = 0;

        public Cell(CellManager setCellManager)
        {
            CM = setCellManager;
        }

        void Reproduce()
        {
            if (energy > energyRequirement)
            {
                energy -= energyRequirement;
                for (int i = 0; i < energy/energyRequirement; i++)

                CM.cells.Add(new Cell(CM));
            }
        }

        Vector2 Forward()
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }



        public void Update()
        {

        }

        /// <summary>
        /// Retunerar data för att rita. x-pos, y-pos, size, rotation
        /// </summary>
        public float[] DrawData
        {
            get { return new float[4] { position.X, position.Y, size, (float)rotation }; }
        }
    }
}
