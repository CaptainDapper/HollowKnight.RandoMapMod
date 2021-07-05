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
  and the compass charm, and the quill. Additionally: grubs, roots,
  and essence boss location pins will also appear on the map. (Pink backgrounds)
* As an alternative to talking with Elder Bug, Some hotkeys have been added.
  `Ctrl + M` will give you all the maps, the charm, and the quill. (Can't be undone).
  `Crtl + P` will hide/show the pins for the randomized locations.
  `Ctrl + G` will toggle the Resource Helper pins- grubs + essence locations.
* If, according to the randomizer's logic, you can't access a location, that pin
  will be smaller and darker.
* If there is a non-geo cost (grubs or essence) for the location on top of the logic
  requirements, a small yellow exclamation point will show itself on top of the pin.

## How to install

1. Make sure `ModCommon.dll` and `RandomizerMod3.0.dll` are already installed
   in your Hollow Knight Mod Folder. If they're not installed, easiest way to
   install them is to [use the Hollow Knight Mod Installer](https://www.nexusmods.com/hollowknight/mods/9).
2. Download [the latest release of `RandoMapMod.dll`](https://github.com/CaptainDapper/HollowKnight.RandoMapMod/releases/).
3. Copy `RandoMapMod.dll` to your Hollow Knight mod folder (for example
   `C:\Program Files (x86)\Steam\steamapps\common\Hollow Knight\hollow_knight_Data\Managed\Mods`).

## Thanks!

Huge thanks to everyone who's worked on the amazing randomizer and map mod.

And shout outs to the [Hollow Knight Racing Discord](https://discord.gg/F3upRRu). They've been really helpful and supportive as I've worked on this.

## Version History

###

### v0.5.1

Finally got the code out there holy crap.

But actually this release introduced the Hotkeys and a few completely boring easter eggs (don't spoil yourself).
I think there were a couple of other things but honestly it has been too long so I forget. Oh well, at least it's updated!

Next is investigating a couple of bugs (lol) as well as adding EVEN MORE pins for the updates.

### v0.4

Complete refactor. Code should be a bit more organized, but maybe that's just me.
Added rando-pool specific sprites for the pins.
Compatibility for Rando Multiworld.

### v0.3.11

Non-randomized Grub and Essence pins
Settings update to handle different randomizer branches

### v0.3.10

Compatibility for Rando 3.10

### v0.3.5

Compatibility for Rando 3

### v0.3.2

First "official" release. Fixed the map boundaries so the map ACTUALLY scrolls to the edges without having to purchase every map first.