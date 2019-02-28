using RandoMapMod;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class CustomPinGroup : MonoBehaviour {
	private List<PinData> list = null;
	private int i = 0;

	void Update() {
		if ( list == null ) {
			this.list = PinData_S.All.Values.ToList();
		}

		if ( Input.GetKeyDown( KeyCode.F7 ) || Input.GetKeyDown( KeyCode.KeypadDivide ) ) {
			// DebugLog.Write( "F7 " + i + "--");
			list[i].Pin.Deselect();
			i--;
			if ( i < 0 ) {
				i = list.Count - 1;
			}
			list[i].Pin.Select();
		} else if ( Input.GetKeyDown( KeyCode.F8 ) || Input.GetKeyDown( KeyCode.KeypadMultiply ) ) {
			//DebugLog.Write( "F8 " + i + "++" );
			list[i].Pin.Deselect();
			i++;
			if ( i >= list.Count ) {
				i = 0;
			}
			list[i].Pin.Select();
		} else if ( Input.GetKeyDown( KeyCode.F9 ) ) {
			DebugLog.Write( "F9" );

			List<string> lines = new List<string>();
			foreach ( PinData pind in list ) {
				lines.Add( pind.ID + " " + pind.OffsetX + " " + pind.OffsetY + " " + pind.OffsetZ );
			}
			DebugLog.Write( lines );
		}
	}
}
