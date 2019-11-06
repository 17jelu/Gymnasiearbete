using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class Sectorcontent
    {
        List<Food> food = new List<Food>();
        List<Cell> cell = new List<Cell>();
        List<GameObject> other = new List<GameObject>();

        public List<GameObject> Content()
        {
            List<GameObject> content = new List<GameObject>();

            content.AddRange(food);
            content.AddRange(cell);

            return content;
        }

        public List<Food> Foods()
        {
            return food;
        }

        public List<Cell> Cells()
        {
            return cell;
        }

        public List<GameObject> Others()
        {
            return other;
        }

        public Dictionary<Cell, Vector2> cellDestination()
        {
            Dictionary<Cell, Vector2> destination = new Dictionary<Cell, Vector2>();
            foreach (Cell c in cell)
            {
                destination.Add(c, c.AI.lastIntresst.Position);
            }

            return destination;
        }

        public void Clear()
        {
            food = new List<Food>();
            cell = new List<Cell>();
            other = new List<GameObject>();
        }

        public void Add(GameObject g)
        {
            if (g.GetType() == typeof(Food))
            {
                food.Add((Food)g);
            }
            else
            if (g.GetType() == typeof(Cell))
            {
                cell.Add((Cell)g);
            }
            else
            {
                other.Add(g);
            }
        }

        public void AddRange(GameObject[] gs)
        {
            foreach (GameObject g in gs)
            {
                this.Add(g);
            }
        }

        public void Remove(GameObject g)
        {
            if (g.GetType() == typeof(Food))
            {
                food.Remove((Food)g);
            }
            else
            if (g.GetType() == typeof(Cell))
            {
                cell.Remove((Cell)g);
            }
            else
            {
                other.Remove((Food)g);
            }
        }
    }
}
