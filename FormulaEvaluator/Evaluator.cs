using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
{
    public static class Evaluator
    {
        public delegate int Lookup(String v);

        public static int Evaluate(String exp, Lookup variableEvaluator)
        {
            //Parse Tokens
            string[] tokens = RemoveWhitespace(Regex.Split(exp, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"));

            Stack<String> operators = new Stack<String>(); //stack for operator tokens: +, -, /, *, (, )
            Stack<int> values = new Stack<int>(); //stack for integer values
            values.Push(int.Parse(tokens[0]));
            //Iterate over array of parsed tokens
            for(int i = 1; i < tokens.Length; i++)
            {
               
                string token = tokens[i]; //current token
                string topOfOperators = " "; //ensures we don't peek the top of the stack when it is empty to avoid getting exceptions we don't want
                if (operators.Count > 0)
                    topOfOperators = operators.Peek();
                if(IsInteger((token)))
                {
                    int intToken = int.Parse(token); // recieve int from string token
                    if (topOfOperators == "/" || topOfOperators == "*")
                    {
                        //Helper method used when operators has a / or * at top of stack and we want to backtrack to calculate product
                        BacktrackProduct(operators, values, intToken);
                    }
                    else
                        values.Push(intToken);
                }
                else if(IsOperator(token))
                {
                    //Use switch to specify action for each unique operator (instead of multiple if)
                    switch (token)
                    {
                        case "+":case "-":
                            if (topOfOperators == "+" || topOfOperators == "-")
                            {
                                //Helper method used when operators have a + or - at top of stack and we want to calculate sum 
                                BacktrackSum(operators, values);
                            }
                            operators.Push(token);
                            break;

                        case "/": case "*":
                            operators.Push(token);
                            break;
             
                        case "(":
                            operators.Push(token);
                            break;

                        case ")":
                            if(topOfOperators == "+" || topOfOperators == "-")
                            {
                                BacktrackSum(operators, values);
                            }
                            if(operators.Pop() != "(") 
                            {
                                //handle possible invalid expression if no opening paranthese was passed
                                throw new ArgumentException("Invalid expression passed. " +
                                    "Could not find opening paranethese");
                            }
                            if(operators.Count > 0) { 
                                if (operators.Peek() == "*" || operators.Peek() == "/")
                                {
                                    //return ArgumentException when an invalid expression was passed given the minimum amount of integers are not in values
                                    if (values.Count < 2)
                                    {
                                        throw new ArgumentException("Invalid expression passed. Unable to" +
                                            "divide or multiply with fewer than two remaining values");
                                    }
                                    if (operators.Peek() == "*")
                                        values.Push(values.Pop() * values.Pop());
                                    else if (operators.Peek() == "/")
                                    {
                                        //use temp variables to allow us to divide in the correct order and push the result onto values
                                        int divisor = values.Pop();
                                        int dividend = values.Pop();
                                        //prevent division by zero
                                        if (divisor == 0)
                                            throw new ArgumentException("Invalid expression. Cannot divide by 0");
                                        values.Push(dividend / divisor);
                                    }
                                    operators.Pop();
                                }
                            }
                            break;
                    }
                }
                else if (IsVariable(token))
                {
                    //initialize theoretical integer variable before try/catch
                    int varValue = 0;
                    try
                    {
                        //use the variableEvaluator function to grab its integer values
                        varValue = variableEvaluator(token);
                    }
                    //catch possible exception if variableEvaluator does not have an integer value for specified variable
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Invalid variable name " + token + " was found within the expression");
                    }
                    if (topOfOperators == "/" || topOfOperators == "*")
                    {
                        //Helper method
                        BacktrackProduct(operators, values, varValue);
                  
                    }
                    else
                        values.Push(varValue);

                }
            }
            //end of iteration over all tokens 

            //if operators is empty return last remaining integer in values stack
            if (operators.Count == 0)
            {
                if (values.Count != 1)
                    throw new ArgumentException("Invalid expression. Execution ended without one remaining integer in the value stack");
                return values.Pop(); //return last values in values assuming ArgumentException is not thrown
            }
            //checking for invalid conditions upon completion of iteration over tokens
            if ((operators.Peek() != "+" && operators.Peek() != "-") || values.Count != 2)
                throw new ArgumentException("Invalid expression. Execution ended with invalid operators and/or remaining integers");
            //condition if last operator is +
            if (operators.Peek() == "+")
            {
                operators.Pop();
                return values.Pop() + values.Pop();
            }
            //assumiung the only remaining case is operators remains with "-" and two integers remain in values stack
            int second = values.Pop();
            int first = values.Pop();
            operators.Pop(); 
            return first - second;


        }

        /// <summary>
        ///checks if token is an operator 
        /// </summary>
        /// <param name="token"></param>
        /// <returns>whether the current token is an operator</returns>
        public static bool IsOperator(String token)
        {
            if (token == "+" || token == "-" ||  token == "(" || token == ")" || token == "/" || token == "*")
                return true;
            return false; 
        }

        /// <summary>
        /// checks if current token is an integer value
        /// </summary>
        /// <param name="token"></param>
        /// <returns>whether token is an integer</returns>
        public static bool IsInteger(String token)
        {
            return int.TryParse(token, out _);
        }

        /// <summary>
        /// checks if current token is a variable to be evaluated by variableEvaluator
        /// </summary>
        /// <param name="token"></param>
        /// <returns>whether token is a variable</returns>
        public static bool IsVariable(string token)
        {
            if (Regex.IsMatch(token, @"([a-zA-z]+)(\d+)")) 
                return true;
            return false;
        }

        /// <summary>
        /// Helper method to remove all trailing and leading whitespace on tokens 
        /// in Evaluate method 
        /// </summary>
        /// 
        /// <param name="tokens"> array of parsed tokens from expression string</param>
        /// <returns></returns>
        public static string[] RemoveWhitespace(string[] tokens)
        {
            for(int i = 0; i < tokens.Length; i++)
            {
                tokens[i] = tokens[i].Trim();
            }
            return tokens;
        }

        /// <summary>
        /// Private helper method made to improve readability of code. 
        /// This method backtracks and calculates the sum/difference of the values in values stack using the most
        /// recent operator in operators when appropriate
        /// </summary>
        /// 
        /// <param name="operators"> operator stack containing mathematical operators</param>
        /// <param name="values"> values stack containing current integers in calculation </param>
        /// 
        private static void BacktrackSum(Stack<string> operators, Stack<int> values)
        {
            //Check for possible invalid expression 
            if (values.Count < 2)
            {
                throw new ArgumentException("Invalid Argument. Unable to add or subtract with " +
                "fewer than two remaining values");
            }
            if (operators.Peek() == "+")
            {
                values.Push(values.Pop() + values.Pop());
            }
            else
            {
                //use temporary variables to subtract in the correct order (subtracting the integer from on top of the stack from the integer below would be the correct operation in reverse)
                int second = values.Pop();
                int first = values.Pop();
                values.Push(first - second);
            }
            //Remove operator from top of operator stack once we are done using it
            operators.Pop();
        }

        /// <summary>
        /// Private helper method made to improve readability of code. 
        /// This method backtracks and calculates the product/quotient of the values in values stack using the most
        /// recent operator in operators when appropriate
        /// </summary>
        /// 
        /// <param name="operators"> operator stack containing mathematical operators</param>
        /// <param name="values"> values stack containing current integers in calculation </param>
        /// <param name="token"> current Integer token</param>
        private static void BacktrackProduct(Stack<string> operators, Stack<int> values, int token)
        {
            //check for a possible invalid expression
            if (values.Count == 0)
            {
                throw new ArgumentException("Invalid expression. Unable to multiply or divide with fewer than " +
                    "two remaining values");
            }
            if (operators.Peek() == "*")
                values.Push(values.Pop() * token);
            else
            {
                //Prevent division by 0
                if (token == 0)
                    throw new ArgumentException("Invalid expression. Cannot divide by 0");
                values.Push(values.Pop() / token);
            }
            //Remove operator from top of operator stack once we are done using it 
            operators.Pop();
        }
    }
}
