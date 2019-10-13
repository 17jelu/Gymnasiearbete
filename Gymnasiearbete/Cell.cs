using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public int size = 0;
        public int speed = 1;

        public bool isMarkedForDelete = false;

        public GameObject(Vector2 startPosition)
        {
            position = startPosition;
            size = 10;
        }

        Vector2 Forward()
        {
            return new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        }

        public void CollisionCheck(List<GameObject> gs)
        {
            foreach (GameObject g in gs)
            {
                if (
                    Math.Pow(g.position.X - this.position.X, 2) + Math.Pow(g.position.Y - this.position.Y, 2) <=
                    Math.Pow(g.size - this.size, 2)
                    )
                {
                    Collision(g);
                }
            }
        }

        public virtual void Collision(GameObject g)
        {

        }

        public void Move(Vector2 direction)
        {
            //direction.Normalize();
            position += direction * speed;
        }

        public virtual void Update()
        {

        }
    }

    class Food : GameObject
    {
        public int Energy
        {
            get
            {
                return 300;
            }
        }

        public Food(Vector2 startPosition) : base(startPosition)
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
        //int speed = 0;
        //int size = 0;
        int perception = 0;

        int preformancepoints = 0;
        public int energy = 0;
        int energyRequirement = 1000;

        Vector2 idleDirection = new Vector2(1, 1);

        public bool isMarkForReproduce = false;

        public int Detectionrange
        {
            get
            {
                return (int)size + perception;
            }
        }

        public Cell(CellManager setCellManager, Vector2 startPosition) : base(startPosition)
        {
            CM = setCellManager;
            size = 20;
            perception = 75;

            energy = energyRequirement;
        }

        void Reproduce(GameWindow window, Random random)
        {
            if (energy > 2 * energyRequirement)
            {
                energy -= energyRequirement;
                isMarkForReproduce = true;
            } else
            {
                isMarkForReproduce = false;
            }
        }

        void EnergyManagement()
        {
            //energy -= (int)size + perception - speed;
            energy--;
            if (energy <= 0)
            {
                this.isMarkedForDelete = true;
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
                        Math.Pow(g.position.X - this.position.X, 2) + Math.Pow(g.position.Y - this.position.Y, 2) <= 
                        Math.Pow(this.Detectionrange, 2) + Math.Pow(g.size, 2)
                        )
                    {
                        percivableObjects.Add(g);
                    }
                }
            }

            Intresst(percivableObjects);
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
                        Math.Pow(g.position.X - this.position.X, 2) + Math.Pow(g.position.Y - this.position.Y, 2) <=
                        Math.Pow(intresst.position.X - this.position.X, 2) + Math.Pow(intresst.position.Y - this.position.Y, 2)
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
                idleDirection.Normalize();
                Move(idleDirection);
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.size > this.size)
                {
                    Vector2 direction = -new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y);
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        Move(direction);
                    }
                } else
                {
                    idleDirection.Normalize();
                    Move(idleDirection);
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y);
                if (direction != Vector2.Zero)
                {
                    direction.Normalize();
                    Move(direction);
                }
            }
        }

        public void Update(List<GameObject> detectionCheck, GameWindow window, Random random)
        {
            PerceptionCheck(detectionCheck);
            CollisionCheck(detectionCheck);
            Reproduce(window, random);
            EnergyManagement();

            if (this.position.X >= window.ClientBounds.Width)
            {
                idleDirection.X = -1;
            }

            if (this.position.X <= 0)
            {
                idleDirection.X = 1;
            }

            if (this.position.Y >= window.ClientBounds.Height)
            {
                idleDirection.Y = -1;
            }

            if (this.position.Y <= 0)
            {
                idleDirection.Y = 1;
            }
        }

        public override void Collision(GameObject g)
        {
            if (g == null)
            {

            }
            else
            if (g.GetType() == typeof(Cell))
            {
                Cell c = (Cell)g;
                if(2 * g.size < this.size)
                {
                    g.isMarkedForDelete = true;
                    this.energy += c.energy;
                }
            }
            else
            if (g.GetType() == typeof(Food))
            {
                Food f = (Food)g;
                g.isMarkedForDelete = true;
                this.energy += f.Energy;
            }
        }
    }
}
