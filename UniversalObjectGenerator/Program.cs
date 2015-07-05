// <author>Anna Witwicka</author>
// <date>14/03/2015</date>
// <summary>Class representing a testing console program.</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalObjectGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClass value;
            for (int i = 0; i < 1; i++)
            {
                value = ObjectGenerator.Generate<TestClass>(new ObjectGeneratorRanges() { MaxNumber = 355, MinNumber = 0 });
                Console.Write(value + " ");
            }
            Console.ReadKey();
        }
    }
}
