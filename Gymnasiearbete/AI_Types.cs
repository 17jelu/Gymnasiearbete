using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;


namespace Gymnasiearbete
{
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

        public void MemoryFileExsist()
        {
            dataPath = this.GetType().ToString().Split('.')[1] + "_" + family + ".memory";
            if (!File.Exists(dataPath))
            {
                var file = File.Create(dataPath);
                file.Close();
            }
        }

        public void MemoryFileLoad()
        {
            MemoryFileExsist();
            foreach (string line in File.ReadAllLines(dataPath))
            {
                memory.Add(line.Split(z));
            }
        }

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
                        if (int.Parse(newMemory[i].Split(z)[1]) < int.Parse(memory[j][1]) && int.Parse(memory[j][1]) > 0)
                        {
                            replace = true;
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
            while (true)
            {
                List<string[]> memoryImportant = new List<string[]>();
                foreach (string[] s in memory)
                {
                    if (s[0] == type + "" + consume)
                    {
                        memoryImportant.Add(s);
                    }
                }

                List<string[]> memoryChoice = new List<string[]>();
                r = new Random();
                int curiosity = 33;
                if (memoryImportant.Count > 0 || r.Next(100) < curiosity)
                {
                    foreach (string[] s in memoryImportant)
                    {
                        if (memoryChoice.Count < 1)
                        {
                            memoryChoice.Add(s);
                        }

                        if (int.Parse(memoryChoice[0][1]) > Math.Max(0, int.Parse(s[1])))
                        {
                            memoryChoice = new List<string[]>();
                            memoryChoice.Add(s);
                        }
                        else
                        if (int.Parse(memoryChoice[0][1]) == int.Parse(s[1]))
                        {
                            memoryChoice.Add(s);
                        }
                        else
                        {
                            memory.Add(new string[3] { type + "" + consume, preformancepoints.ToString(), r.Next(choises.Length).ToString() });
                        }

                        int memoryChoiseIndex = r.Next(memoryChoice.Count);
                        
                        foreach(string[] sp in memory)
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
                    memory.Add(new string[3] { type + "" + consume, preformancepoints.ToString(), r.Next(choises.Length).ToString() });
                }
            }
        }

        public int[] AIR(Cell cell, List<GameObject> percivableObjects)
        {
            return Intresst(cell, percivableObjects);
        }

        protected virtual int[] Intresst(Cell cell, List<GameObject> percivableObjects)
        {
            GameObject intresst = null;

            return Decision(cell, intresst);
        }

        protected virtual int[] Decision(Cell cell, GameObject intresst)
        {
            return new int[3] { 0, 0, 0 };
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
                        Math.Pow(g.position.X - cell.position.X, 2) + Math.Pow(g.position.Y - cell.position.Y, 2) <=
                        Math.Pow(intresst.position.X - cell.position.X, 2) + Math.Pow(intresst.position.Y - cell.position.Y, 2)
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
                if (intresst.size > Cell.consumeScale * cell.size)
                {
                    Vector2 direction = intresst.position - cell.position;
                    return new int[3] { -1, (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                if (cell.size > Cell.consumeScale * intresst.size && cell.speed > intresst.speed)
                {
                    Vector2 direction = intresst.position - cell.position;
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
                Vector2 direction = intresst.position - cell.position;
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
                        Math.Pow(g.position.X - cell.position.X, 2) + Math.Pow(g.position.Y - cell.position.Y, 2) <=
                        Math.Pow(intresst.position.X - cell.position.X, 2) + Math.Pow(intresst.position.Y - cell.position.Y, 2)
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
                if (intresst.size > Cell.consumeScale * cell.size)
                {
                    Vector2 direction = intresst.position - cell.position;
                    return new int[3] { MemoryChoice(1, 1), (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
                }
                else
                if (cell.size > Cell.consumeScale * intresst.size && cell.speed > intresst.speed)
                {
                    Vector2 direction = intresst.position - cell.position;
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
                Vector2 direction = intresst.position - cell.position;
                return new int[3] { MemoryChoice(0, 0), (int)Math.Floor(direction.X), (int)Math.Floor(direction.Y) };
            }

            return new int[0];
        }
    }
}
