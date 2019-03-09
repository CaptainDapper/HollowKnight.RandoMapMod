# Randomizer Map

This mod is made for the [Randomizer v2.0 mod, by seanpr96 and Firzen](https://github.com/seanpr96/HollowKnight.RandomizerMod). Simply put, it adds a pin on the map for each randomized location. The pins will also show up differently if it's possible to get to that location with your current rando settings.

My hopes for this is to make learning the randomizer more accessible to newer players who don't know the base map very well, and to help more experienced players really learn the logic deeply and improve routing decisions.

Let me know if you find any bugs, have any feature requests, or just have questions.

## How it works

There are three different types of pins: Small Dark (?) Pins, Big Red (?) Pins, and Yellow (!) Pins.

The Small (?) indicate item locations that are currently out-of-logic. It's possible they are still obtainable through sequence breaks. 

The Big (?) indicate item locations that are currently in logic, and one of these will contain the progression you are looking for, unless you're already in go mode.

The Yellow (!) indicate item locations that are currently in logic but still require some form of prerequisite to obtain; I.E. Sly's Shopkeeper Key, or the 10 grubs for the Grubsong location, or essence for Senorita Mothface, etc.

## Dependencies

Modding API, ModCommon, RandomizerMod2.0

## How to install

Ensure you have the above dependencies installed, then place the .dll file in the Hollow Knight/Managed/Mods directory.

## Thanks!

Huge thanks to seanpr96 for the amazing randomizer.

And shout outs to the [Hollow Knight Racing Discord](https://discord.gg/F3upRRu). They've been really helpful and supportive as I've worked on this.

## Version History

### v0.3.2

First "official" release. Fixed the map boundaries so the map ACTUALLY scrolls to the edges without having to purchase every map first.
