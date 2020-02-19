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
            //"STOP", "IDLE", "MOVETO", "MOVETOSLOW", "MOVETOFAST", "MOVEFROM", "MOVEFROMSLOW", "MOVEFROMFAST" };
        }

        protected override void Intresst(Cell cell, SectorContent percivableObjects)
        {
            string data = "!";
            string dataCount = "[COUNT]";
            if (percivableObjects.Foods.Count > 0)
            {
                dataCount += "FOOD:" + percivableObjects.Foods.Count;
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
                dataCount += indata[i] + ":";
                for (int j = i; j < indata.Count; j++)
                {
                    if (i == j)
                    {
                        amount++;
                        indata.RemoveAt(i);
                    }
                }
                dataCount += amount;
            }

            if (dataCount == "!")
            {
                dataCount = "NULL";
            }

            string dataClose = "[CLOSE]";
            if (percivableObjects.All.Count > 0)
            {
                GameObject g;
                g = percivableObjects.All[0];

                //gör intresset till det närmaste objectet av saker AI kan se
                foreach (GameObject ge in percivableObjects.All)
                {
                    if (
                        Math.Pow(ge.Position.X - cell.Position.X, 2) + Math.Pow(ge.Position.Y - cell.Position.Y, 2) <=
                        Math.Pow(g.Position.X - cell.Position.X, 2) + Math.Pow(g.Position.Y - cell.Position.Y, 2)
                        )
                    {
                        g = ge;
                    }
                }

                dataClose += Data(cell, g);
            } else
            {
                dataClose += "NULL";
            }

            data += dataCount + dataClose;
            MemoryChoice(data);



            lastIntresst = null;
            Decision(cell, lastIntresst);
        }

        public override string Data(Cell cell, GameObject dataObject)
        {
            string dataOut = "NULL" + "/E" + (cell.Energy > Cell.energyRequirement);
            if (dataObject != null)
            {
                if (dataObject.GetType() == typeof(Cell))
                {
                    Cell dataCell = (Cell)dataObject;

                    dataOut = "CELL";

                    dataOut += "/S";
                    string sizeData = "=";
                    if (dataCell.Size > Cell.consumeScale * cell.Size)
                    {
                        sizeData = ">";
                    }

                    if (dataCell.Size * Cell.consumeScale < cell.Size)
                    {
                        sizeData = "<";
                    }
                    dataOut += sizeData;

                    dataOut += "/V";
                    string speedData = "=";
                    if (dataCell.Speed > cell.Speed)
                    {
                        speedData = ">";
                    }
                    if (dataCell.Speed < cell.Speed)
                    {
                        speedData = "<";
                    }
                    dataOut += speedData;
                }

                if (dataObject.GetType() == typeof(Food))
                {
                    dataOut = "FOOD";
                }
            }

            return dataOut;
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            
        }
    }
}
