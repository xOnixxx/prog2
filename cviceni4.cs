using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Z7
    {
        private int value;

        public Z7(int value)
        {
            this.value = value % 7;
            if (this.value < 0)
            {
                this.value += 7;
            }
        }

        public static Z7 operator +(Z7 a, Z7 b)
        {
            return new Z7(a.value + b.value);
        }

        public static Z7 operator -(Z7 a, Z7 b)
        {
            return new Z7(a.value - b.value);
        }

        public static Z7 operator *(Z7 a, Z7 b)
        {
            return new Z7(a.value * b.value);
        }

        public static bool operator <(Z7 a, Z7 b)
        {
            return a.value < b.value;
        }

        public static bool operator >(Z7 a, Z7 b)
        {
            return b < a;
        }

        public static bool operator ==(Z7 a, Z7 b)
        {
            return a.value == b.value;
        }

        public static bool operator !=(Z7 a, Z7 b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Z7 z && value == z.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Write manually line by line.
            using (var f = File.CreateText(@"data.txt"))
            {
                f.WriteLine("hello world");
            }

            // Write all lines at once.
            File.WriteAllLines(@"data.txt", new string[] { "hello", "world" });

            // Write entire file at once.
            File.WriteAllText(@"data.txt", "hello\nworld\n");

            // Read manually line by line.
            using (var f = File.OpenText(@"data.txt"))
            {
                string line = f.ReadLine();

                while (line != null)
                {
                    Console.WriteLine(line);
                    line = f.ReadLine();
                }
            }

            // Read all lines at once.
            var allLines = File.ReadAllLines(@"data.txt");
            foreach (var line in allLines)
            {
                Console.WriteLine(line);
            }

            // Read entire file at once.
            Console.WriteLine(File.ReadAllText(@"data.txt"));
            
            // Read all lines lazily.
            IEnumerable<string> lines = File.ReadLines(@"data.txt");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }

            // Binary input/output
            using (var f = File.OpenWrite(@"data.bin"))
            {
                f.WriteByte(0xff);
            }

            using (var f = File.OpenRead(@"data.bin"))
            {
                Console.WriteLine(f.ReadByte());
            }
        }
    }
}
