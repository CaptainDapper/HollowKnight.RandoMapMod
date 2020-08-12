using Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	//Some object names get changed around in the Randomizer. Luckily, these changes are saved in the save-data.
	class JSONAction {
		public string sceneName;
		public string objectName;
		public string newShinyName;
		public float x;
		public float y;

		public enum Type {
			AddShinyToChest,
			ReplaceObjectWithShiny,
			CreateNewShiny,
			ChangeChestGeo,
			NONE
		}
	}

	static class ObjectNames {
		private static readonly DebugLog logger = new DebugLog(nameof(ObjectNames));
		private static Dictionary<string, string> dict = null;

		public static string Get( PinData pinD ) {
			if ( dict == null ) {
				Load( GameManager.instance.profileID );
			}

			string newName = "";
			if ( dict.TryGetValue( pinD.ID, out newName ) ) {
				return newName;
			} else {
				return pinD.OriginalName;
			}
		}

		private static void Add( string pinID, string newName ) {
			dict.Add( pinID, newName );
		}

		public static void Load(int saveSlot) {
			/*
			 * GOOD LORD WHY REMOVE THE STRINGVALUES
			foreach ( string val in RandomizerMod.RandomizerMod.Instance.Settings.StringValues.Values ) {
				if ( val.Contains( "newShinyName" ) ) {
					ObjectName newONC = JsonUtility.FromJson<ObjectName>( val );
					Add( newONC );
				}
			*/
			dict = new Dictionary<string, string>();
						
			Platform.Current.ReadSaveSlot( saveSlot, (Action<byte[]>) ( fileBytes =>
			{
				try {
					SaveGameData data = JsonUtility.FromJson<SaveGameData>( !GameManager.instance.gameConfig.useSaveEncryption || Platform.Current.IsFileSystemProtected ? Encoding.UTF8.GetString( fileBytes ) : Encryption.Decrypt( (string) new BinaryFormatter().Deserialize( (Stream) new MemoryStream( fileBytes ) ) ) );
					foreach ( string key in data.modData["RandomizerMod"].StringValues.Keys ) {
						JSONAction.Type type = JSONAction.Type.NONE;
							type = getActionType( key );
							if ( type == JSONAction.Type.NONE ) {
								continue;
							}

						string val = data.modData["RandomizerMod"].StringValues[key];
						JSONAction actionData = JsonUtility.FromJson<JSONAction>( val );
						PinData pinD = null;
						string newName = "";
						switch ( type ) {
							case JSONAction.Type.AddShinyToChest:
							case JSONAction.Type.ReplaceObjectWithShiny:
								pinD = PinData_S.All.Values.Where(
									pins => pins.SceneName == actionData.sceneName
										&& pins.OriginalName == actionData.objectName
									).FirstOrDefault();
								newName = actionData.newShinyName;
								break;
							case JSONAction.Type.CreateNewShiny:
								pinD = PinData_S.All.Values.Where(
									pins => pins.SceneName == actionData.sceneName
										&& pins.NewX == (int) actionData.x
										&& pins.NewY == (int) actionData.y
										&& pins.NewShiny == true
									).FirstOrDefault();
								newName = actionData.newShinyName;
								break;
							case JSONAction.Type.ChangeChestGeo:
								pinD = PinData_S.All.Values.Where(
									pins => pins.SceneName == actionData.sceneName
										&& pins.InChest == true
									).FirstOrDefault();
								newName = actionData.objectName;
								break;
							case JSONAction.Type.NONE:
							default:
								logger.Error( "What the crap just happened...? This enum is weeeeird." );
								break;
						}
						if ( pinD != null && newName != "" ) {
							//DebugLog.Write( "ONC Added: Item '" + pinD.ID + "' ObjectName '" + newName + "' Type '" + type + "'" );
							Add( pinD.ID, newName );
						}
					}
				} catch ( Exception ex ) {
					logger.Error( "Error trying to MANUALLY FREAKING LOAD the save data" );
					logger.Error( ex.ToString() );
				}
			} ) );
		}

		private static JSONAction.Type getActionType( string key ) {
			foreach ( JSONAction.Type type in Enum.GetValues(typeof(JSONAction.Type)).Cast<JSONAction.Type>() ) {
				if ( type != JSONAction.Type.NONE && key.Contains( type.ToString() ) ) {
					return type;
				}
			}
			return JSONAction.Type.NONE;
		}
	}
}
