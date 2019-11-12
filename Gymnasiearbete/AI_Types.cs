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
        public bool DEBUGNOMEMORYSAVE = true;

        public string family;
        string dataPath;
        List<Memory> memory = new List<Memory>();
        public Memory lastMemory;
        public GameObject lastIntresst;
        protected string[] choises = new string[0];

        //public int preformancepoints = 0;

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

        readonly static char z = '|';

        protected Random r;

        public AI(Cell parent, Cell cell)
        {
            r = Game1.random;
            if (parent != null && cell != null)
            {
                idleDestination = new P(cell.Position);
                direction = -parent.AI.Direction;
                family = parent.AI.family;
            }

            
            if (!DEBUGNOMEMORYSAVE)
            {
                MemoryFileLoad();
            }
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
            if (DEBUGNOMEMORYSAVE)
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
        /// <param name="type"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        protected string MemoryChoice(string situation)
        {
            while (true)
            {
                List<Memory> memoryImportant = new List<Memory>();
                foreach (Memory m in memory)
                {
                    if (m.Situation == situation)
                    {
                        memoryImportant.Add(m);
                    }
                }

                List<Memory> memoryChoice = new List<Memory>();
                if (memoryImportant.Count > 0)
                {
                    double curiosity = 1;
                    if (r.Next(100) < 100 - curiosity)
                    {
                        foreach (Memory m in memoryImportant)
                        {
                            if (memoryChoice.Count < 1)
                            {
                                memoryChoice.Add(m);
                            }

                            if (memoryChoice[0].Points < m.Points)
                            {
                                memoryChoice = new List<Memory>();
                                memoryChoice.Add(m);
                            }
                            else
                            if (memoryChoice[0].Points == m.Points)
                            {
                                memoryChoice.Add(m);
                            }

                            int memoryChoiseIndex = r.Next(memoryChoice.Count);

                            lastMemory = memoryChoice[memoryChoiseIndex];
                            return lastMemory.Desicion;
                        }
                    }
                    else
                    {
                        Memory mem = new Memory(situation, 0.ToString(), choises[r.Next(choises.Length)]);
                        for (int i = 0; i < memory.Count; i++)
                        {
                            if (memory[i].Situation == mem.Situation)
                            {
                                if (memory[i].Desicion == mem.Desicion)
                                {
                                    lastMemory = mem;
                                    return lastMemory.Desicion;
                                }   
                            }
                        }
                        memory.Add(mem);
                        lastMemory = mem;
                        return lastMemory.Desicion;
                    }
                }
                else
                {
                    Memory mem = new Memory(situation, 0.ToString(), choises[r.Next(choises.Length)]);
                    memory.Add(mem);
                    lastMemory = mem;
                    return lastMemory.Desicion;
                }
            }
        }

        //TILDELA POÄNG FÖR VAL
        public void MemoryReward(int reward, bool lastT_allF = true)
        {
            foreach (Memory m in memory)
            {
                if (lastT_allF)
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
                    m.Points += reward;
                }
            }
        }

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

        public static AI GetAI(AIType type, Cell parent, Cell cell)
        {
            switch (type)
            {
                case AIType.NoBrain:
                default:
                    //NoBrain
                    break;

                case AIType.CloseTargeting:
                    return new AI_ClosestTargetingLearn(parent, cell);

                case AIType.Player:
                    return new Player();
            }
            return new AI_NoBrain();
        }

        public enum AIType
        {
            NoBrain,
            CloseTargeting,
            Player
        }

        public void AIR(Cell cell, List<GameObject> percivableObjects)
        {
            Intresst(cell, percivableObjects);
        }

        /// <summary>
        /// Väljer vad som är intressant för AI
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="percivableObjects"></param>
        /// <returns></returns>
        protected virtual void Intresst(Cell cell, List<GameObject> percivableObjects)
        {
            GameObject intresst = null;

            lastIntresst = intresst;
            Decision(cell, intresst);
        }

        /// <summary>
        /// Väljer val utifrån intresse
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="intresst"></param>
        /// <returns></returns>
        protected virtual void Decision(Cell cell, GameObject intresst)
        {
            Actions(cell, "DEFAULT", new int[2] { 0, 0 });
        }

        /// <summary>
        /// definerar vad valen innebär
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="decision"></param>
        protected virtual void Actions(Cell cell, string decision, int[] parameters)
        {
            direction = new Vector2(parameters[0], parameters[1]);
            cell.Move(direction);
        }
    }

    class AI_NoBrain : AI
    {
        public AI_NoBrain() : base(null, null)
        {
            choises = new string[0];
            lastMemory = new Memory("NULL", "0", "NULL");
            lastIntresst = null;
        }
    }

    class Player : AI
    {
        public Player() : base(null, null)
        {
            choises = new string[] { "W", "A", "S", "D", "WA", "WD", "SA", "SD" };
            direction = new Vector2(1, 1);
            lastMemory = new Memory("PLAYER", "0", "PLAYER");
        }

        protected override void Intresst(Cell cell, List<GameObject> percivableObjects)
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

            if (dir != Vector2.Zero)
            {
                dir.Normalize();
            }

            Actions(cell, null, new int[] { (int)dir.X, (int)dir.Y });
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            cell.Move(new Vector2(parameters[0], parameters[1]));
        }
    }

    class AI_ClosestTargetingLearn : AI
    {
        public AI_ClosestTargetingLearn(Cell parent, Cell cell) : base(parent, cell)
        {
            choises = new string[] { "MOVETO", "MOVEFROM", "IDLE"};
        }

        protected override void Intresst(Cell cell, List<GameObject> percivableObjects)
        {
            GameObject intresst = null;
            if (percivableObjects.Count > 0)
            {
                intresst = percivableObjects[0];

                foreach (GameObject g in percivableObjects)
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

            lastIntresst = intresst;
            Decision(cell, intresst);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            string data = "NULL";
            if (intresst == null)
            {
                intresst = idleDestination;
            }

            if (intresst != null)
            {
                data = intresst.GetType().ToString().ToUpper().Split('.')[1];
                direction = intresst.Position - cell.Position;

                if (intresst.GetType() == typeof(Cell))
                {
                    Cell c = (Cell)intresst;
                    if (c.Size * Cell.consumeScale < cell.Size)
                    {
                        data += "<";
                    }

                    if (cell.Size * Cell.consumeScale < c.Size)
                    {
                        data += ">";
                    }

                    if (cell.Speed > c.Speed)
                    {
                        data += "*";
                    }
                }
            }

            if (Math.Pow(idleDestination.Position.X - cell.Position.X, 2) + Math.Pow(idleDestination.Position.Y - cell.Position.Y, 2) < 5)
            {
                idleDestination.SetPosition(new Vector2(
                    r.Next((int)Math.Floor(cell.Position.X - cell.Detectionrange), (int)Math.Floor(cell.Position.X + cell.Detectionrange)),
                    r.Next((int)Math.Floor(cell.Position.Y - cell.Detectionrange), (int)Math.Floor(cell.Position.Y + cell.Detectionrange))
                    ));
            }

            Actions(cell, MemoryChoice(data), new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            Vector2 moveDirection = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    cell.Move(idleDestination.Position - cell.Position);
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
