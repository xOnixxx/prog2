using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static int LeftIx(int i)
        {
            return i * 2 + 1;
        }

        static int RightIx(int i)
        {
            return i * 2 + 2;
        }

        static int ParentIx(int i)
        {
            return (i - 1) / 2;
        }

        static void BubbleDown(List<int> list, int begin, int end)
        {
            int ix = begin;
            while (LeftIx(ix) < end)
            {
                int l = LeftIx(ix);
                int r = RightIx(ix);

                int swap = ix;
                if (list[l] > list[swap])
                {
                    swap = l;
                }
                if (r < end && list[r] > list[swap])
                {
                    swap = r;
                }

                if (swap != ix)
                {
                    (list[swap], list[ix]) = (list[ix], list[swap]);
                    ix = swap;
                }
                else
                {
                    break;
                }
            }
        }

        static void Heapify(List<int> list)
        {
            for (int i = ParentIx(list.Count - 1); i >= 0; i--)
            {
                BubbleDown(list, i, list.Count);
            }
        }

        static void Heapsort(List<int> list)
        {
            Heapify(list);
            for (int i = list.Count - 1; i > 0; i--)
            {
                (list[i], list[0]) = (list[0], list[i]);
                BubbleDown(list, 0, i);
            }
        }

        static void Main(string[] args)
        {
            char[] delims = null;
            string[] array = Console.ReadLine().Split(delims, StringSplitOptions.RemoveEmptyEntries);

            var numbers = new List<int>();
            foreach (string s in array)
            {
                numbers.Add(int.Parse(s));
            }

            Heapsort(numbers);
            Console.WriteLine("[" + string.Join(",", numbers) + "]");
        }
    }
}
