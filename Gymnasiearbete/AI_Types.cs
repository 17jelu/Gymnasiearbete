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
        protected int[] choises = new int[0];

        public int preformancepoints = 0;

        char z = '|';

        Random r;

        public AI(Random random, string familySet)
        {
            r = random;
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
        protected int MemoryChoice(int type, int consume)
        {
            int breakLoopTimer = 10;
            while (breakLoopTimer > 0)
            {
                breakLoopTimer--;
                List<string[]> memoryImportant = new List<string[]>();
                foreach (string[] mem in memory)
                {
                    if (mem[0] == type + "" + consume)
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

                            return int.Parse(memoryChoice[memoryChoiseIndex][2]);
                        }
                    }
                    else
                    {
                        string[] str = new string[3] { type + "" + consume, preformancepoints.ToString(), r.Next(choises.Length).ToString() };
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
                    string[] str = new string[3] { type + "" + consume, preformancepoints.ToString(), r.Next(choises.Length).ToString() };
                    memory.Add(str);
                }
            }

            return -100;
        }

        public int[] AIR(Cell cell, List<GameObject> percivableObjects)
        {
            return Intresst(cell, percivableObjects);
        }

        /// <summary>
        /// Väljer vad som är intressant för AI
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="percivableObjects"></param>
        /// <returns></returns>
        protected virtual int[] Intresst(Cell cell, List<GameObject> percivableObjects)
        {
            GameObject intresst = null;

            return Decision(cell, intresst);
        }

        /// <summary>
        /// Väljer val utifrån intresse
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="intresst"></param>
        /// <returns></returns>
        protected virtual int[] Decision(Cell cell, GameObject intresst)
        {
            return new int[3] { 0, 0, 0 };
        }

        public string Debug_AI()
        {
            return "{TP[" + this.GetType().ToString().Split('.')[1] + "] FA[" + family + "]}";
        }
    }

    class AI_NoBrain : AI
    {
        public AI_NoBrain(Random random, string familySet) : base(random, familySet)
        {
            choises = new int[] { 0 };
        }
    }

    class AI_ClosestTargeting : AI
    {
        public AI_ClosestTargeting(Random random, string familySet) : base(random, familySet)
        {

        }

        protected override int[] Intresst(Cell cell, List<GameObject> percivableObjects)
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

            return Decision(cell, intresst);
        }

        protected override int[] Decision(Cell cell, GameObject intresst)
        {
            if (intresst == null)
            {
                return new int[3] { 0, 0, 0 };
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.Size > Cell.consumeScale * cell.Size)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    return new int[3] { -1, (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                if (cell.Size > Cell.consumeScale * intresst.Size && cell.Speed > intresst.Speed)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    return new int[3] { 1, (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                {
                    return new int[3] { 0, 0, 0 };
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = intresst.Position - cell.Position;
                return new int[3] { 1, (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
            }

            return new int[3] { 0, 0, 0 };
        }
    }

    class AI_ClosestTargetingLearn : AI
    {
        public AI_ClosestTargetingLearn(Random random, string familySet) : base(random, familySet)
        {
            choises = new int[3] { 0, 1, -1 };
        }

        protected override int[] Intresst(Cell cell, List<GameObject> percivableObjects)
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

            return Decision(cell, intresst);
        }

        protected override int[] Decision(Cell cell, GameObject intresst)
        {
            if (intresst == null)
            {
                return new int[3] { MemoryChoice(-1, 0), 0, 0 };
            }
            else
            if (intresst.GetType() == typeof(Cell))
            {
                if (intresst.Size > Cell.consumeScale * cell.Size)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    return new int[3] { MemoryChoice(1, 1), (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                if (cell.Size > Cell.consumeScale * intresst.Size && cell.Speed > intresst.Speed)
                {
                    Vector2 direction = intresst.Position - cell.Position;
                    return new int[3] { MemoryChoice(1, -1), (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                {
                    return new int[3] { MemoryChoice(1, 0), 0, 0 };
                }
            }
            else
            if (intresst.GetType() == typeof(Food))
            {
                Vector2 direction = intresst.Position - cell.Position;
                return new int[3] { MemoryChoice(0, 0), (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
            }

            return new int[0];
        }
    }
}
