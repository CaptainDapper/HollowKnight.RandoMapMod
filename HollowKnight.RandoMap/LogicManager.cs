using System;
using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod {
	static class LogicManager {
		//I'm not proud of this, but most of this code was copied right from the Randomizerv2.0 mod...
		//I could have made use of the original assembly, but they were marked internal in Seanpr's assembly.
		private static Dictionary<string, string[]> macros = null;

		public static void AddMacro( string macroName, string rawMacro ) {
			if ( macros == null ) {
				macros = new Dictionary<string, string[]>();
			}

			macros.Add( macroName, ShuntingYard( rawMacro ) );
		}

		public static string[] ShuntingYard( string infix ) {
			int i = 0;
			Stack<string> stack = new Stack<string>();
			List<string> postfix = new List<string>();

			while ( i < infix.Length ) {
				string op = getNextOperator( infix, ref i );

				// Easiest way to deal with whitespace between operators
				if ( op.Trim( ' ' ) == string.Empty ) {
					continue;
				}

				if ( op == "+" || op == "|" ) {
					while ( stack.Count != 0 && ( op == "|" || ( op == "+" && stack.Peek() != "|" ) ) && stack.Peek() != "(" ) {
						postfix.Add( stack.Pop() );
					}

					stack.Push( op );
				} else if ( op == "(" ) {
					stack.Push( op );
				} else if ( op == ")" ) {
					while ( stack.Peek() != "(" ) {
						postfix.Add( stack.Pop() );
					}

					stack.Pop();
				} else {
					// Parse macros
					if ( macros.TryGetValue( op, out string[] macro ) ) {
						postfix.AddRange( macro );
					} else {
						postfix.Add( op );
					}
				}
			}

			while ( stack.Count != 0 ) {
				postfix.Add( stack.Pop() );
			}

			return postfix.ToArray();
		}

		public static bool ParsePrereqNode( string node ) {
			PlayerData pd = PlayerData.instance;
			if ( node.Contains( '>' ) ) {
				string[] str = node.Split( '>' );
				int testVal = int.Parse( str[1] );
				return ( pd.GetInt( str[0] ) > testVal );
			} else {
				return pd.GetBool( node );
			}
		}

		public static bool ParseLogic( string[] logic, Predicate<string> eval ) {
			if ( logic == null || logic.Length == 0 ) {
				return true;
			}

			string[] results = new string[logic.Length];

			Stack<bool> stack = new Stack<bool>();

			for ( int i = 0; i < logic.Length; i++ ) {
				switch ( logic[i] ) {
					case "+":
						if ( stack.Count < 2 ) {
							DebugLog.Warn( $"Could not parse logic: Found + when stack contained less than 2 items" );
							return false;
						}

						stack.Push( stack.Pop() & stack.Pop() );
						results[i] = "+";
						break;
					case "|":
						if ( stack.Count < 2 ) {
							DebugLog.Warn( $"Could not parse logic: Found | when stack contained less than 2 items" );
							return false;
						}

						stack.Push( stack.Pop() | stack.Pop() );
						results[i] = "|";
						break;
					case "SHADESKIPS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.ShadeSkips );
						results[i] = stack.Peek().ToString();
						break;
					case "ACIDSKIPS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.AcidSkips );
						results[i] = stack.Peek().ToString();
						break;
					case "SPIKETUNNELS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.SpikeTunnels );
						results[i] = stack.Peek().ToString();
						break;
					case "MISCSKIPS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.MiscSkips );
						results[i] = stack.Peek().ToString();
						break;
					case "FIREBALLSKIPS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.FireballSkips );
						results[i] = stack.Peek().ToString();
						break;
					case "MAGSKIPS":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.MagSkips );
						results[i] = stack.Peek().ToString();
						break;
					case "NOCLAW":
						stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.NoClaw );
						results[i] = stack.Peek().ToString();
						break;
					case "EVERYTHING":
						stack.Push( false );
						results[i] = stack.Peek().ToString();
						break;
					default:
						stack.Push( eval( logic[i] ) );
						results[i] = stack.Peek().ToString();
						break;
				}
			}

			if ( stack.Count == 0 ) {
				DebugLog.Warn( $"Could not parse logic: Stack empty after parsing" );
				return false;
			}

			if ( stack.Count != 1 ) {
				DebugLog.Warn( $"Extra items in stack after parsing logic" );
			}

			return stack.Pop();
		}

		private static string getNextOperator( string infix, ref int i ) {
			int start = i;

			if ( infix[i] == '(' || infix[i] == ')' || infix[i] == '+' || infix[i] == '|' ) {
				i++;
				return infix[i - 1].ToString();
			}

			while ( i < infix.Length && infix[i] != '(' && infix[i] != ')' && infix[i] != '+' && infix[i] != '|' ) {
				i++;
			}

			return infix.Substring( start, i - start ).Trim( ' ' );
		}
	}
}
