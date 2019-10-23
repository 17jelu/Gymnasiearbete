using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
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
        List<string[]> memory = new List<string[]>();
        protected string[] choises = new string[0];

        public int preformancepoints = 0;

        public Vector2 idleDirection;

        readonly static char z = '|';

        Random r;

        public AI(Random random, string familySet)
        {
            r = random;

            int[] iDir = new int[] { -1, 1 };
            idleDirection = new Vector2(iDir[r.Next(iDir.Length)], iDir[r.Next(iDir.Length)]);
            family = familySet;
            MemoryFileLoad();
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
                memory.Add(line.Split(z));
            }
        }

        /// <summary>
        /// skriver till .memory fil från minne
        /// </summary>
        public void MemoryFileWrite()
        {
            MemoryFileExsist();
            List<string> newMemory = new List<string>();
            newMemory.AddRange(File.ReadAllLines(dataPath));

            for (int j = 0; j < memory.Count; j++)
            {
                bool replace = false;
                for (int i = 0; i < newMemory.Count; i++)
                {
                    if (newMemory[i].Split(z)[0] == memory[j][0])
                    {
                        replace = true;
                        memory[j][0].ToString();
                        //memory[j][1].ToString();
                        memory[j][2].ToString();
                        if (int.Parse(newMemory[i].Split(z)[1]) < int.Parse(memory[j][1]) && int.Parse(memory[j][1]) > 0)
                        {
                            newMemory[i] = memory[j][0] + z + memory[j][1] + z + memory[j][2];
                        }
                    }
                }

                if (!replace)
                {
                    newMemory.Add(memory[j][0] + z + memory[j][1] + z + memory[j][2]);
                }
            }

            File.WriteAllLines(dataPath, newMemory);
        }

        /// <summary>
        /// Gör val utifrån minne beroende på typ och risk att bli uppäten
        /// </summary>
        /// <param name="type"></param>
        /// <param name="consume"></param>
        /// <returns></returns>
        protected string MemoryChoice(string situation)
        {
            int breakLoopTimer = 10;
            while (breakLoopTimer > 0)
            {
                breakLoopTimer--;
                List<string[]> memoryImportant = new List<string[]>();
                foreach (string[] mem in memory)
                {
                    if (mem[0] == situation)
                    {
                        memoryImportant.Add(mem);
                    }
                }

                List<string[]> memoryChoice = new List<string[]>();
                if (memoryImportant.Count > 0)
                {
                    double curiosity = 0.5;
                    if (r.Next(100) < 100 - curiosity)
                    {
                        foreach (string[] s in memoryImportant)
                        {
                            if (memoryChoice.Count < 1)
                            {
                                memoryChoice.Add(s);
                            }

                            if (int.Parse(memoryChoice[0][1]) < Math.Max(0, int.Parse(s[1])))
                            {
                                memoryChoice = new List<string[]>();
                                memoryChoice.Add(s);
                            }
                            else
                            if (int.Parse(memoryChoice[0][1]) == int.Parse(s[1]))
                            {
                                memoryChoice.Add(s);
                            }

                            int memoryChoiseIndex = r.Next(memoryChoice.Count);
                            foreach (string[] sp in memory)
                            {
                                if (sp[0] == memoryChoice[memoryChoiseIndex][0])
                                {
                                    sp[1] = preformancepoints.ToString();
                                }
                            }

                            return memoryChoice[memoryChoiseIndex][2];
                        }
                    }
                    else
                    {
                        string[] str = new string[3] { situation, preformancepoints.ToString(), choises[r.Next(choises.Length)] };
                        for (int i = 0; i < memory.Count; i++)
                        {
                            if (memory[i][0] == str[0])
                            {
                                memory[i][2] = str[2];
                            }
                        }
                    }
                }
                else
                {
                    string[] str = new string[3] { situation, preformancepoints.ToString(), choises[r.Next(choises.Length)] };
                    memory.Add(str);
                }
            }

            return "";
        }

        public string DEBIÙG()
        {
            string result = "";
            result += "[" + family + "]";
            foreach (string[] s in memory)
            {
                result += "[" + s[0] + z + s[1] + z + s[2] + "]";
            }
            return "{" + result + "}";
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
            Vector2 direction = new Vector2(parameters[0], parameters[1]);
            cell.Move(direction);
        }
    }

    class AI_NoBrain : AI
    {
        public AI_NoBrain(Random random, string familySet) : base(random, familySet)
        {
            choises = new string[0];
        }
    }

    class AI_ClosestTargeting : AI
    {
        public AI_ClosestTargeting(Random random, string familySet) : base(random, familySet)
        {
            choises = new string[3] { "IDLE", "MOVETO", "MOVEFROM" };
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

            Decision(cell, intresst);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            if (intresst == null)
            {
                Actions(cell, choises[0], new int[2] { 0, 0 });
                return;
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.Size > Cell.consumeScale * cell.Size)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    Actions(cell, choises[2], new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                    return;
                }
                else
                if (cell.Size > Cell.consumeScale * intresst.Size && cell.Speed > intresst.Speed)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    Actions(cell, choises[1], new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                    return;
                }
                else
                {
                    Actions(cell, choises[0], new int[2] { 0, 0 });
                    return;
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = intresst.Position - cell.Position;
                Actions(cell, choises[1], new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                return;
            }

            Actions(cell, choises[0], new int[2] { 0, 0 });
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            Vector2 direction = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    cell.Move(idleDirection);
                    break;

                case "MOVETO":
                    cell.Move(direction);
                    break;

                case "MOVEFROM":
                    cell.Move(-direction);
                    break;

            }
        }
    }

    class AI_ClosestTargetingLearn : AI
    {
        public AI_ClosestTargetingLearn(Random random, string familySet) : base(random, familySet)
        {
            choises = new string[3] { "IDLE", "MOVETO", "MOVEFROM" };
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

            Decision(cell, intresst);
        }

        protected override void Decision(Cell cell, GameObject intresst)
        {
            if (intresst == null)
            {
                Actions(cell, MemoryChoice("NULL"), new int[2] { (int)Math.Floor(idleDirection.X), (int)Math.Floor(idleDirection.Y) });
                return;
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.Size > Cell.consumeScale * cell.Size)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    Actions(cell, MemoryChoice("CELL" + "BIG"), new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                    return;
                }
                else
                if (cell.Size > Cell.consumeScale * intresst.Size && cell.Speed > intresst.Speed)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    Actions(cell, MemoryChoice("CELL" + "SMALLSLOW"), new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                    return;
                }
                else
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    Actions(cell, MemoryChoice("CELL"), new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                    return;
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = intresst.Position - cell.Position;
                Actions(cell, MemoryChoice("FOOD"), new int[2] { (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) });
                return;
            }
        }

        protected override void Actions(Cell cell, string decision, int[] parameters)
        {
            Vector2 direction = new Vector2(parameters[0], parameters[1]);
            switch (decision)
            {
                case "IDLE":
                    if (cell.Position.X + cell.Detectionrange / 2 >= CellManager.simulationArea.X + CellManager.simulationArea.Width)
                    {
                        this.idleDirection.X = -1;
                    }

                    if (cell.Position.X - cell.Detectionrange / 2 <= CellManager.simulationArea.X)
                    {
                        this.idleDirection.X = 1;
                    }

                    if (cell.Position.Y + cell.Detectionrange / 2 >= CellManager.simulationArea.Y + CellManager.simulationArea.Height)
                    {
                        this.idleDirection.Y = -1;
                    }

                    if (cell.Position.Y - cell.Detectionrange / 2 <= CellManager.simulationArea.Y)
                    {
                        this.idleDirection.Y = 1;
                    }
                    cell.Move(idleDirection);
                    break;

                case "MOVETO":
                    cell.Move(direction);
                    break;

                case "MOVEFROM":
                    cell.Move(-direction);
                    break;
            }
        }
    }
}
