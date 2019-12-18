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
        int lifetime = 60 * 5 * (int)EnergyControlls.FoodSpawnTime;


        public Food(Vector2 startPosition) : base(startPosition)
        {
            energy = (float)EnergyControlls.FoodEnergy;
        }

        public override void Update()
        {
            if (--lifetime < 0)
            {
                this.isMarkedForDelete = true;
            }
        }
    }

    //CELL
    class Cell : Entity
    {
        public float Energy
        {
            get
            {
                return energy;
            }
        }

        protected CellManager CM;
        public static float energyRequirement = (float)EnergyControlls.CellEnergyRequirement; //500; //333; //1000;
        public static double consumeScale = 1.2;

        protected float perception = 0;

        protected AI ai;
        public AI AI
        {
            get
            {
                return ai;
            }
            set
            {
                ai = value;
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
                isMarkForReproduce = true;
            } else
            {
                isMarkForReproduce = false;
            }
        }

        public Cell Reproduce()
        {
            isMarkForReproduce = false;
            energy -= energyRequirement;
            AI.MemoryReward(1, true);
            AI.MemoryFileWrite();
            Cell cchild = new Cell(CM, this, ai.GetAIType, Position, this.size, this.speed, this.perception);

            int mutationChance = 50;
            if (StaticGlobal.Random.Next(100) < mutationChance)
            {
                cchild.size = Math.Max(1, cchild.size + StaticGlobal.Random.Next(-5, 5+1));
                cchild.speed = Math.Max(1, cchild.speed + StaticGlobal.Random.Next(-5, 5+1));
                cchild.perception = Math.Max(1, cchild.perception + StaticGlobal.Random.Next(-5, 5+1));
            }

            cchild.position = this.position + cchild.ai.Direction * this.Detectionrange + cchild.ai.Direction * cchild.Detectionrange;
            return cchild;
        }
        
        public void EnergyManagement(float amount = 0)
        {
            /*
            energy -= (
                this.size/CellManagerControlls.DefaultCellSize + 
                this.speed/CellManagerControlls.DefaultCellSpeed + 
                this.perception/CellManagerControlls.DefaultCellPerception
                )/3;
                */

            if (amount > 0)
            {
                ai.MemoryReward((int)Math.Floor(amount / 100));
            }

            energy += amount;
            if (energy <= 0)
            {
                this.isMarkedForDelete = true;
            }
        }

        //Perception
        void PerceptionCheck(List<GameObject> pc)
        {
            SectorContent percivableObjects = new SectorContent();
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
            //EnergyManagement();
            if (!isMarkedForDelete)
            {
                PerceptionCheck(detectionCheck);
                CollisionCheck(detectionCheck);
                //ReproduceCheck();
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
                if(g.Size * consumeScale < this.size)
                {
                    EnergyManagement(c.Energy);
                    g.isMarkedForDelete = true;
                }
            }
            else
            if (g.GetType() == typeof(Food))
            {
                Food f = (Food)g;
                EnergyManagement(f.Energy);
                g.isMarkedForDelete = true;
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
