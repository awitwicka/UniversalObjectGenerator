// <author>Anna Witwicka</author>
// <date>14/03/2015</date>
// <summary>Static class generating objects with randomly generated data.</summary>

using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Text;

namespace UniversalObjectGenerator
{
    public class ObjectGeneratorRanges
    {
        private int minNumber = 1;
        public int MinNumber
        {
            get { return minNumber; }
            set { minNumber = value; }
        }

        private int maxNumber = 100;
        public int MaxNumber
        {
            get { return maxNumber; }
            set { maxNumber = value; }
        }

        private int minStringLength = 1;
        public int MinStringLength
        {
            get { return minStringLength; }
            set { minStringLength = value; }
        }

        private int maxStringLength = 10;
        public int MaxStringLength
        {
            get { return maxStringLength; }
            set { maxStringLength = value; }
        }
    }

    public static class ObjectGenerator
    {
        private static System.Random rnd = new System.Random();
        
        /// <summary>
        /// Limit of the depth of the recursion calls.
        /// </summary>
        public static int recursionLimit = 2;

        /// <summary>
        /// Generates instance of an object of the type T, with all its members filled with random values from ranges defined in argument 'range'.
        /// </summary>
        /// <typeparam name="T">Type of the object to be created.</typeparam>
        /// <param name="ranges">Object constaining minimal and maximal values used during object generation.</param>
        /// <returns>Instance of an object of the type T.</returns>
        public static T Generate<T>(ObjectGeneratorRanges ranges)
        {
            return GenerateRecursive<T>(ranges, 0);
        }

        /// <summary>
        /// Generates instance of an object of the type T, with all its members filled with random values from ranges defined in ObjectGeneratorRanges class.
        /// </summary>
        /// <typeparam name="T">Type of the object to be created.</typeparam>
        /// <returns>Instance of an object of the type T.</returns>
        public static T Generate<T>()
        {
            return GenerateRecursive<T>(new ObjectGeneratorRanges(), 0);
        }

        /// <summary>
        /// Generates instance of an object of the type T, with all its members filled with random values from ranges defined in argument 'range'.
        /// The recursive variable is used to limit the depth of the recursion calls.
        /// </summary>
        /// <typeparam name="T">Type of the object to be created.</typeparam>
        /// <param name="ranges">Object constaining minimal and maximal values used during object generation.</param>
        /// <param name="recursiveVariable">Number describing depth of recursion.</param>
        /// <returns>Instance of an object of the type T.</returns>
        private static T GenerateRecursive<T>(ObjectGeneratorRanges ranges, int recursiveVariable) {
            Type t = typeof(T);
            FieldInfo max = t.GetField("MaxValue");
            FieldInfo min = t.GetField("MinValue");
            int minRand;
            int maxRand;
           
            //IF IS PRIMITIVE
            if (t.IsPrimitive) {
                if (t.Equals(typeof(bool)))
                    return (T) Convert.ChangeType((rnd.Next(0, 2) == 0), t); 
                else if (t.Equals(typeof(char)))
                    return (T) Convert.ChangeType(generateString(1), t); 
                else if (max != null || min != null) {
                    if ((int)min.GetValue(null)<ranges.MaxNumber || (int)max.GetValue(null)>ranges.MinNumber) {
                        minRand = Math.Max(ranges.MinNumber,(int)min.GetValue(null));
                        maxRand = Math.Min(ranges.MaxNumber,(int)max.GetValue(null));
                    } else {
                        minRand = (int)min.GetValue(null);
                        maxRand = (int)max.GetValue(null);
                    }
                    return (T) Convert.ChangeType(rnd.Next(minRand, maxRand+1), t);
                } else
                    throw new NotSupportedException(t.Name);
            } else if (t.Equals(typeof(String)))
                return (T) Convert.ChangeType(generateString(rnd.Next(ranges.MinStringLength, ranges.MaxStringLength + 1)), t);
            //IF IS A CLASS
            else if (t.IsClass)
            {
                object obj = ConstructObject<T>(ranges, recursiveVariable);
                FillProperties<T>(obj, ranges, recursiveVariable);
                return (T)Convert.ChangeType(obj, t);
            } 
            throw new NotSupportedException();
        }

        /// <summary>
        /// Fills properties of an object 'obj' with random data, excluding collections which will be initiated as empty collections.
        /// </summary>
        /// <param name="obj">Object to be filled.</param>
        /// <typeparam name="T">Type of the object to be filled.</typeparam>
        /// <param name="ranges">Object constaining minimal and maximal values used during object generation.</param>
        /// <param name="recursiveVariable">Number describing depth of recursion.</param>
        private static void FillProperties<T>(object obj, ObjectGeneratorRanges ranges, int recursiveVariable)
        {
            Type t = typeof(T);
            //Don't check for the properties if the current type implements ICollection.
            if (!(t.GetInterfaces().Contains(typeof(ICollection))))
            {
                List<PropertyInfo> properties = obj.GetType().GetProperties().ToList();
                foreach (PropertyInfo property in properties)
                {
                    var insideType = property.PropertyType;
                    if (property.SetMethod != null)
                        //If the current type is not a class (excluding string) make one more level of recursion.
                        if (recursiveVariable < recursionLimit + ((!property.PropertyType.IsClass || property.PropertyType.Equals(typeof(string)))? 1 : 0 ))
                        {
                            MethodInfo method = typeof(ObjectGenerator).GetMethod("GenerateRecursive", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                            object value = method.MakeGenericMethod(insideType).Invoke(null, new object[] { ranges, recursiveVariable + 1 });
                            property.SetValue(obj, value, null);
                        }
                }
            } 
        }

        /// <summary>
        /// Constructs instance of an object of type T using the simplest constructor.
        /// </summary>
        /// <typeparam name="T">Type of the object to be constructed.</typeparam>
        /// <param name="ranges">Object constaining minimal and maximal values used during object generation.</param>
        /// <param name="recursiveVariable">Number describing depth of recursion.</param>
        /// <returns>Constructed object.</returns>
        private static object ConstructObject<T>(ObjectGeneratorRanges ranges, int recursiveVariable)
        {
            Type t = typeof(T);
            List<ConstructorInfo> constructors = t.GetConstructors().OrderBy(constr => constr.GetParameters().Length).ToList();
            //Take the constructor of a class with the smallest number of the parameters.
            ConstructorInfo con = constructors[0];
            List<ParameterInfo> parameters = con.GetParameters().ToList();
            List<object> constructorParameters = new List<object>();
            foreach (var param in parameters)
            {
                constructorParameters.Add(
                    typeof(ObjectGenerator).GetMethod("GenerateRecursive", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                        .MakeGenericMethod(param.ParameterType).Invoke(null, new object[] { ranges, recursiveVariable + 1 })
                    );
            }
            object obj = con.Invoke(constructorParameters.ToArray());
            return obj;
        }

        /// <summary>
        /// Generates a string of random letters between A and Z of given size.
        /// </summary>
        /// <param name="size">Size of the random string.</param>
        /// <returns>String of random letters.</returns>
        private static string generateString(int size)
        {
            //Taken from http://stackoverflow.com/questions/1122483/random-string-generator-returning-same-string.
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = (char)rnd.Next('A', 'Z' + 1);
                builder.Append(ch);
            }
            return builder.ToString();
        }
    }

}
