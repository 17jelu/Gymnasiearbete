﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gymnasiearbete
{
    //MAT
    class Food : Entity
    {
        Circle circle; // Jesper
        bool hasBeenDrawn; // Jesper
        public bool BeenDrawn // Jesper
        {
            get { return hasBeenDrawn; }
            set { hasBeenDrawn = value; }
        }

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
            circle = new Circle(Circle.UnitCircle.Point8, Color.LawnGreen, size, position);

            energy = energySet;
        }

        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            circle.Render(graphicsDevice, camera);

            hasBeenDrawn = false;
        }
    }

    //CELL
    class Cell : Entity
    {
        Circle circle; // Jesper
        bool hasBeenDrawn = false; // Jesper
        public bool BeenDrawn // Jesper
        {
            get { return hasBeenDrawn; }
            set { hasBeenDrawn = value; }
        }
        protected static CellManager CM;
        protected static float energyRequirement = 333; //1000;
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

        public Cell(CellManager setCellManager, AI aiSet, Vector2 startPosition, float dnaSize, float dnaSpeed, float dnaPerception) : base(startPosition)
        {
            circle = new Circle(Circle.UnitCircle.Point16, Color.White, dnaSize, startPosition); // Jesper

            CM = setCellManager;
            ai = aiSet;
            size = dnaSize;
            speed = dnaSpeed;
            perception = dnaPerception;


            energy = energyRequirement;
        }

        // Jesper
        public void Draw(GraphicsDevice graphicsDevice, Camera camera)
        {
            // TODO (Jesper): fixa
            new Circle(Circle.UnitCircle.Point16,
                new Color(3 * this.Detectionrange * this.speed / 255f, this.size * 2, 127f / 255f, 0.2f),
                this.Detectionrange, this.position).Render(graphicsDevice, camera);

            circle.Position = position;
            circle.Render(graphicsDevice, camera);

            hasBeenDrawn = false;
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
            Cell cchild = new Cell(CM, new AI_ClosestTargetingLearn(new Random(), ai.family), Vector2.Zero, this.size, this.speed, this.perception);

            int mutationChance = 50;
            if (random.Next(100) < mutationChance)
            {
                cchild.size = Math.Max(1, cchild.size + random.Next(-5, 5+1));
                cchild.speed = Math.Max(0.5f, cchild.speed + random.Next(-10, 10+1) * 0.1f);
                cchild.perception = Math.Max(1, cchild.perception + random.Next(-5, 5+1));
            }

            Vector2 reverseDirection = -this.ai.Direction;
            reverseDirection.Normalize();
            cchild.ai.Direction = reverseDirection;
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
