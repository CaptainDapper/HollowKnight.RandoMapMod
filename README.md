# Randomizer Map

This mod is made for the [Randomizer v3.0 mod, by seanpr96, Firzen and homothetyhk](https://github.com/homothetyhk/HollowKnight.RandomizerMod).
Simply put, it adds a pin on the map for each randomized location. The pins
will also show up differently if it's possible to get to that location with
your current rando settings.

My hopes for this is to make learning the randomizer more accessible to newer
players who don't know the base map very well, and to help more experienced
players really learn the logic deeply and improve routing decisions.

Let me know if you find any bugs, have any feature requests, or just have
questions.

## How it works

There are five different types of pins: Small Dark (?) Pins, Big Red (?)
Pins, Yellow (!) Pins, Small Dark ($) Pins, and Big Green ($) Pins.

* The Small (?) indicate item locations that are considered to be not
  reachable based on the allowed-skip settings in the randomizer. It's
  possible they are still obtainable through sequence breaks.
* The Big (?) indicate item locations that are currently considered
  reachable, and one of these will contain the progression you are
  looking for if you are stuck.
* The Yellow (!) indicate item locations that are currently considered
  reachable but still require some form of prerequisite to obtain. For
  example, Grubs for Grubfather or Essence for Seer.
* The Small ($) indicate shop locations that are currently considered
  not reacehable.
* The Green ($) indicate shop locations that are currently in recheable,
  as a reminder to the player that the shop may still have items for sale
  they may need.

## Dependencies

Modding API, ModCommon, RandomizerMod2.0

## How to install

Ensure you have the above dependencies installed, then place the .dll file in the Hollow Knight/Managed/Mods directory.

## Thanks!

* seanpr96 for the original version of the randomizer.
* homothetyhk for version 3 of the randomizer.
* CaptainDapper for the original version of the RandoMapMod.
* TheGreatGallus for updating RandoMapMod to work with v3 of the randomizer.

## Version History

* v0.3.2
  * First "official" release. Fixed the map boundaries so the map ACTUALLY scrolls to the edges without having to purchase every map first.
* v0.3.5
  * Compatibility for Rando 3
* v0.3.8
  * Fixed the location of various pins (Monarch Wing, ShadeCloak, Isma's Tear, etc. see https://github.com/TheGreatGallus/HollowKnight.RandoMapMod/commit/4fde875a9daf92d1b6e8519aa11e9f362b7c32b7 for the full list).
  * Talking to Elderbug now causes Cornifer to immediately go home and sleep.
* v0.4.0
  * Elderbug now also gives Collector's Map, indicating the location of the grubs, if grubs are not randomized.
* v0.5.0
  * There is now an overlay on the map that shows how many reachable checks there are in each location.
