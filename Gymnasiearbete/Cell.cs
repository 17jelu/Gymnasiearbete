using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gymnasiearbete
{
    class Cell
    {
        CellManager CM;

        int energy = 0;
        int speed = 0;
        int size = 0;
        int perception = 0;

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
    }
}
