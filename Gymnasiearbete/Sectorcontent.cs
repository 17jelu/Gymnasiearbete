using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gymnasiearbete
{
    class SectorContent
    {
        List<Food> food = new List<Food>();
        List<Cell> cell = new List<Cell>();
        List<GameObject> other = new List<GameObject>();

        public List<GameObject> All
        {
            get
            {
                List<GameObject> content = new List<GameObject>();

                content.AddRange(food);
                content.AddRange(cell);
                content.AddRange(other);

                return content;
            }
        }

        public List<Food> Foods
        {
            get { return food; }
        }

        public List<Cell> Cells
        {
            get { return cell; }
        }

        public List<GameObject> Others
        {
            get { return other; }
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
