using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    /// <summary>
    /// Grundklass för alla spelplansobjekt
    /// </summary>
    class GameObject
    {
        Vector2 position = Vector2.Zero;
        double rotation = 0;
        double size = 0;

        public GameObject()
        {

        }

        Vector2 Forward()
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }

        public virtual void Update()
        {

        }

        /// <summary>
        /// Retunerar data för att rita. x-pos, y-pos, size, rotation
        /// </summary>
        public float[] DrawData
        {
            get { return new float[4] { position.X, position.Y, (float)size, (float)rotation }; }
        }
    }

    class food : GameObject
    {
        int energy = 100;

        public food()
        {

        }
    }

    /// <summary>
    /// Grund Cell klassen
    /// </summary>
    class Cell : GameObject
    {
        CellManager CM;

        int curiosity = 0;
        int energy = 0;
        int speed = 0;
        int size = 0;
        int perception = 0;

        int preformancepoints = 0;

        int energyRequirement = 0;

        Vector2[] perceptionChecks;

        public Cell(CellManager setCellManager) : base()
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

        public override void Update()
        {

        }
    }
}
