using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    //MAT
    class Food : Entity
    {
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
    class Cell : Entity
    {
        protected static CellManager CM;
        protected static float energyRequirement = 333; //1000;
        public static double consumeScale = 1.2;

        protected float perception = 0;

        protected AI ai;

        protected Vector2 idleDirection = new Vector2(1f, 1f);

        public bool isMarkForReproduce = false;

        public float Detectionrange
        {
            get
            {
                return size + perception;
            }
        }

        public Cell(CellManager setCellManager, AI aiSet, Vector2 startPosition, float dnaSize, float dnaSpeed, float dnaPerception) : base(startPosition)
        {
            CM = setCellManager;
            ai = aiSet;
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
            ai.preformancepoints++;
            ai.MemoryFileWrite();
            Cell cchild = new Cell(CM, new AI_ClosestTargetingLearn(new Random(), ai.family), Vector2.Zero, this.size, this.speed, this.perception);

            int mutationChance = 50;
            if (random.Next(100) < mutationChance)
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
                        Math.Pow(g.Position.X - this.position.X, 2) + Math.Pow(g.Position.Y - this.position.Y, 2) <= 
                        Math.Pow(this.Detectionrange, 2) + Math.Pow(g.Size, 2)
                        )
                    {
                        percivableObjects.Add(g);
                    }
                }
            }

            Actions(ai.AIR(this, percivableObjects));
        }

        protected virtual void Actions(int[] decision)
        {
            Vector2 direction = new Vector2(decision[1], decision[2]);
            switch (decision[0])
            {
                case 0:
                    Move(idleDirection);
                    break;

                case 1:
                    Move(direction);
                    break;

                case -1:
                    Move(-direction);
                    break;

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

        protected override void Collision(GameObject g)
        {
            if (g == null)
            {
                return;
            }
            
            if (g.GetType() == typeof(Cell))
            {
                Cell c = (Cell)g;
                if(consumeScale * g.Size < this.size)
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

        public string Debug_Cell()
        {
            return "{SZ[" + this.Size + "] SP[" + this.Speed + "] DR[" + this.Detectionrange + "] EG[" + this.energy + "]}" + ai.Debug_AI();
        }
    }
}
