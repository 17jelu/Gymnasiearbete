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
        protected CellManager CM;
        protected static float energyRequirement = 500; //333; //1000;
        public static double consumeScale = 1.2;

        protected float perception = 0;

        protected AI ai;
        public AI AI
        {
            get
            {
                return ai;
            }
        }

        public bool isMarkForReproduce = false;

        public float Detectionrange
        {
            get
            {
                return size + perception;
            }
        }

        public Cell(CellManager setCellManager, Cell parent, AI.AIType aiType, Vector2 startPosition, float dnaSize, float dnaSpeed, float dnaPerception) : base(startPosition)
        {
            CM = setCellManager;
            ai = AI.GetAI(aiType, parent, this);
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

        public Cell Reproduce(Random random)
        {
            //ai.preformancepoints++;
            AI.MemoryFileWrite();
            Cell cchild = new Cell(CM, this, AI.AIType.CloseTargeting, Vector2.Zero, this.size, this.speed, this.perception);

            int mutationChance = 50;
            if (random.Next(100) < mutationChance)
            {
                cchild.size = Math.Max(1, cchild.size + random.Next(-5, 5+1));
                cchild.speed = Math.Max(0.5f, cchild.speed + random.Next(-10, 10+1) * 0.1f);
                cchild.perception = Math.Max(1, cchild.perception + random.Next(-5, 5+1));
            }

            cchild.position = this.position + cchild.ai.Direction * this.Detectionrange + cchild.ai.Direction * cchild.Detectionrange;
            return cchild;
        }
        
        void EnergyManagement()
        {
            energy -= (this.size/10 + this.speed/2 + this.perception/30)/3;
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
                    if (
                        Math.Pow(g.Position.X - this.position.X, 2) + Math.Pow(g.Position.Y - this.position.Y, 2) <= 
                        Math.Pow(this.Detectionrange, 2) + Math.Pow(g.Size, 2)
                        )
                    {
                        percivableObjects.Add(g);
                    }
                }
            }

            ai.AIR(this, percivableObjects);
        }

        public void Update(List<GameObject> detectionCheck, Random random)
        {
            PerceptionCheck(detectionCheck);
            CollisionCheck(detectionCheck);
            ReproduceCheck();
            EnergyManagement();
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
                if(g.Size * consumeScale < this.size)
                {
                    ai.MemoryReward(3);
                    this.energy += c.energy;
                    g.isMarkedForDelete = true;
                }
            }
            else
            if (g.GetType() == typeof(Food))
            {
                Food f = (Food)g;
                g.isMarkedForDelete = true;
                this.energy += f.Energy;
                ai.MemoryReward(3);
            }
        }

        public override string DEBUG()
        {
            string result = "";
            result += "[" + Math.Floor(Position.X) + ":" + Math.Floor(Position.Y) + "]";
            result += "[" + Size + "; " + Speed + "; " + perception + "]";
            result += "[" + energy + "]";
            return "{" + result + "}";
        }
    }
}
