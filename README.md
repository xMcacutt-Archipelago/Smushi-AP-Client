# Smushi Come Home - Archipelago

This mod is a client side implementation for Archipelago.gg
For the APWorld for Smushi see [Smushi APWorld](https://github.com/xMcacutt-Archipelago/Archipelago-SmushiComeHome/releases/latest)

## Setup

#### Mod Manager - RECOMMENDED ONCE OPERATIONAL

If you are reading this on Thunderstore, great! Simply download the [Thunderstore Mod Manager](https://thunderstore.io/), follow the instructions to set up KeyWe and install the [Archipelago client mod](https://thunderstore.io/c/smushicomehome/p/xMcacutt/Smushi_Archipelago/).
This is the recommended installation method but manual installation is also possible and described below.

#### Manual Installation

First, download the latest release from the releases page and extract.

To Install MANUALLY, you'll need to install [BepinEx](https://github.com/BepInEx/BepInEx/releases/tag/v5.4.23.4). Navigate to your KeyWe folder, which should be in steamapps/common/KeyWe, and copy the Bepin files into this folder. Then run KeyWe.exe. this will create the necessary subdirectories for Bepin to function and for mods to load.

Next, close the game and navigate to the plugins folder in the BepInEx folder, and copy the Smushi_Archipelago folder from the latest release into into it.

Once you launch the game, the client should be running.

## Connecting

Upon loading into the main menu, press start, and you will be greeted by the connection screen, simply fill the text fields with the Server, Port, Slot Name, and Password, and press connect.

## Log Window

The log in the bottom left of the game can be enabled/disabled by pressing F7. This will show items collected and received in the Archipelago world as they are sent/received.
History is saved to the log and can be scrolled through by pressing F8.

## How This Rando Works

### Locations

In Smushi AP, the following locations are considered checks by default

- Each task / quests

- Each mushroom researched in the mycology journal (once obtained) which requires selecting the mushroom in the journal menu

- Each augmenter

### Items

The following Items can be sent to the Smushi world

- All quest items
- Energy Spores / Wind Essences
- Skins

## Goal Conditions
There are currently two options for goal conditions.

### Capybaras Reuinited
This requires returning home by helping the capybaras to reuinite.
The sacred orb is always locked to its default location so Sacred Holm and Chungy Cave must both be completed in either order.
The capybaras are fixed such that the player can leave and come back without issue.

### Heart of the Forest Restored
This requires obtaining the four sacred streamers and delivering them to the sacred tree in Grove of Life. 
Grove can be entered by finding the five lotus flowers throughout the multiworld and delivering them to the lotus platforms in the Lake.

## The Magic Conch Shell
Usually the conch shell would allow you to travel to any location. To avoid logic breaks, the conch shell will only allow you to travel to locations you have previously visited.
