# Vector Thrust Redux
### Fork from Vector Thrust OS (which itself is a fork of VectorThrust2)
### Self rotating thrusters, baby!

Workshop link: 

Mod.io link: 

## What's new:
- WIP

## BASIC SETUP
1. Add a controller and programmable block on the same grid.

2. Add thrusters on hinges/rotors (hereafter refferred to as nacelles). Stacking hinges/rotors on other hinges/rotors will only work if one of the layers are locked. Any deeper cannot be guaranteed.

2,1. (OPTIONAL) Add multiple vector thrusters in the same axis, but BE CAREFUL, you have to build the strongest one in the center of mass (To get center of mass go to your Inventory, click "Info" tab then check "Show Center of Mass").

3. Download the script on mod.io or Steam Workshop, search the script and load it into the programmable block (Click Edit), then click 'Save & Exit', no need to modify any variables in the script, all configuration is on Custom Data.

4. Setup your buttons.. either use the 'Control Module' mod by DIGI or bind programmable block "Run" onto your hotbar with arguments (details below).

### IMPORTANT: IF YOU FEEL THAT THE THRUSTERS FEEL STRANGE, INCREASE ROTOR TORQUE AND BRAKING TORQUE


## SCRIPT IS HEAVY FOR YOUR SERVER?

Don't worry! There's an option for that desired low end gameplay: VTOS introduced "Skipframes=", where each frame is processed & N frames are skipped, improving performance but making the script less precise the higher the value. VTOS recommends putting it no more than 4 in space and 2 in planets. I have expanded this option to include skipping for groups of tasks, demarked `Update1`, `update10`, & `Update100`. You can be more aggressive with skipping on less frequent tasks (e.g. `Update100`), though VTOS' recommendations apply to tasks sccheduled for `Update1`.

## INFO PANEL SETUP
#### While this is Optional, it can be helpful for debugging ship design.
1. Place a text panel or screen-containing controller (e.g. cockpit)
2. ...

## Known bugs, and workarounds.
