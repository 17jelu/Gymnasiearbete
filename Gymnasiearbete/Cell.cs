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

        public void Move(Vector2 direction)
        {
            position += direction;
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

        public int Detectionrange
        {
            get
            {
                return size + perception;
            }
        }

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

        //Perception
        void PerceptionCheck(List<GameObject> pc)
        {
            List<GameObject> percivableObjects = new List<GameObject>();
            foreach (GameObject g in pc)
            {
                if (g != this)
                {
                    //PERCEPTION
                    if (
                        g.position.X*g.position.X - (this.position.X + this.Detectionrange) * (this.position.X + this.Detectionrange) <= 0 && 
                        g.position.Y*g.position.Y - (this.position.Y + this.Detectionrange) * (this.position.Y + this.Detectionrange) <= 0
                        )
                    {
                        percivableObjects.Add(g);
                    }
                }
            }
        }

        void Intresst(List<GameObject> percivableObjects)
        {
            GameObject intresst = null;
            if (percivableObjects.Count > 0)
            {
                intresst = percivableObjects[0];

                foreach (GameObject g in percivableObjects)
                {
                    if (
                        g.position.X * g.position.X - (this.position.X + this.Detectionrange) * (this.position.X + this.Detectionrange) <= intresst.position.X &&
                        g.position.Y * g.position.Y - (this.position.Y + this.Detectionrange) * (this.position.Y + this.Detectionrange) <= intresst.position.Y
                        )
                    {
                        intresst = g;
                    }
                }
            }

            Decision(intresst);
        }

        void Decision(GameObject intresst)
        {
            if (intresst == null)
            {
                Move(new Vector2(1,1));
            }

            if (intresst.GetType() == typeof(Cell))
            {
                Move(-new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y));
            }

            if (intresst.GetType() == typeof(Food))
            {
                Move(-new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y));
            }
        }

        public void Update(List<GameObject> detectionCheck)
        {
            PerceptionCheck(detectionCheck);
        }
    }
}
