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
        public Vector2 position = Vector2.Zero;
        double rotation = 0;
        double size = 0;

        public bool isMarkedForDelete = false;

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

    class Food : GameObject
    {
        int energy = 100;

        public Food()
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

        void PerceptionCheck(List<GameObject> cl)
        {
            foreach (Cell c in cl)
            {
                if (c != this)
                {
                    //PERCEPTION
                    if (
                        c.position.X*c.position.X - (this.position.X + this.size + this.perception) * (this.position.X + this.size + this.perception) >= 0 && 
                        c.position.Y*c.position.Y - (this.position.Y + this.size + this.perception) * (this.position.Y + this.size + this.perception) <= 0
                        )
                    {
                        //DECITIONS
                    }
                }
            }
        }

        public void Update(List<GameObject> detectionRange)
        {
            PerceptionCheck(detectionRange);
        }
    }
}
