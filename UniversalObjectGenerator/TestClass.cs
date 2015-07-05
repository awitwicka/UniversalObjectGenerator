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
    public class TestClass
    {

        public int numberInt1;
        public int numberInt2;
        public string textString1;

        public char propertyChar { get; set; }
        public bool propertyBool { get; set; }
        public int propertyInt {get; set;}
        public SubClass nestedObject { get; set; }
        public string propertyString { get; set; }
        public TestClass obj {get; set;}
        public HashSet<TestClass> list { get; set; }

        public TestClass() {}
        public TestClass(int number1, int number2, string text1)
        {
            this.numberInt1 = number1;
            this.numberInt2 = number2;
            this.textString1 = text1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("***TestClass***");
            sb.AppendLine("int1: " + numberInt1 + " int2: " + numberInt2 + " string: " + textString1);
            sb.AppendLine("prop_int: " + propertyInt + " prop_string: " + propertyString + " prop_char: " + propertyChar + " prop_bool " + propertyBool);
            sb.AppendLine("prop_list: " + list);
            if (nestedObject != null)
                sb.AppendLine(nestedObject.ToString());
            if (obj != null)    
                sb.AppendLine(obj.ToString());
            return sb.ToString();
        }
    }
}
