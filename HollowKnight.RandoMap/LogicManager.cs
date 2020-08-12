using ModCommon;
using ModCommon.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod
{
	static class LogicManager
	{
		private static readonly DebugLog logger = new DebugLog(nameof(LogicManager));
		private static Dictionary<string, string[]> macros = null;

		public static List<string> reachableItems;
		public static List<string> checkedItems;

		public static void AddMacro(string macroName, string rawMacro)
		{
			if (macros == null)
			{
				macros = new Dictionary<string, string[]>();
			}
			macros.Add(macroName, ShuntingYard(rawMacro));
		}

		/// <summary>
		/// Translate an infix expression into a postfix expression. For example, given an expression like
		/// <c>"(Cliffs_02[bot1] + (CLAW | WINGS)) | Cliffs_02[door1] | Cliffs_02[left1] | Cliffs_02[left2]"</c>,
		/// this will return a array <c>"Cliffs_02[bot1]", "Mantis_Claw", "Monarch_Wings", "|", "+",
		/// "Cliffs_02[door1]", "|", "Cliffs_02[left1]", "|", "Cliffs_02[left2]", "|"</c>.
		/// </summary>
		/// <param name="infix">A string representation of an infix expression.</param>
		/// <returns>The corresponding parsed postfix expression.</returns>
		public static string[] ShuntingYard(string infix)
		{
			int i = 0;
			Stack<string> stack = new Stack<string>();
			List<string> postfix = new List<string>();

			while (i < infix.Length)
			{
				string op = getNextOperator(infix, ref i);
				// Easiest way to deal with whitespace between operators
				if (op.Trim(' ') == string.Empty)
				{
					continue;
				}
				if (op == "+" || op == "|")
				{
					while (stack.Count != 0 && (op == "|" || (op == "+" && stack.Peek() != "|")) && stack.Peek() != "(")
					{
						postfix.Add(stack.Pop());
					}
					stack.Push(op);
				}
				else if (op == "(")
				{
					stack.Push(op);
				}
				else if (op == ")")
				{
					while (stack.Peek() != "(")
					{
						postfix.Add(stack.Pop());
					}

					stack.Pop();
				}
				else
				{
					// Parse macros
					if (macros.TryGetValue(op, out string[] macro))
					{
						postfix.AddRange(macro);
					}
					else
					{
						postfix.Add(op);
					}
				}
			}
			while (stack.Count != 0)
			{
				postfix.Add(stack.Pop());
			}
			return postfix.ToArray();
		}

		private static string getNextOperator(string infix, ref int i)
		{
			int start = i;

			if (infix[i] == '(' || infix[i] == ')' || infix[i] == '+' || infix[i] == '|')
			{
				i++;
				return infix[i - 1].ToString();
			}

			while (i < infix.Length && infix[i] != '(' && infix[i] != ')' && infix[i] != '+' && infix[i] != '|')
			{
				i++;
			}

			return infix.Substring(start, i - start).Trim(' ');
		}
	}
}