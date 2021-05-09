using System;
using UnityEngine;
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
		private const string BORING_PHRASE_1 = "ithinkidranktoomuch";
		private const string BORING_PHRASE_2 = "touchfuzzygetdizzy";

		static SeriouslyBoring() {
			if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1) {
				if (!BoringMode1) ToggleBoringMode1();
				if (!BoringMode2) ToggleBoringMode2();
			}
		}

		internal static string BoringCheck1(string typedString) {
			if (typedString.ToLower().Contains(BORING_PHRASE_1)) {
				ToggleBoringMode1();

				return "";
			}

			return typedString;
		}

		internal static string BoringCheck2(string typedString) {
			if (typedString.ToLower().Contains(BORING_PHRASE_2)) {
				ToggleBoringMode2();

				return "";
			}

			return typedString;
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



	internal class BoringPinThing : MonoBehaviour {
		private SpriteRenderer _SR => this.gameObject.GetComponent<SpriteRenderer>();
		private Pin _Pin => this.gameObject.GetComponent<Pin>();

		protected void Update() {
			if (SeriouslyBoring.BoringMode2) {
				//"touchfuzzygetdizzy"
				Color _dizzyColor = Color.black;
				float _dizzySpeed = 0f;
				if (_dizzyColor == Color.black) {
					_dizzyColor = RND.ColorHSV();
					_dizzySpeed = RND.Range(-1f, 1f);
				}

				_dizzySpeed = Mathf.Clamp(_dizzySpeed + RND.Range(-0.01f, 0.01f), -1f, 1f);

				Color.RGBToHSV(_dizzyColor, out float hue, out _, out _);

				hue += _dizzySpeed / 750;

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
				//"ithinkidranktoomuch"
				const float k = 1f;                     //leash strength (spring force?)
				const float l = 0.03f;                  //leash length
				const float N = 0.5f;                   //Correction Vector Strength
				const float maxAccel = 1f / 10;
				const float maxSpeed = 2f;

				//Correction Vector =
				//                    y = 2(2^-(x/l)^4k) - 1
				//     y ranges from -1 to 1 and is shaped like a bell curve.
				//          https://www.desmos.com/calculator/fy7m9c7ctn
				Vector3 from_leash_anchor = this.transform.localPosition - this._Pin.OrigPosition;

				float x = from_leash_anchor.magnitude;
				double y = 2 * Math.Pow(2, -Math.Pow(x / l, 4 * k)) - 1;    // Correction Vector

				Vector3 correction = (float) y * from_leash_anchor;
				Vector3 j = RND.insideUnitCircle;

				Vector3 acceleration = Vector3.ClampMagnitude(j + (N * correction), maxAccel * Time.deltaTime);

				Vector3 velocity = Vector3.zero;
				velocity += acceleration;                               //veloc changes by accel

				this.transform.localPosition += Vector3.ClampMagnitude(velocity, maxSpeed * Time.deltaTime);
				//end "ithinkidranktoomuch"
			} else if (this.transform.localPosition != this._Pin.OrigPosition) {
				this.transform.localPosition = this._Pin.OrigPosition;
			}
		}
	}

	internal class BoringBusinessInputListener : MonoBehaviour {
		string _typedString = "";

		protected void Update() {

			string inputString = Input.inputString;
			if (inputString != string.Empty) {
				//_typedString = SeriouslyBoring.BoringCheck1(_typedString);
				//_typedString = SeriouslyBoring.BoringCheck2(_typedString);

				if (_typedString.Length > 20) {
					_typedString.Substring(1);
				}
			}
		}
	}
}