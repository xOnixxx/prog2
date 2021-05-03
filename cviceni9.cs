using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ConsoleApp1
{
    class EggDropperTD
    {
        private Dictionary<(int, int), int> memoized = new Dictionary<(int, int), int>();

        public int MinAttempts(int floors, int eggs, bool writeFloor = false)
        {
            if (floors == 0) return 0;
            if (eggs == 0) return int.MaxValue;
            if (floors == 1) return 1;
            if (eggs == 1) return floors;

            var key = (floors, eggs);
            if (memoized.ContainsKey(key))
            {
                return memoized[key];
            }

            int result = int.MaxValue;
            int bestFloor = -1;
            for (int floor = 1; floor <= floors; floor++)
            {
                int candidate = Math.Max(MinAttempts(floor - 1, eggs - 1), MinAttempts(floors - floor, eggs));
                if (candidate < result)
                {
                    result = candidate;
                    bestFloor = floor;
                }
            }

            if (writeFloor)
            {
                Console.WriteLine($"Best floor: {bestFloor}");
            }

            result++;
            memoized[key] = result;
            return result;
        }
    }

    class EggDropperBU
    {
        public static int MinAttempts(int floors, int eggs)
        {
            int[,] best = new int[floors + 1, eggs + 1];

            for (int i = 0; i < eggs + 1; i++)
            {
                best[0, i] = 0;
                best[1, i] = 1;
            }

            for (int i = 2; i < floors + 1; i++)
            {
                best[i, 0] = int.MaxValue;
                best[i, 1] = i;
            }

            for (int i = 2; i < eggs + 1; i++)
            {
                for (int j = 2; j < floors + 1; j++)
                {
                    int result = int.MaxValue;
                    for (int floor = 1; floor <= j; floor++)
                    {
                        int candidate = Math.Max(best[floor - 1, i - 1], best[j - floor, i]);
                        if (candidate < result) result = candidate;
                    }
                    best[j, i] = result + 1;
                }
            }

            return best[floors, eggs];
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new EggDropperTD().MinAttempts(1000, 10));
            Console.WriteLine(EggDropperBU.MinAttempts(1000, 10));
        }
    }
}
