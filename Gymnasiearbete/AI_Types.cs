using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Gymnasiearbete
{
    /// <summary>
    /// Grundklass för all AI
    /// </summary>
    class AI
    {
        public string family;
        string dataPath;

        List<Memory> memory = new List<Memory>();
        public Memory lastMemory;
        public GameObject lastIntresst;
        protected string[] choises = new string[0];

        protected AIType aiType;
        public AIType GetAIType
        {
            get
            {
                return aiType;
            }
        }

        protected Vector2 direction;
        public Vector2 Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }

        protected P idleDestination;
        protected int idleTimer = 0;

        readonly static char z = '|';

        protected Random r;

        public AI(Cell parent, Cell cell)
        {
            r = StaticGlobal.Random;
            if (parent != null && cell != null)
            {
                idleDestination = new P(cell.Position);
                direction = -parent.AI.Direction;
                direction.Normalize();
                family = parent.AI.family;
            }

            
            if (!AI_Controlls.NoMemorySave)
            {
                MemoryFileLoad();
            }

            //aiType = AIType.NoBrain;
        }

        /// <summary>
        /// Kollar om rätt .memory fil finns
        /// </summary>
        public void MemoryFileExsist()
        {
            dataPath = this.GetType().ToString().Split('.')[1] + "_" + family + ".memory";
            if (!File.Exists(dataPath))
            {
                var file = File.Create(dataPath);
                file.Close();
            }
        }

        /// <summary>
        /// Laddar .memory fil till minne
        /// </summary>
        public void MemoryFileLoad()
        {
            MemoryFileExsist();
            foreach (string line in File.ReadAllLines(dataPath))
            {
                memory.Add(new Memory(line.Split(z)[0], line.Split(z)[1], line.Split(z)[2]));
            }
        }

        /// <summary>
        /// skriver till .memory fil från minne
        /// </summary>
        public void MemoryFileWrite()
        {
            if (AI_Controlls.NoMemorySave)
            {
                return;
            }

            MemoryFileExsist();
            List<Memory> newMemory = new List<Memory>();
            List<string> temp = new List<string>();
            temp.AddRange(File.ReadAllLines(dataPath));

            foreach (string s in temp)
            {
                newMemory.Add(new Memory(s.Split(z)[0], s.Split(z)[1], s.Split(z)[2]));
            }
            

            for (int j = 0; j < memory.Count; j++)
            {
                bool exsists = false;
                for (int i = 0; i < newMemory.Count; i++)
                {
                    if (newMemory[i].Situation == memory[j].Situation)
                    {
                        if (newMemory[i].Desicion == memory[j].Desicion)
                        {
                            exsists = true;
                            newMemory[i].Points = Math.Max(newMemory[i].Points, memory[j].Points);
                        }
                    }
                }

                if (!exsists)
                {
                    newMemory.Add(memory[j]);
                }
            }

            temp = new List<string>();
            for (int i = 0; i < newMemory.Count; i++)
            {
                temp.Add(newMemory[i].ToString());
            }
            temp.Sort();
            File.WriteAllLines(dataPath, temp);
        }

        /// <summary>
        /// Gör val utifrån minne beroende på typ och risk att bli uppäten
        /// </summary>
        protected Memory MemoryChoice(string situation)
        {
            while (true)
            {
                //Kollar efter minnen vilka är relevanta till situationen
                List<Memory> memoryImportant = new List<Memory>();
                foreach (Memory m in memory)
                {
                    if (m.Situation == situation)
                    {
                        memoryImportant.Add(m);
                    }
                }

                //Välja bästa minnet, med chans för mutation
                List<Memory> memoryChoice = new List<Memory>();
                double curiosity = 1;
                if (memoryImportant.Count > 0 && r.Next(100) < 100 - curiosity)
                {
                    //Jämför minnespoäng
                    memoryChoice.Add(memoryImportant[0]);
                    foreach (Memory m in memoryImportant)
                    {
                        if (m.Points > memoryChoice[0].Points)
                        {
                            memoryChoice.Clear();
                            memoryChoice.Add(m);
                        }
                        else
                        if (m.Points == memoryChoice[0].Points)
                        {
                            memoryChoice.Add(m);
                        }

                        //Väljer slumpvalt utifrån de bästa minnena
                        lastMemory = memoryChoice[r.Next(memoryChoice.Count)];
                        return lastMemory;
                    }
                }
                else
                {
                    //skapar ett minne som svar på att inga minnen för situationen finns eller mutation
                    Memory mem = new Memory(situation, 0.ToString(), choises[r.Next(choises.Length)]);

                    //I fallet att det är mutation, undersöks det om minnet redan finns
                    for (int i = 0; i < memory.Count; i++)
                    {
                        if (memory[i].Situation == mem.Situation)
                        {
                            if (memory[i].Desicion == mem.Desicion)
                            {
                                lastMemory = memory[i];
                                return lastMemory;
                            }
                        }
                    }

                    //I fallet för att det inte finns minnen för situationen läggs det till
                    memory.Add(mem);
                    lastMemory = mem;
                    return lastMemory;
                }
            }
        }

        /// <summary>
        /// TILDELA POÄNG FÖR VAL. 
        /// amplyfy ökar poäng för bra val och minskar poäng för dåliga val
        /// </summary>
        public void MemoryReward(int reward, bool amplify = false)
        {
            foreach (Memory m in memory)
            {
                if (!amplify)
                {
                    if (m.Situation == lastMemory.Situation)
                    {
                        if (m.Desicion == lastMemory.Desicion)
                        {
                            m.Points += reward;
                        }
                    }
                }
                else 
                {
                    if (m.Points > 0)
                    {
                        m.Points += reward;
                    } else
                    {
                        m.Points -= reward;
                    }
                    
                }
            }
        }

        //Standardiserat format för relevant data
        public string Data(Cell cell, GameObject dataObject)
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
                    
                    if(dataCell.Size * Cell.consumeScale < cell.Size)
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

        //Striver ut relevant data för debug
        public string DEBIUG()
        {
            string result = "";
            result += "[" + family + "]";
            result += "[" + lastMemory.ToString() + "]";
            foreach (Memory m in memory)
            {
                result += "[" + m.ToString() + "]";
            }
            return "{" + result + "}";
        }

        //Retunerar AI för initiering av ny celler
        public static AI GetAI(AIType type, Cell parent, Cell cell)
        {
            switch (type)
            {
                case AIType.NoBrain:
                default:
                    //NoBrain
                    break;

                case AIType.CloseTargeting:
                    return new AI_ClosestTargeting(parent, cell);

                case AIType.PointsTargeting:
                    return new AI_PointsTargeting(parent, cell);

                case AIType.Player:
                    return new Player();
            }
            return new AI_NoBrain();
        }

        //Enum för de olika aityperna
        public enum AIType
        {
            NoBrain,
            CloseTargeting,
            PointsTargeting,
            Player
        }

        /// <summary>
        /// Standardiserad utgångspunkt för AI Processen (AI-Run (AIR))
        /// </summary>
        public void AIR(Cell cell, SectorContent percivableObjects)
        {
            Intresst(cell, percivableObjects);
        }

        /// <summary>
        /// Väljer vad som är intressant för AI
        /// </summary>
        protected virtual void Intresst(Cell cell, SectorContent percivableObjects)
        {
            GameObject intresst = null;

            lastIntresst = intresst;
            Decision(cell, intresst);
        }

        /// <summary>
        /// Väljer val utifrån intresse
        /// </summary>
        protected virtual void Decision(Cell cell, GameObject intresst)
        {
            Actions(cell, "DEFAULT", new int[2] { 0, 0 });
        }

        /// <summary>
        /// Definerar vad valen innebär
        /// </summary>
        protected virtual void Actions(Cell cell, string decision, int[] parameters)
        {
            direction = new Vector2(parameters[0], parameters[1]);
            cell.Move(direction);
        }
    }

    /// <summary>
    /// AI-Typ vilken inte tänker
    /// </summary>
    class AI_NoBrain : AI
    {
        public AI_NoBrain() : base(null, null)
        {
            aiType = AIType.NoBrain;
            choises = new string[0];
            lastMemory = new Memory("NULL", "0", "NULL");
            lastIntresst = null;
        }
    }

    /// <summary>
    /// "AI-Typ" vilken kan kontrolleras av spelare
    /// </summary>
    class Player : AI
    {
        public Player() : base(null, null)
        {
            aiType = AIType.Player;
            choises = new string[] { "W", "A", "S", "D", "WA", "WD", "SA", "SD" };
            direction = new Vector2(1, 1);
            lastMemory = new Memory("PLAYER", "0", "PLAYER");
        }

        protected override void Intresst(Cell cell, SectorContent percivableObjects)
        {
            Decision(cell, null);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            Vector2 dir = Vector2.Zero;

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                dir += new Vector2(0, -1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                dir += new Vector2(-1, 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                dir += new Vector2(1, 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                dir += new Vector2(0, 1);
            }

            /*
            if (dir != Vector2.Zero)
            {
                dir.Normalize();
            }
            */
            Actions(cell, null, new int[] { (int)dir.X, (int)dir.Y });
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            cell.Move(new Vector2(parameters[0], parameters[1]));
        }
    }

    /// <summary>
    /// AI-Typ vilken endast bryr sig om vad som är närmast den
    /// </summary>
    class AI_ClosestTargeting : AI
    {
        public AI_ClosestTargeting(Cell parent, Cell cell) : base(parent, cell)
        {
            aiType = AIType.CloseTargeting;
            choises = new string[] { "MOVETO", "MOVEFROM", "IDLE"};
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

        protected override void Decision(Cell cell, GameObject intresst)
        {
            //Om AI har nått-, är för långt ifrån- eller inte hunnit till idle positionen, sätts en ny idle position.
            if (Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) < Math.Pow(2, 2) || 
                Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) > Math.Pow(2 * cell.Detectionrange, 2) ||
                --idleTimer <= 0)
            {
                idleTimer = (int)Math.Floor(cell.Detectionrange/cell.Speed);
                idleDestination.SetPosition(new Vector2(
                    r.Next((int)Math.Floor(cell.Position.X - 2 * cell.Detectionrange), (int)Math.Floor(cell.Position.X + 2 * cell.Detectionrange)),
                    r.Next((int)Math.Floor(cell.Position.Y - 2 * cell.Detectionrange), (int)Math.Floor(cell.Position.Y + 2 * cell.Detectionrange))
                    ));
            }

            direction = intresst.Position - cell.Position;

            Actions(cell, MemoryChoice(Data(cell, intresst)).Desicion, new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });

        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            //Utför val
            Vector2 moveDirection = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    cell.Move(moveDirection);
                    break;

                case "MOVETO":
                    cell.Move(moveDirection);
                    break;

                case "MOVEFROM":
                    cell.Move(-moveDirection);
                    break;
            }
        }
    }

    /// <summary>
    /// AI-Typ vilken bryr sig om situationer med mest poäng
    /// </summary>
    class AI_PointsTargeting : AI
    {
        public AI_PointsTargeting(Cell parent, Cell cell) : base(parent, cell)
        {
            aiType = AIType.PointsTargeting;
            choises = new string[] { "MOVETO", "MOVEFROM", "IDLE" };
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
            if(bestIntrest.Count <= 0)
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
            Decision(cell, bestIntrest[StaticGlobal.Random.Next(bestIntrest.Count)]);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            //Om AI har nått-, är för långt ifrån- eller inte hunnit till idle positionen, sätts en ny idle position.
            if (Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) < Math.Pow(2, 2) ||
                Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) > Math.Pow(2 * cell.Detectionrange, 2) ||
                --idleTimer <= 0)
            {
                idleTimer = (int)Math.Floor(cell.Detectionrange/cell.Speed);
                idleDestination.SetPosition(new Vector2(
                    r.Next((int)Math.Floor(cell.Position.X - 2 * cell.Detectionrange), (int)Math.Floor(cell.Position.X + 2 * cell.Detectionrange)),
                    r.Next((int)Math.Floor(cell.Position.Y - 2 * cell.Detectionrange), (int)Math.Floor(cell.Position.Y + 2 * cell.Detectionrange))
                    ));
            }

            direction = intresst.Position - cell.Position;

            Actions(cell, MemoryChoice(Data(cell, intresst)).Desicion, new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });

        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            //Utför val
            Vector2 moveDirection = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    cell.Move(moveDirection);
                    break;

                case "MOVETO":
                    cell.Move(moveDirection);
                    break;

                case "MOVEFROM":
                    cell.Move(-moveDirection);
                    break;
            }
        }
    }
}
