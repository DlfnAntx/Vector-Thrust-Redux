# Vector Thrust Redux
### Self rotating thrusters, baby!
#### Fork from Vector Thrust OS (which itself is a fork of VectorThrust2)

Workshop link: **TBA**

Mod.io link: **TBA**

- - -

## WHAT'S NEW: VECTOR BEHAVIOUR

- WIP
- ...

- - -

## BASIC SETUP

1. Add a controller (e.g. remote control, cockpit, etc.) and a programmable block onto the same grid.

2. Add thrusters on hinge/rotor subgrids (hereafter referred to as nacelles). Stacking hinges/rotors on other hinges/rotors will only work if one of the layers are locked. Any deeper cannot be guaranteed.

- (OPTIONAL) Add multiple vector thrusters in the same axis, but BE CAREFUL, you have to build the strongest one in the center of mass (To get center of mass go to your Inventory, click "Info" tab then check "Show Center of Mass").

3. Please download the script on mod.io or subscribe through the Steam Workshop (the releases here on Github are for development redistribution only). Click `Edit` in the Programmable Block, the click `Browse Scripts`, search the script & load it in. The editor should now populate with a Readme & the condensed code. Click `Check Code`, and if you get `Compilation successful.` you're all good to go & can hit `Ok` (x2).

- The default settings should be good for most use-cases, but feel free to edit variables in the block's `Custom Data` (not that code editor).

4. Setup your buttons.. either use the 'Control Module' mod by DIGI or bind programmable block "Run" onto your hotbar with arguments (details below).

## COMMANDS

`gear` - Switches between configurable percentages which throttle maximum acceleration.
`park` - The effective off/on switch. Can engage automatically by default under conservative conditions (e.g. connector attaches to static grid).
...

## TAGGING

Rebuilt from the ground up, for your convenience. No need for commands now.
Want VTRX to use something that it isn't? Include `[VT-use]` in the group name (if you're feeling lazy), block name (if you're more organized), or block Custom Data (if you're just that kind of minimalist).
Don't want VTRX to use something? Include `[VT-ignore]` the same way.
The default `greedy` mode is balanced to meet most engineer's needs, but if you would rather add 'use' tags to things manually, you can turn it off.
> Even in greedy mode, thrusters and gyroscopes on the main grid (a.k.a not on nacelles) are read-only outside of cruise mode, unless you include a 'use' tag.

## TROUBLESHOOTING YOUR SHIP

I've put a lot of effort into reducing the number of configurable constants. I mean that in a 'less magic numbers = good' way.
The philosophy is that if a variable needs to be tuned per ship configuration, there's a way to derrive it from the ship programmaticially. If a variable needs to be tuned to the game engine, then it should come pre-configured.
That being said, Clang is a special beast that knows no master:

- If you feel that the thrusters are acting strange, the first thing to try is increasing rotor torque & braking torque.
- ...
- If you can demonstrate that constants in VTRX tuned to the game engine need adjusting, please post an issue/pull-request to the Github.

## ADVANCED SETUP: INFO PANELS
#### While this is Optional, it can be helpful for debugging ship design. Or remembering what gear you are in.

1. Place a text panel or screen-containing controller (e.g. cockpit)
2. Include `VT-Redux:0` in CustomData for blocks with multiple screens, or `[VT-status]` for LCD's.

## ADVANCED SETUP: PERFORMANCE
#### Script is heavy for your server?

Don't worry! There's an option for that desired low end gameplay: VTOS introduced "Skipframes=", where each frame is processed & then `N` frames are skipped, improving performance but making the script less responsive the higher the value.
VTOS recommends putting it no more than `4` while in space and `2` in planet gravity. I have expanded this option to include skipping for groups of tasks, demarked `Update1`, `update10`, & `Update100`. You can be more aggressive with skipping on less frequent tasks groups (e.g. `Update100`) than VTOS' recommendations, which were for tasks scheduled for `Update1`.

## Known issues with workarounds:
 - ...