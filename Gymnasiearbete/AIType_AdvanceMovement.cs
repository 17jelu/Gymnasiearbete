using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gymnasiearbete
{
    class BaseAdvancedMovementAI : AI
    {
        public BaseAdvancedMovementAI(Cell parent, Cell cell) : base(parent, cell)
        {
            aiType = AIType.AdvancedMovement;
            choises = new string[1] { "NULL" };
            //"STOP", "IDLE", "MOVETO", "MOVETOSLOW", "MOVETOFAST", "MOVEFROM", "MOVEFROMSLOW", "MOVEFROMFAST"
        }

        protected override void Intresst(Cell cell, SectorContent percivableObjects)
        {
            string data = "";
            if (percivableObjects.Foods.Count > 0)
            {
                data += "FOOD:" + percivableObjects.Foods.Count;
            }
            List<string> indata = new List<string>();
            foreach (GameObject g in percivableObjects.Cells)
            {
                indata.Add(Data(cell, g));
            }
            indata.Sort();

            for (int i = 0; i < indata.Count; i++)
            {
                int amount = 0;
                data += indata[i] + ":";
                for (int j = i; j < indata.Count; j++)
                {
                    if (i == j)
                    {
                        amount++;
                        indata.RemoveAt(i);
                    }
                }
                data += amount;
            }

            if (data == "")
            {
                data = "NULL";
            }
            MemoryChoice(data);



            lastIntresst = null;
            Decision(cell, lastIntresst);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            
        }
    }
}
