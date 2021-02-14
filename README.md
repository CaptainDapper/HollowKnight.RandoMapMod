# Randomizer Map

[![GitHub Stars](https://img.shields.io/github/stars/CaptainDapper/HollowKnight.RandoMapMod.svg)](https://github.com/CaptainDapper/HollowKnight.RandoMapMod)
[![GitHub Issues](https://img.shields.io/github/issues/CaptainDapper/HollowKnight.RandoMapMod.svg)](https://github.com/CaptainDapper/HollowKnight.RandoMapMod/issues)
[![Current Version](https://img.shields.io/badge/version-0.5.1-green.svg)](https://github.com/CaptainDapper/HollowKnight.RandoMapMod)

This is a mod for Hollow Knight Randomizer that adds pins to the map showing the location of the various checks.
It aids in the accessibility of Hollow Knight Randomizer aimed at beginners,
casual fans, and people interested in learning the Logic more deeply without studying boolean phrases.

![Example Screenshot](./readmeAssets/screenshot.jpg)

## Features

* Talking to Elderbug 3 times activates the mod and gives you all maps
  (including the Collector's Map if grubs are non-randomized) and the compass
  charm.
* Alternatively, `Ctrl + M` will give you all the maps, `Ctrl + G` will toggle
  the Collector's Map, and `Crtl + P` will hide/show the Rando pins.
* The map will contain pins indicating which checks are currently reachable.
* The map will show a list of areas and the number of checks available in each
  area.
* In addition, non-randomized Grub and Essence sources have pins to indicate whether or not they are currently accessible in logic.

## How to install

1. Make sure `ModCommon.dll` and `RandomizerMod3.0.dll` are already installed
   in your Hollow Knight Mod Folder. If they're not installed, easiest way to
   install them is to [use the Hollow Knight Mod Installer](https://www.nexusmods.com/hollowknight/mods/9).
2. Download [the latest release of `RandoMapMod.dll`](https://github.com/CaptainDapper/HollowKnight.RandoMapMod/releases/).
3. Copy `RandoMapMod.dll` to your Hollow Knight mod folder (for example
   `C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods`).
4. Start a new game.
5. Talk to Elderbug 3 times.

## How it works

There are five different types of pins:

* **Small Dark (?) Pins** indicate item locations that are considered to be not
  reachable based on the allowed-skip settings in the randomizer. It's possible
  they are still obtainable through sequence breaks.
* **Big Red (?) Pins** indicate item locations that are currently considered
  reachable, and one of these will contain the progression you are looking for
  if you are stuck.
* **Yellow (!) Pins** indicate item locations that are currently considered
  reachable but still require some form of prerequisite to obtain. For example,
  Grubs for Grubfather or Essence for Seer.
* **Small Dark ($) Pins** indicate shop locations that are currently considered
  not reacehable.
* **Big Green ($) Pins** indicate shop locations that are currently in
  reachable, as a reminder to the player that the shop may still have items for
  sale they may need.

## Thanks!

Huge thanks to everyone who's worked on the amazing randomizer and map mod.

And shout outs to the [Hollow Knight Racing Discord](https://discord.gg/F3upRRu). They've been really helpful and supportive as I've worked on this.

## Version History

### v0.3.2

First "official" release. Fixed the map boundaries so the map ACTUALLY scrolls to the edges without having to purchase every map first.

### v0.3.5

Compatibility for Rando 3

### v0.3.10

Compatibility for Rando 3.10

### v0.3.11

Non-randomized Grub and Essence pins
Settings update to handle different randomizer branches
