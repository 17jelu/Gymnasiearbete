using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Gymnasiearbete
{
    /// <summary>
    /// Grundklass för alla spelplansobjekt
    /// </summary>
    class GameObject
    {
        protected Vector2 position = Vector2.Zero;
        public Vector2 Position
        {
            get
            {
                return position;
            }
        }

        protected float size = 0;
        public float Size
        {
            get
            {
                return size;
            }
        }
        protected float speed = 0;
        public float Speed
        {
            get
            {
                return speed;
            }
        }

        public bool isMarkedForDelete = false;

        public GameObject(Vector2 startPosition)
        {
            position = startPosition;
            size = 10;
        }

        protected void CollisionCheck(List<GameObject> gs)
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

        protected virtual void Collision(GameObject g)
        {

        }

        

        public virtual void Update()
        {

        }

        public void SetPosition(Vector2 newPosition)
        {
            position = newPosition;
        }

        public virtual string DEBUG()
        {
            string result = "";
            result += "[" + Math.Floor(Position.X) + ":" + Math.Floor(Position.Y) + "]";
            return "{" + result + "}";
        }
    }

    /// <summary>
    /// Grundklass för alla "levande" spelplansobjekt
    /// </summary>
    class Entity : GameObject
    {
        protected float energy = 0;

        public Entity(Vector2 startPosition) : base(startPosition)
        {

        }

        public void Move(Vector2 direction)
        {
            if (direction != Vector2.Zero)
            {
                direction.Normalize();
                position += direction * speed;
            }
        }
    }

    /// <summary>
    /// Tomt GameObject vilken endast markerar en punkt och har kort namn
    /// </summary>
    class P : GameObject
    {
        public P(Vector2 startPosition) : base(startPosition)
        {

        }
    }
}
