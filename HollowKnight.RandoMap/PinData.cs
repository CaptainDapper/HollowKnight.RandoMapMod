using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RandoMapMod {
	class PinData {
		public enum Types {
			SceneData,
			PlayerSceneVisited,
			PlayerBool,
			PlayerGT
		}
		public string Name {
			get; set;
		}
		public string Item {
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
		public string SceneName {
			get;
			internal set;
		}
		public string ObjectName {
			get;
			internal set;
		}
		public string PDName {
			get;
			internal set;
		}
		public string PDValue {
			get;
			internal set;
		}
		public string LogicRaw {
			get;
			internal set;
		}
		public GameObject gameObject {
			get;
			set;
		}

		public PinData() {

		}

		public PinData( string name ) {
			this.Name = name;
		}
	}
}
