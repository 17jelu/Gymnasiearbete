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
        readonly double rotation = 0;
        public float size = 0;
        public float speed = 1;

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
                    Math.Pow(g.size + this.size, 2)
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
        static CellManager CM;

        //int curiosity = 0;
        //int speed = 0;
        //int size = 0;
        public float perception = 0;

        int preformancepoints = 0;
        
        public float energy = 0;
        float energyRequirement = 1000;

        Vector2 idleDirection = new Vector2(1, 1);

        public bool isMarkForReproduce = false;

        public float Detectionrange
        {
            get
            {
                return size + perception;
            }
        }

        public Cell(CellManager setCellManager, Vector2 startPosition, float dnaSize, float dnaSpeed, float dnaPerception) : base(startPosition)
        {
            CM = setCellManager;
            size = dnaSize;
            speed = dnaSpeed;
            perception = dnaPerception;


            energy = energyRequirement;
        }

        void ReproduceCheck()
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

        public Cell Reproduce(GameWindow window, Random random)
        {
            Cell cchild = new Cell(CM, new Vector2(this.position.X - this.Detectionrange, this.position.Y - this.Detectionrange), this.size, this.speed, this.perception);
            
            if (random.Next(0, 100) <= 50)
            {
                cchild.size = Math.Max(1, cchild.size + random.Next(-1, 2) * 2);
                cchild.speed = Math.Max(1, cchild.speed + random.Next(-1, 2) * 1);
                cchild.perception = Math.Max(1, cchild.perception + random.Next(-1, 2) * 5);
            }

            return cchild;
        }
        
        void EnergyManagement()
        {
            //energy -= Math.Max(0.05f, 1 * this.size/20 * this.speed/1 * this.perception/50);
            energy -= Math.Max(0.05f, this.size / 20 - this.speed / 1 - this.perception / 50);
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
                if (intresst.size > 1.2 * this.size)
                {
                    Vector2 direction = -new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y);
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        Move(direction);
                    }
                }
                else
                if (this.size > 1.2 * intresst.size && this.speed > intresst.speed)
                {
                    Vector2 direction = new Vector2(intresst.position.X - this.position.X, intresst.position.Y - this.position.Y);
                    if (direction != Vector2.Zero)
                    {
                        direction.Normalize();
                        Move(direction);
                    }
                }
                else
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
            ReproduceCheck();
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
                if(1.2 * g.size < this.size)
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
