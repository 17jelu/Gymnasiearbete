using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Gymnasiearbete
{
    /// <summary>
    /// Grundklass för all AI
    /// </summary>
    class AI
    {
        public string family = "";
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

        public AI(Cell parent, Cell cell)
        {
            if (parent != null && cell != null)
            {
                idleDestination = new P(cell.Position);
                direction = -parent.AI.Direction;
                direction.Normalize();
                if (parent.AI.family == null || parent.AI.family == "")
                {
                    family = StaticGlobal.Family.NewFamily();
                }
                else
                {
                    family = parent.AI.family;
                    StaticGlobal.Family.AddMember(family);
                }
            }
            
            MemoryFileLoad();

            aiType = AIType.NoBrain;
        }

        /// <summary>
        /// Ränsar .memory fil
        /// </summary>
        public void MemoryFilePurge()
        {
            MemoryFileExsist();
            string[] empty = new string[0];
            File.WriteAllLines(dataPath, empty);
        }

        /// <summary>
        /// Kollar om rätt .memory fil finns
        /// </summary>
        public void MemoryFileExsist()
        {
            dataPath = "Memories/";
            if (!Directory.Exists(dataPath))
            {
                var directory = Directory.CreateDirectory(dataPath);
            }

            dataPath += this.GetType().ToString().Split('.')[1] + "_" + family.ToString() + ".memory";
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
            if (AIControlls.NoMemorySave)
            {
                return;
            }

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
            if (AIControlls.NoMemorySave)
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
                double curiosity = 0;
                if (memoryImportant.Count > 0 && StaticGlobal.Random.Next(100) < 100 - curiosity)
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
                        lastMemory = memoryChoice[StaticGlobal.Random.Next(memoryChoice.Count)];
                        return lastMemory;
                    }
                }
                else
                {
                    //skapar ett minne som svar på att inga minnen för situationen finns eller mutation
                    Memory mem = new Memory(situation, 0.ToString(), choises[StaticGlobal.Random.Next(choises.Length)]);

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
            if (lastMemory == null)
            {
                return;
            }

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
        public virtual string Data(Cell cell, GameObject dataObject)
        {
            return "";
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

                case AIType.Player:
                    return new Player();

                case AIType.TargetingClose:
                    return new AI_ClosestTargeting(parent, cell);

                case AIType.TargetingPoints:
                    return new AI_PointsTargeting(parent, cell);

                case AIType.AdvancedMovement:
                    return new BaseAdvancedMovementAI(parent, cell);
            }
            return new AI_NoBrain();
        }

        //Enum för de olika aityperna
        public enum AIType
        {
            NoBrain,
            Player,
            TargetingClose,
            TargetingPoints,
            AdvancedMovement,

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

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            cell.EnergyManagement(-10);
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
            choises = new string[0];
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
            Vector2 moveDirection = new Vector2(parameters[0], parameters[1]);
            cell.Move(moveDirection);

            if (moveDirection != Vector2.Zero)
            {
                cell.EnergyManagement(-(
                cell.Size / CellManagerControlls.DefaultCellSize +
                cell.Speed / CellManagerControlls.DefaultCellSpeed +
                (cell.Detectionrange - cell.Size) / CellManagerControlls.DefaultCellPerception
                ) / 3);
            }
            else
            {
                cell.EnergyManagement(-10);
            }
        }
    }

    
}
