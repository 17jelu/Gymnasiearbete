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
        public float speed = 0;

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

    //MAT
    class Food : GameObject
    {
        float energy = 300;
        public float Energy
        {
            get
            {
                return energy;
            }
        }

        public Food(Vector2 startPosition, float energySet = 300) : base(startPosition)
        {
            energy = energySet;
        }
    }

    //CELL
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

        double consumeScale = 1.2;

        Vector2 idleDirection = new Vector2(0.7071f, 0.7071f);

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
            Cell cchild = new Cell(CM, Vector2.Zero, this.size, this.speed, this.perception);
            

            if (random.Next(0, 100) <= 50)
            {
                cchild.size = Math.Max(1, cchild.size + random.Next(-5, 5+1));
                cchild.speed = Math.Max(0.5f, cchild.speed + random.Next(-10, 10+1) * 0.1f);
                cchild.perception = Math.Max(1, cchild.perception + random.Next(-5, 5+1));
            }

            cchild.idleDirection = -this.idleDirection;
            cchild.position = this.position + cchild.idleDirection * this.Detectionrange + cchild.idleDirection * cchild.Detectionrange + cchild.idleDirection * cchild.size;
            return cchild;
        }
        
        void EnergyManagement()
        {
            energy--;
            //energy -= Math.Max(0.05f, 1 * this.size/20 * this.speed/1 * this.perception/50);
            //energy -= Math.Max(0.05f, this.size / 20 - this.speed / 1 - this.perception / 50);
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
                Actions("0", idleDirection);
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.size > consumeScale * this.size)
                {
                    Vector2 direction = intresst.position - this.position;
                    Actions("2", direction);
                }
                else
                if (this.size > consumeScale * intresst.size && this.speed > intresst.speed)
                {
                    Vector2 direction = intresst.position - this.position;
                    Actions("1", direction);
                }
                else
                {
                    Actions("0", idleDirection);
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = intresst.position - this.position;
                Actions("1", direction);
            }
        }

        void Actions(string decision, Vector2 direction)
        {
            if (direction != Vector2.Zero)
            {
                direction.Normalize();

                switch (decision)
                {
                    case "0":
                        Move(idleDirection);
                        break;

                    case "1":
                        Move(direction);
                        break;

                    case "2":
                        Move(-direction);
                        break;
                }
            }
        }

        public void Update(List<GameObject> detectionCheck, GameWindow window, Random random)
        {
            PerceptionCheck(detectionCheck);
            CollisionCheck(detectionCheck);
            ReproduceCheck();
            EnergyManagement();

            if (this.position.X + this.size / 2 >= window.ClientBounds.Width)
            {
                idleDirection.X = -0.7071f;
            }

            if (this.position.X - this.size / 2 <= 0)
            {
                idleDirection.X = 0.7071f;
            }

            if (this.position.Y + this.size / 2 >= window.ClientBounds.Height)
            {
                idleDirection.Y = -0.7071f;
            }

            if (this.position.Y - this.size / 2 <= 0)
            {
                idleDirection.Y = 0.7071f;
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
                if(consumeScale * g.size < this.size)
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
