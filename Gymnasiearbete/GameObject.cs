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
        public float size = 0;
        public float speed = 0;

        public bool isMarkedForDelete = false;

        public GameObject(Vector2 startPosition)
        {
            position = startPosition;
            size = 10;
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

        

        public virtual void Update()
        {

        }
    }

    /// <summary>
    /// Grundklass för alla "levande" spelplansobjekt
    /// </summary>
    class Entity : GameObject
    {
        public float energy = 0;

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
}
