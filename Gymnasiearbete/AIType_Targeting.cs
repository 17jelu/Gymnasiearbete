using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gymnasiearbete
{
    class BaseTargetingAI : AI
    {
        public BaseTargetingAI(Cell parent, Cell cell) : base(parent, cell)
        {
            choises = new string[] { "MOVETO", "MOVEFROM", "IDLE", "STOP" };
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            //Om AI har nått-, är för långt ifrån- eller inte hunnit till idle positionen, sätts en ny idle position.
            if (Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) < Math.Pow(2, 2) ||
                Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) > Math.Pow(2 * cell.Detectionrange, 2) ||
                --idleTimer <= 0)
            {
                idleTimer = (int)Math.Floor(cell.Detectionrange / cell.Speed * 10);
                double angle = StaticGlobal.Random.NextDouble() * 2 * Math.PI;
                idleDestination.SetPosition(new Vector2(
                    (float)(cell.Position.X + cell.Detectionrange * Math.Cos(angle)),
                    (float)(cell.Position.Y + cell.Detectionrange * Math.Sin(angle))
                    ));
            }

            direction = intresst.Position - cell.Position;

            Actions(cell, MemoryChoice(Data(cell, intresst)).Desicion, new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            Vector2 moveDirection = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    cell.EnergyManagement(-(
                cell.Size / CellManagerControlls.DefaultCellSize +
                cell.Speed / CellManagerControlls.DefaultCellSpeed +
                (cell.Detectionrange - cell.Size) / CellManagerControlls.DefaultCellPerception
                ) / 3);
                    cell.Move(moveDirection);
                    break;

                case "MOVETO":
                    cell.EnergyManagement(-(
                cell.Size / CellManagerControlls.DefaultCellSize +
                cell.Speed / CellManagerControlls.DefaultCellSpeed +
                (cell.Detectionrange - cell.Size) / CellManagerControlls.DefaultCellPerception
                ) / 3);
                    cell.Move(moveDirection);
                    break;

                case "MOVEFROM":
                    cell.EnergyManagement(-(
                cell.Size / CellManagerControlls.DefaultCellSize +
                cell.Speed / CellManagerControlls.DefaultCellSpeed +
                (cell.Detectionrange - cell.Size) / CellManagerControlls.DefaultCellPerception
                ) / 3);
                    cell.Move(-moveDirection);
                    break;

                case "STOP":
                    cell.EnergyManagement(-(
                        (cell.Detectionrange - cell.Size) / CellManagerControlls.DefaultCellPerception)
                        );
                    break;
            }
        }
    }

    /// <summary>
    /// AI-Typ vilken endast bryr sig om vad som är närmast den
    /// </summary>
    class AI_ClosestTargeting : BaseTargetingAI
    {
        public AI_ClosestTargeting(Cell parent, Cell cell) : base(parent, cell)
        {
            aiType = AIType.TargetingClose;
            //choises = new string[] { "MOVETO", "MOVEFROM", "IDLE", "STOP" };
        }

        protected override void Intresst(Cell cell, SectorContent percivableObjects)
        {
            GameObject intresst = null;
            if (percivableObjects.All.Count > 0)
            {
                intresst = percivableObjects.All[0];

                //gör intresset till det närmaste objectet av saker AI kan se
                foreach (GameObject g in percivableObjects.All)
                {
                    if (
                        Math.Pow(g.Position.X - cell.Position.X, 2) + Math.Pow(g.Position.Y - cell.Position.Y, 2) <=
                        Math.Pow(intresst.Position.X - cell.Position.X, 2) + Math.Pow(intresst.Position.Y - cell.Position.Y, 2)
                        )
                    {
                        intresst = g;
                    }
                }
            }

            if (intresst == null)
            {
                intresst = idleDestination;
            }
            lastIntresst = intresst;
            Decision(cell, lastIntresst);
        }
    }

    /// <summary>
    /// AI-Typ vilken bryr sig om situationer med mest poäng
    /// </summary>
    class AI_PointsTargeting : BaseTargetingAI
    {
        public AI_PointsTargeting(Cell parent, Cell cell) : base(parent, cell)
        {
            aiType = AIType.TargetingPoints;
            //choises = new string[] { "MOVETO", "MOVEFROM", "IDLE", "STOP"};
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

        protected override void Intresst(Cell cell, SectorContent percivableObjects)
        {
            //Samlar val med mest poäng
            List<GameObject> bestIntrest = new List<GameObject>();
            if (percivableObjects.All.Count > 0)
            {
                bestIntrest.Add(percivableObjects.All[0]);
                foreach (GameObject g in percivableObjects.All)
                {
                    if (MemoryChoice(Data(cell, g)).Points > MemoryChoice(Data(cell, bestIntrest[0])).Points)
                    {
                        bestIntrest.Clear();
                        bestIntrest.Add(g);
                    }
                    else
                    if (MemoryChoice(Data(cell, g)).Points == MemoryChoice(Data(cell, bestIntrest[0])).Points)
                    {
                        bestIntrest.Add(g);
                    }
                }
            }

            //Om det inte finns minnen
            if (bestIntrest.Count <= 0)
            {
                bestIntrest.Add(idleDestination);
            }

            //För att hindra obeslutsamhet ska den fortsätta göra det den val att göra från början
            if (bestIntrest.Contains(lastIntresst))
            {
                bestIntrest.Clear();
                bestIntrest.Add(lastIntresst);
            }

            //Väljer slumpvalt val utifrån val med mest poäng
            lastIntresst = bestIntrest[StaticGlobal.Random.Next(bestIntrest.Count)];
            Decision(cell, lastIntresst);
        }
    }
}
