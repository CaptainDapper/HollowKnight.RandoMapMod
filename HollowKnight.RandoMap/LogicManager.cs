using ModCommon;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RandoMapMod {
	static class LogicManager {
		//I'm not proud of this, but most of this code was copied right from the Randomizerv2.0 mod...
		//I could have made use of the original assembly, but they were marked internal in Seanpr's assembly.
		private static Dictionary<string, string[]> macros = null;

        public static List<string> reachableItems;
        public static List<string> checkedItems;

        public static void AddMacro( string macroName, string rawMacro ) {
            //Dev.Log(macroName + " (" + rawMacro + ")");
			if ( macros == null ) {
				macros = new Dictionary<string, string[]>();
			}

			macros.Add( macroName, ShuntingYard( rawMacro ) );
		}

		public static string[] ShuntingYard( string infix ) {
            //Dev.Log("1");
			int i = 0;
			Stack<string> stack = new Stack<string>();
			List<string> postfix = new List<string>();

            while ( i < infix.Length ) {
                //Dev.Log("2 " + i);
                string op = getNextOperator( infix, ref i );
                //Dev.Log("3");
                // Easiest way to deal with whitespace between operators
                if ( op.Trim( ' ' ) == string.Empty ) {
					continue;
				}
                //Dev.Log("4");

                if ( op == "+" || op == "|" ) {
                    //Dev.Log("5a");
                    while ( stack.Count != 0 && ( op == "|" || ( op == "+" && stack.Peek() != "|" ) ) && stack.Peek() != "(" ) {
						postfix.Add( stack.Pop() );
					}

					stack.Push( op );
				} else if ( op == "(" ) {
                    //Dev.Log("5b");
                    stack.Push( op );
				} else if ( op == ")" ) {
                    //Dev.Log("5c");
                    while ( stack.Peek() != "(" ) {
						postfix.Add( stack.Pop() );
					}

					stack.Pop();
				} else {
                    //Dev.Log("5d");
                   // Dev.Log(op);
                    // Parse macros
                    if ( macros.TryGetValue( op, out string[] macro ) ) {
						postfix.AddRange( macro );
					} else {
						postfix.Add( op );
					}
				}
                //Dev.Log("6");
            }

			while ( stack.Count != 0 ) {
				postfix.Add( stack.Pop() );
			}
            //Dev.Log("7");

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

			Stack<bool> stack = new Stack<bool>();
            //Dev.Log("Parse Logic");
			for ( int i = 0; i < logic.Length; i++ ) {
                //Dev.Log(logic[i]);
                switch ( logic[i] ) {
					case "+":
						if ( stack.Count < 2 ) {
							DebugLog.Warn( $"Could not parse logic: Found + when stack contained less than 2 items" );
							return false;
						}
						stack.Push( stack.Pop() & stack.Pop() );
						break;
					case "|":
						if ( stack.Count < 2 ) {
							DebugLog.Warn( $"Could not parse logic: Found | when stack contained less than 2 items" );
							return false;
						}

						stack.Push( stack.Pop() | stack.Pop() );
						break;
                    case "MILDSKIPS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.MildSkips);
                        break;
                    case "SHADESKIPS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.ShadeSkips);
                        break;
                    case "FIREBALLSKIPS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.FireballSkips);
                        break;
                    case "ACIDSKIPS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.AcidSkips);
                        break;
                    case "SPIKETUNNELS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.SpikeTunnels);
                        break;
                    case "DARKROOMS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.DarkRooms);
                        break;
                    case "SPICYSKIPS":
                        stack.Push(RandomizerMod.RandomizerMod.Instance.Settings.SpicySkips);
                        break;
                    //case "SHADESKIPS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.ShadeSkips );
                    //	break;
                    //case "ACIDSKIPS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.AcidSkips );
                    //	break;
                    //case "SPIKETUNNELS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.SpikeTunnels );
                    //	break;
                    //case "MISCSKIPS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.MiscSkips );
                    //	break;
                    //case "FIREBALLSKIPS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.FireballSkips );
                    //	break;
                    //case "MAGSKIPS":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.MagSkips );
                    //	break;
                    //case "NOCLAW":
                    //	stack.Push( RandomizerMod.RandomizerMod.Instance.Settings.NoClaw );
                    //	break;
                    //case "EVERYTHING":
                    //	stack.Push( false );
                    //	break;
                    default:
						stack.Push( eval( logic[i] ) );
						break;
				}
                //Dev.Log(logic[i] + "(" + stack.Peek() + ")");
            }

			if ( stack.Count == 0 ) {
				DebugLog.Warn( $"Could not parse logic: Stack empty after parsing" );
				return false;
			}

			if ( stack.Count != 1 ) {
				DebugLog.Warn( $"Extra items in stack after parsing logic" );
			}

            //Dev.Log("This Logic is " + stack.Peek());
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
