using ModCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using RND = UnityEngine.Random;

/*
 I'm warning you this file is:

                _ _           _                _             
               | | |         | |              (_)            
 _ __ ___  __ _| | |_   _    | |__   ___  _ __ _ _ __   __ _ 
| '__/ _ \/ _` | | | | | |   | '_ \ / _ \| '__| | '_ \ / _` |
| | |  __/ (_| | | | |_| |   | |_) | (_) | |  | | | | | (_| |
|_|  \___|\__,_|_|_|\__, |   |_.__/ \___/|_|  |_|_| |_|\__, |
                     __/ |                              __/ |
                    |___/                              |___/ 


 I would suggest not looking at all of this stuff.




 Honestly, it's not worth it.


































 You're probably going to hate yourself if you continue!













































 IT REALLY SUCKS!























































 
            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""














************************************************
* Okay so if you've made it THIS far, then you *
* probably are going to go all the way and see *
* all the boring stuff down below... But truly *
* you will only give yourself a headache or an *
* exceptionally bad rash while trying to write *
* all the notes you're going to need while you *
* very boringly try to understand all this. If *
* you think there's anything interesting here, *
* well, you're as sorely mistaken as your hand *
* will be... sore... I mean... Uh well I think *
* you get the point. So make like a good sneak *
* thief, and quit snooping where your eyes and *
* hands don't belong! GET OUT! STOP PEEKING!!! *
************************************************






























            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""
























































































































































































































































































            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""




            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""




            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""




PLEASE



            uuuuuuuuuuuuuuuuuuuu
          u" uuuuuuuuuuuuuuuuuu "u
        u" u$$$$$$$$$$$$$$$$$$$$u "u
      u" u$$$$$$$$$$$$$$$$$$$$$$$$u "u
    u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
  u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
u" u$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$u "u
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
$ $$$" ... "$...  ...$" ... "$$$  ... "$$$ $
$ $$$u `"$$$$$$$  $$$  $$$$$  $$  $$$  $$$ $
$ $$$$$$uu "$$$$  $$$  $$$$$  $$  """ u$$$ $
$ $$$""$$$  $$$$  $$$u "$$$" u$$  $$$$$$$$ $
$ $$$$....,$$$$$..$$$$$....,$$$$..$$$$$$$$ $
$ $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$ $
"u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
  "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
    "u "$$$$$$$$$$$$$$$$$$$$$$$$$$$$" u"
      "u "$$$$$$$$$$$$$$$$$$$$$$$$" u"
        "u "$$$$$$$$$$$$$$$$$$$$" u"
          "u """""""""""""""""" u"
            """"""""""""""""""""















































































































































































































































































































































































































































































































































































































































































































































































































































































































































































... okay f you. you just spoiled yourself, are you happy now? Jerk.
*/
namespace RandoMapMod.BoringInternals {
	internal static class SeriouslyBoring {
		internal const string BORING_PHRASE_1 = "ithinkidranktoomuch";
		internal const string BORING_PHRASE_2 = "touchfuzzygetdizzy";

		static SeriouslyBoring() {
			if ((DateTime.Now.Month == 4 && DateTime.Now.Day == 1)) {
				if (!BoringMode1) ToggleBoringMode1();
				if (!BoringMode2) ToggleBoringMode2();
			}
		}

		internal static bool BoringMode1 { get; private set; } = false;
		internal static void ToggleBoringMode1() {
			BoringMode1 = !BoringMode1;
		}

		internal static bool BoringMode2 { get; private set; } = false;
		internal static void ToggleBoringMode2() {
			BoringMode2 = !BoringMode2;
		}
	}

	class Anchors {
		public static Anchors Instance { get; private set; } = new Anchors();
		private static Dictionary<BoringPinThing, Vector3> _anchors = new Dictionary<BoringPinThing, Vector3>();
		private static float _seconds = -20f;

		public Vector3 this[BoringPinThing pin] {
			get {
				if (_anchors.TryGetValue(pin, out Vector3 vec)) {
					return vec;
				}

				return Vector3.zero;
			}
			set {
				if (_anchors.ContainsKey(pin)) {
					_anchors[pin] = value;
					return;
				}

				_anchors.Add(pin, value);
			}
		}

		private static void Shuffle() {
			BoringPinThing[] pins = _anchors.Keys.ToArray();
			List<Vector3> oldVecs = _anchors.Values.ToList();
			List<Vector3> newVecs = new List<Vector3>();

			Random rnd = new Random();
			while (oldVecs.Count > 0) {
				int next = rnd.Next(oldVecs.Count);
				newVecs.Add(oldVecs[next]);
				oldVecs.RemoveAt(next);
			}

			Dictionary<BoringPinThing, Vector3> dict = new Dictionary<BoringPinThing, Vector3>();
			for (int i = 0; i < pins.Length; i++) {
				dict.Add(pins[i], newVecs[i]);
			}

			_anchors = dict;
		}

		private static int logtrack = 3000;

		internal static void Update(BoringPinThing pin, float sumval) {
			if (pin != _anchors.Keys.First()) {
				return;
			}

			if (_seconds < 0f) {
				Shuffle();
				logtrack = 3000;
				_seconds = -20f;
			}

			if (_seconds < -5f) {
				_seconds = RND.Range(15f, 30f);
			}

			_seconds -= Time.deltaTime;

			if (_seconds*3 < logtrack) {
				DebugLog.Log($"Anchor Update : {_seconds}, {sumval}");
				logtrack = (int)_seconds*3;
			}
		}
	}

	class BoringPinThing : MonoBehaviour {
		private Vector3 _Anchor {
			get { return Anchors.Instance[this]; }
			set { Anchors.Instance[this] = value; }
		}

		private SpriteRenderer _SR => this.gameObject.GetComponent<SpriteRenderer>();
		private Pin _Pin => this.gameObject.GetComponent<Pin>();

		protected void Start() {
			//DebugLog.Log("BoringPin Start");
		}
		protected void Awake() {
			//DebugLog.Log("BoringPin Awake");
		}

		private Color _dizzyColor = Color.black;
		private float _dizzySpeed = 0f;
		private Vector3 _velocity = Vector3.zero;

		protected void Update() {
			if (SeriouslyBoring.BoringMode2) {
				//"touchfuzzygetdizzy"
				if (_dizzyColor == Color.black) {
					_dizzyColor = RND.ColorHSV();
					_dizzySpeed = RND.Range(-1f, 1f);
				}

				_dizzySpeed = Mathf.Clamp(_dizzySpeed + RND.Range(-0.01f, 0.01f), -1f, 1f);

				Color.RGBToHSV(_dizzyColor, out float hue, out _, out _);

				hue += _dizzySpeed * Time.deltaTime;

				if (hue >= 1) {
					hue = 0;
				}

				_dizzyColor = Color.HSVToRGB(hue, 1, 1);
				this._SR.color = _dizzyColor;
				//end "touchfuzzygetdizzy"
			} else {
				//Normal colors plz.
				if (this._SR.color != this._Pin.OrigColor && this._SR.color != this._Pin.InactiveColor) {
					if (GameStatus.ItemIsReachable(this._Pin.PinData.ID)) {
						this._SR.color = this._Pin.OrigColor;
					} else {
						this._SR.color = this._Pin.InactiveColor;
					}
				}
				//end Normal colors plz.
			}

			if (SeriouslyBoring.BoringMode1) {
				if (this._Anchor == Vector3.zero) {
					this._Anchor = this._Pin.OrigPosition;
				}

				//"ithinkidranktoomuch"
				const float k = 0.4f;                   //leash strength (spring force?)
				const float l = 0.01f;                  //leash length
				const float N = 2f;                   //Correction Vector Strength
				const float maxAccel = 1f / 10;
				const float maxSpeed = 2f;

				//Correction Vector =
				//                    y = 2(2^-(x/l)^4k) - 1
				//     y ranges from -1 to 1 and is shaped like a bell curve.
				//          https://www.desmos.com/calculator/fy7m9c7ctn
				Vector3 from_leash_anchor = this.transform.localPosition - this._Anchor;

				float x = from_leash_anchor.magnitude;
				double y = 2 * Math.Pow(2, -Math.Pow(x / l, 4 * k)) - 1;    // Correction Vector

				Vector3 correction = (float) y * from_leash_anchor;
				Vector3 j = RND.insideUnitCircle;

				Vector3 acceleration = Vector3.ClampMagnitude(j + (N * correction), maxAccel);

				_velocity += acceleration;                               //veloc changes by accel

				this.transform.localPosition += Vector3.ClampMagnitude(_velocity, maxSpeed) * Time.deltaTime;

				Anchors.Update(this, x);
				//end "ithinkidranktoomuch"
			} else if (this.transform.localPosition != this._Pin.OrigPosition) {
				this.transform.localPosition = this._Pin.OrigPosition;
			}
		}
	}
}