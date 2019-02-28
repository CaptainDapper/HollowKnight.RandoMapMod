using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	class PinData {
		public enum Types {
			SceneData,
			PlayerBool
		}

		//Assigned with pindata.xml
		public string ID {
			get;
			internal set;
		}
		public string PinScene {
			get;
			internal set;
		}

		public Types CheckType {
			get;
			internal set;
		}
		public string CheckBool {
			get;
			internal set;
		}
		public float OffsetX {
			get;
			internal set;
		}
		public float OffsetY {
			get;
			internal set;
		}
		public float OffsetZ {
			get;
			internal set;
		}

		//Assigned with Randomizer's items.xml:
		public string SceneName {
			get;
			internal set;
		}
		public string OriginalName {
			get;
			internal set;
		}
		public string LogicRaw {
			get;
			internal set;
		}
		public string LogicBool {
			get;
			internal set;
		}
		public bool NewShiny {
			get;
			internal set;
		}
		public int NewX {
			get;
			internal set;
		}
		public int NewY {
			get;
			internal set;
		}

		//Stuff and things
		private string[] pLogic = null;
		public string[] Logic {
			get {
				if ( pLogic == null ) {
					pLogic = LogicManager.ShuntingYard( this.LogicRaw );
				}
				return pLogic;
			}
		}
		public bool Obtained {
			get {
				return PlayerData.instance.GetBool( this.LogicBool );
			}
		}
		public bool Possible {
			get {
				//DebugLog.Write( this.ID + " Possible?" );
				return LogicManager.ParseLogic( this.Logic );
			}
		}
		public string ObjectName {
			get {
				return ObjectNames.Get( this );
			}
		}

		public Vector3 Offset {
			get {
				return new Vector3( this.OffsetX, this.OffsetY, this.OffsetZ );
			}
		}

		public Pin Pin {
			get;
			set;
		}

		public PinData() {
			this.SceneName = "";
			this.OriginalName = "";
			this.LogicRaw = "";
			this.LogicBool = "";
			this.NewShiny = false;
		}
	}
}
