// <author>Anna Witwicka</author>
// <date>14/03/2015</date>
// <summary>Class used for testing the ObjectGenerator class.</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalObjectGenerator
{
    public class SubClass
    {
        public int numberInt1;
        public readonly int numberInt2;
        public int numberInt3;

        public int propertyInt { get { return numberInt3; }  }
        public char propertyChar { get; set; }
        public bool propertyBool { get; set; }
        public SubClass(int number1, int number2)
        {
            this.numberInt1 = number1;
            this.numberInt2 = number2;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("***SubClass***");
            sb.AppendLine("int1: " + numberInt1 + " int2: " + numberInt2 + " int3: " + numberInt3);
            sb.AppendLine("prop_char: " + propertyChar + " prop_bool " + propertyBool);
            return sb.ToString();
        }
    }
}
