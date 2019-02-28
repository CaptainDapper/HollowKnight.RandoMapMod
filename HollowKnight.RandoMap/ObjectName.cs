using Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	//Some object names get changed around in the Randomizer. Luckily, these changes are saved in the save-data.
	class ObjectName {
		public string sceneName;
		public string objectName;
		public string newShinyName;
		public float x;
		public float y;
	}

	static class ObjectNames {
		private static List<ObjectName> list = null;

		public static string Get( PinData pinD ) {
			if ( list == null )
				Load(GameManager.instance.profileID);

			foreach ( ObjectName onc in list ) {
				if ( pinD.NewShiny == true && pinD.NewX == (int) onc.x && pinD.NewY == (int) onc.y ) {
					return onc.newShinyName;
				}

				if ( onc.sceneName == pinD.SceneName && onc.objectName == pinD.OriginalName ) {
					return onc.newShinyName;
				}
			}

			return pinD.OriginalName;
		}

		private static void Add( ObjectName onc ) {
			list.Add( onc );
		}

		public static void Load(int saveSlot) {
			/*
			 * GOOD LORD SEAN WHY REMOVE THE STRINGVALUES
			foreach ( string val in RandomizerMod.RandomizerMod.Instance.Settings.StringValues.Values ) {
				DebugLog.Write( val );
				if ( val.Contains( "newShinyName" ) ) {
					ObjectName newONC = JsonUtility.FromJson<ObjectName>( val );
					Add( newONC );
				}
			*/

			list = new List<ObjectName>();
						
			Platform.Current.ReadSaveSlot( saveSlot, (Action<byte[]>) ( fileBytes =>
			{
				try {
					SaveGameData data = JsonUtility.FromJson<SaveGameData>( !GameManager.instance.gameConfig.useSaveEncryption || Platform.Current.IsFileSystemProtected ? Encoding.UTF8.GetString( fileBytes ) : Encryption.Decrypt( (string) new BinaryFormatter().Deserialize( (Stream) new MemoryStream( fileBytes ) ) ) );
					foreach ( string val in data.modData["RandomizerMod"].StringValues.Values ) {
						if ( val.Contains( "newShinyName" ) ) {
							ObjectName newONC = JsonUtility.FromJson<ObjectName>( val );
							Add( newONC );
						}
					}
				} catch ( Exception ex ) {
					DebugLog.Error( "Error trying to MANUALLY FREAKING LOAD the save data" );
					DebugLog.Error( ex.ToString() );
				}
			} ) );
		}
	}
}
