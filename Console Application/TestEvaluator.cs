using FormulaEvaluator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Console_Application
{
    class TestEvaluator
    {
        private static void TestRemoveWhiteSpace()
        {
            Console.WriteLine("\nTesting RemoveWhiteSpace");
            string[] testStrings = new string[3];
            testStrings[0] = "50 ";
            testStrings[1] = " 10";
            testStrings[2] = "   20   ";
            testStrings = Evaluator.RemoveWhitespace(testStrings);
            Console.WriteLine(testStrings[0] + "," + testStrings[1] + "," + testStrings[2]);
        }

        private static void TestIsOperator()
        {
            Console.WriteLine("\nTesting IsOperator method");
            string[] testOperators = new string[7];
            testOperators[0] = "+";
            testOperators[1] = "-";
            testOperators[2] = "/";
            testOperators[3] = "*";
            testOperators[4] = "(";
            testOperators[5] = ")";
            testOperators[6] = "10";
            for(int i = 0;i < testOperators.Length;i++)
            {
                Console.WriteLine(testOperators[i] + ": " + Evaluator.IsOperator(testOperators[i]));
            }
        }

        private static void TestIsInteger()
        {
            Console.WriteLine("\nTesting IsInteger method");
            Random rand = new Random();
            for(int i = 0; i < 10; i++)
            {
                int randomInt = rand.Next(0, 1000);
                Console.WriteLine(randomInt + ": " + Evaluator.IsInteger(randomInt.ToString()));
            }
            string[] notInts = new string[5];
            notInts[0] = "A";
            notInts[1] = "+";
            notInts[2] = "B10";
            notInts[3] = "BC";
            notInts[4] = " ";
            for(int i = 0; i < notInts.Length; i++)
            {
                Console.WriteLine(notInts[i] + ": " + Evaluator.IsInteger(notInts[i]));
            }
        }

        private static void TestIsVariable()
        {
            Console.WriteLine("\nTesting IsVariable method");
            string[] testVars = new string[7];
            testVars[0] = "A10";
            testVars[1] = "B6";
            testVars[2] = "C100";
            testVars[3] = "D500";
            testVars[4] = "E50";
            testVars[5] = "10";
            testVars[6] = "AaBb10";
            for(int i = 0; i < testVars.Length; i++)
            {
                Console.WriteLine(testVars[i] + ": " + Evaluator.IsVariable(testVars[i]));
            }
        }

        private static void TestEvaluate()
        {
            Console.WriteLine("\nTesting Evaluate Method");
            Evaluator.Lookup findVar = new Evaluator.Lookup(search);
            Console.WriteLine("5 + 10 + 50 * (10/5) + 5 = " + Evaluator.Evaluate("5 + 10 + 50 * (10/5) + 5", findVar));
            Console.WriteLine("10 / 5 + 20 * (50 * 10 + 5) + 5 = " +  Evaluator.Evaluate("10 / 5 + 20 * (50 * 10 + 5) + 5", findVar));
            Console.WriteLine("50 / A10 = " + Evaluator.Evaluate("50 / A10", findVar));
            Console.WriteLine("50 + 10 - 5 * 10 = " +  Evaluator.Evaluate("50 + 10 - 5 * 10", findVar));
            Console.WriteLine("10 * 50 + 5 - 7 * (10 - 10 + (5 + 5)) * 10 = " + Evaluator.Evaluate("10 * 50 + 5 - 7 * (10 - 10 + (5 + 5)) * 10", findVar));

        }

        private static int search(string s)
        {
            if (s == "A10")
                return 10;
            return 5;
        }
        
        static void Main(string[] args)
        {
            TestRemoveWhiteSpace();
            TestIsOperator();
            TestIsInteger();
            TestIsVariable();
            TestEvaluate();
        }
    }
}
