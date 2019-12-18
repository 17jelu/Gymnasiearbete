using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gymnasiearbete
{
    class Memory
    {
        static char z = '|';

        string situation;
        public string Situation
        {
            get
            {
                return situation;
            }
        }
        int points;
        public int Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
            }
        }
        string decision;
        public string Desicion
        {
            get
            {
                return decision;
            }
            set
            {
                decision = value;
            }
        }

        public Memory(string situationSet, string pointsSet, string decisionSet)
        {
            situation = situationSet;
            points = int.Parse(pointsSet);
            decision = decisionSet;
        }

        public new string ToString()
        {
            return situation + z + points + z + decision;
        }
    }

}
