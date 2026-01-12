# Changelog

---
## v1.0.9
No Client Changes

Poptracker aligned with apworld
### Logic
- Ancient Relics Returned logic says it’s possible without Ancient Relics
- Strawberry Augmenter logic says it’s possible without Band of Elasticity
- Hygrocybe Conica logic may be wrong. Should be possible with Leaf, Hook lvl 1, and Tool of Mining OR Leaf and Hook lvl 2
- Secret Opener Found logic should include Hooks lvl 2
- Macrolepiota Procera and Myrtle Pools Wind Essence accessible with 2 Energy Spore and 2
- Hooks (and mycology journal for the mushroom obviously)


## v1.0.8
No Client Changes
### Poptracker
- Sacred Streamer 2 (Web Area) shown as accessible without Bandaid (Advanced Logic)
- Ancient Relic 1 location checks off Ancient Relic 2 and vice versa
### Logic
- Container of Light Found: Possible with Leaf and Hooks lvl 1
- Ink Augmenter: Should require Sturdy Hooks
- Looks like UT isn’t showing any Advanced Logic checks?
- Secret Augmenter: Possible with nothing
- Cave Blueberry: Not possible with just Leaf and Blade of Power
- Strawberry Augmenter: Possible with Sprint and Band of Elasticity
- Blue Flower Shrine: Possible with just Sprint
- Ancient Relic 1 (Yen): Possible with Sprint, Super Spore, Tool of Mining, and Hooks lvl 1
- Fall: Can enter with nothing
- Explosive Powder 1 Found: Possible with Hooks lvl 1 OR Leaf OR Blade of Power OR Sprint and Super Spore
- Cryptic Caverns Wind Essence: Possible with just leaf
- Dark Cave Energy Spore: Possible with Sprint, Super Spore, Hooks lvl 1, Firestarter Kit
- Lake: Can enter with Hooks lvl 1 OR Leaf OR Blade of Power OR Sprint and Super Spore
- Heart of the Forest: Can clip into the tree with the key fragment. Possible with Hooks lvl 1

## v1.0.7
- Various logic changes
- Fix pop tracker not requiring mycology journal for Macrolepiota Procera
- Make Stuck handling use the nearest out of bounds to the player
- Fix dynamite missing from Elder's Home jar
- Actually fix Ancient Relics Returned
- Correctly flush queue on connection to avoid dropped items while offline
- Fix Clavaria rock collecting mission not re-enabling miner

## v1.0.6
- Various logic changes
- Fix Ancient Relics Returned not working
- Force Smushi Home on finishing base game
- Allow player to use conch in Smushi Home
- Added I'm Stuck button to pause menu
- Pickups respawn to avoid locks with glitch logic
- Speedrun mode no longer forced but always available in menu

## v1.0.5
- Super Spore Found reading as Old String Found?
- Tool of Mining and Super Spore check not shown as in logic in both unitracker and poptracker when having tool of mining and 1 hook (should be), only became in logic after receiving 2 hooks
- Ancient Relic 1 found not checked off in unitracker (did get checked in poptracker)
- Unitracker only showing Forest checks as in logic after collecting 2 blueberries
- Poptracker shows Grove as accessible with <5 lotus flowers
- Falsely received glider after putting one of the hands in for the pink flower check in garden, then immediately lost it again putting the second hand in
- Elder’s Home cave softlocks if you don’t have headlamp
- Could light candles on indigo island without firestarter kit
- Rico Sacred Streamer check becomes inaccessible if you got Super Spore beforehand
- Myrtle Pools Wind Essence (Stump) and Macrolepiota Procera are reachable with Hook, 2 Energy Spore and Super Spore
- Ancient Relic (Brick Ledge) Only Requires Sturdy Hooks, 2 Spore of Energy and Super Spore
- Have 2 “Ancient Relic 2”s, cant get the Ancient Relics Returned check
- Rainbow Augmenter logic requires 1 Ring
- Explosive Powder count bug
- Conch warping in Sacred Holm loses your Capy

## v1.0.4
- Fix accidental save data break on player data awake
- Add advanced logic

## v1.0.3
- Clavaria Augmenter logically requires mine
- Clavaria Augmenter not sending check
- Not enough green crystals in Garden for both purchases
- Can’t dismount Capy after entering and leaving Hidden Lotus
- Screwdriver Purchase not sending check
- Rainbow Augmenter not sending check
- Old String Found not sending check
- Conch UI not working
- Logical conditions on rings incorrect
- Logical conditions on Chungy save incorrect
- Tool of Mining removed during diving and not returned
- Sacred Streamer 2-4 might have gotten mixed up
- Freezes on returning scared streamers
- Logical conditions on Precious Augmenter incorrect
- Can’t deliver the 4 streamers until you’ve done at least one of the streamer locations
- Conch receive not sending check
- Ancient Relic Completion not sending check
- Headlamp not required in all dark caves
- Rico disappears after returning the 4 streamers
- Delivering the streamers doesn’t give goal completion
- Tool of Mining location is possible with just Sturdy Hooks + Tool of Mining
- Ancient Relic 1 is possible with Sturdy Hooks + Tool of Mining
- Blade of Power needed for Rainbow Augmenter
- Blade of Power needed for Secret Opener
- Screwdriver requires glider
- Force Grove entry to need water essence
