# VTRX | Vector Thrust Redux
### Self rotating thrusters, baby!

> Abbreviation is pronounced "Vee-trix". Forked from [Vector Thrust OS](https://github.com/pyro000/Vector-Thrust-OS) (which itself is a fork of [VectorThrust2](https://github.com/1wsx10/VectorThrust2)). Some credit goes to the authors and contributors of these projects, of which there are many.

Workshop link: [**TBA**]()

Mod.io link: [**TBA**]()

GitHub link: [DlfnAntx/Vector-Thrust-Redux/](https://github.com/DlfnAntx/Vector-Thrust-Redux/)

- - -

## WHAT'S DIFFERENT: VECTOR BEHAVIOUR

- WIP
- If multiple angles of thrusters are on a nacelle, it's rotational direction is chosen by the strongest potential thrust for the *current environment*. The solver will use thrusters of other directions on the nacelle if at a helpful angle, but they will not contribute to the nacelle's rotation.
- If a subgrid is static relative to the main grid (e.g. rotational lock, or attached via connector), it is treated as part of the main grid when counting towards subgrid depth in the solver.
- Gyros are now utilized, and can be subgridded.
- ...

## WHAT'S DIFFERENT: OS BEHAVIOUR

- 'Parking' is the effective on/off switch. I do not recommend turning off the programmable block unless it is set to park.
- Countless improvements upon performance. Some of the additional 'space' this freed up has been put to use with new features.
- Instead of making assumptions about how you control your batteries, tanks, etc., VTRX opts to trigger timers at park/unpark, leaving such decisions up to the engineer.
- There is no pre-packaged horizon UI (which saves performance if that's not your thing). Whiplash141's artificial horizon script ([Steam](https://steamcommunity.com/sharedfiles/filedetails/?id=1721247350), [GitHub](https://github.com/Whiplash141/SpaceEngineersScripts/blob/master/Released/sprite_artificial_horizon.cs)) works rather well and is maintained.
- **NEW:** You can now slave other grids and use their vector thrust too:
    - Functionally speaking, this is implemented by writing 'heartbeat' data to the active controller determined by the game. There can only be one (among connected grids)!
    - Other instances of VTRX with slaving enabled will choose to read the 'heartbeat' data and fulfill it's request.
    - Slaves will *temporarily* inherit some of the Custom Data settings of the master relating to thrust/torque.
    - Slaves with greedy enabled will control gyros/thrusters of their main grid (it's not like their pilot could use them when they aren't the active controller anyway).
    - You cannot master while parked, but you can slave. Slaves will return to their last state when the master disconnects, parks, or is destroyed. (Anything that stops/stales heartbeat data.)
    - **Side effect:** if your ship's VTRX is set to be able to master (default) and you connect to another ship that has a vector thrust script *other than VTRX*, there WILL be fighting.
    - Writing of the heartbeat is a very small but constant performance impact, *even while not controlling a slave*. You may want to disable mastering in extremely performance-constrained scenarios.
    - There is no IGC (if you know what that entails), so antennas aren't required. This guarantees no voodoo dolling of disconnected ships too.
- Cruise control has adjustable speed goal, and can now optionally keep your ship aligned with gravity.
    - Note: while actively master during cruise, there must be an active controller if you want thrust/torque assist from the slave.

- - -

## BASIC SETUP
#### TLDR: START HERE!

1. Add a controller (e.g. remote control, cockpit, etc.) and a programmable block onto the *same* grid.

2. Add thrusters on hinge/rotor subgrids (hereafter referred to as nacelles). Stacking hinges/rotors on other hinges/rotors will only work if one of the layers are locked. Any deeper cannot be guaranteed with VTRX's solver. Limiting the angle of hinges/rotors or locking them is respected by VTRX. Turning thrusters on/off manually is also respected by VTRX.

> (OPTIONAL) Add multiple vector thrusters in the same axis, but *be careful*, you should build the strongest one in the center of mass (To get center of mass go to your Inventory, click `Info` tab then check `Show Center of Mass`).

3. *Please download the script on mod.io or subscribe through the Steam Workshop (the releases here on GitHub are for development redistribution only).* 
   Click `Edit` in the Programmable Block, then click `Browse Scripts`, search for `Vector Thrust Redux` & double click it to load it in. The editor should now populate with a Readme & the condensed code. Click `Check Code`, and if you get `Compilation successful.` you're all good to go & can hit `Ok` (x2).

> The default settings should be good for most use-cases, but feel free to edit variables in the block's `Custom Data` (NOT the code editor within `Edit`).

4. Setup your buttons! Either use the 'Control Module' mod by DIGI ([Steam](https://steamcommunity.com/sharedfiles/filedetails/?id=655948251), [GitHub](https://github.com/THDigi/ControlModule)) or bind the programmable block `Run` onto your hotbar with command arguments (details below).

## COMMANDS
#### Yes there are aliases. I've always been bad at remembering these things.

- `park` - The effective off/on switch. Can engage automatically by default under conservative conditions (e.g. connector attaches to static grid, landing gear locks).
- `gear` - Switches between configurable percentages which throttle maximum acceleration.
- `dampeners` - Normally matches with the ship's toggle, but can be toggled via command if there are no thrusters on the main grid. Keen hates this.
- `setup` - Manually triggers a scan of the ship (which happens automaticially at initialization and potentially during checks every `Update100`).
- `cruise` - Toggles the cruise control. Maintains speed in the direction the ship is pointing, and dampens everything else. Speed can be incremented.
- `...`

## TROUBLESHOOTING YOUR SHIP
#### Where are all of the fun arbitrary constants?! I must configuremax my ship!

I've put a lot of effort into reducing the number of configurable constants. I mean that in a 'less magic numbers = good' way.
> The philosophy is that if a variable needs to be tuned per ship configuration, there's a way to derive it from the ship programmatically. If a variable needs to be tuned to the game engine, then it should come pre-configured. That being said, Clang is a special beast that knows no master.

- The programmable block and the controller you pilot from MUST be on the same grid, and not separated by a rotor/hinge/piston.
- If you feel that the nacelles are acting strangely, the first thing to try is increasing rotor/hinge torque & braking torque.
- VTRX respects what thrusters you turn off. If a nacelle isn't burning in an environment it should be working, try turning it's thrusters on.
- If you can demonstrate that constants in VTRX need adjusting (or find any clear bug, for that matter), please post an issue/pull-request to the [GitHub](https://github.com/DlfnAntx/Vector-Thrust-Redux/issues/new). That way your insight and testing can help everyone.
- If VTRX is modifying or ignoring blocks in a way you don't like, try smaller adjustments via the tagging system (details below) before turning greedy off.

## ADVANCED SETUP: TAGGING
#### Rebuilt from the ground up, for your convenience. No need for commands now.

### 'Use' and 'Ignore' tags
- Want VTRX to use something that it isn't? Include `[VT-use]` in the group name (if you're feeling lazy), block name (if you're more organized), or block Custom Data (if you're more minimalist).
- Don't want VTRX to use something? Include `[VT-ignore]` the same way. This can stop VTRX from reading the locked state of a landing-gear/connector for park, or stop VTRX from rotating (unlocked) hinges/rotors you would like to control manually. Note that subgrid thrusters that don't also have the 'ignore tag' on such a nacelle would still be used if at a helpful angle.
- VTRX doesn't support superposition. Don't mix 'use' & 'ignore' tags on the same thing (e.g. group name says use, Custom Data says ignore).
- The default `greedy` mode is balanced to meet most engineers' needs, but if you would rather add 'use' tags to everything manually, you can turn greedy off in Custom Data.
- Even in greedy mode, thrusters and gyroscopes on the main grid (a.k.a not on nacelles) are read-only outside of cruise-control/slaving, unless you include a 'use' tag. This is to reduce instructions made (performance) and to keep the pilot (more) in control if the programmable block is disabled without parking.

### Triggering Timers
- Parking can trigger timers (configurable)...
- Have a single timer you want triggered on park *&* unpark? Include both tags!

## ADVANCED SETUP: GYROSCOPES
#### Who looks at the center of mass anyway?

- Using gyroscopes within its control, VTRX will attempt to offset torque introduced by nacelles that aren't mirrored with or aligned to the Center of Mass (COM). With greedy mode on (default), only gyroscopes on subgrids are modified.
- To include use of gyroscopes on the main grid, they need the 'use' tag.
- This allows for much more creative ship designs, given you put enough gyros to offset the torque introduced by your 'creativity'.

## ADVANCED SETUP: INFO PANELS
#### While this is optional, it can be helpful for debugging ship design. Or remembering what gear you are in.

1. Place a text panel or screen-containing controller (e.g. cockpit).
2. Include `VT-Redux:0` in Custom Data for blocks with multiple screens, or `[VT-status]` in name, group name, or Custom Data for LCD's.

## ADVANCED SETUP: PERFORMANCE
#### Crazy enough to keep scripts enabled on your large server?

- Don't worry! There's an option for that desired low end gameplay: VTOS introduced "Skipframes=", where each frame is processed & then `N` frames are skipped, improving performance but making the script less responsive the higher the value.
- VTOS recommends putting it no more than `4` while in space and `2` in planet gravity. VTRX has expanded this option to include skipping for groups of tasks, demarked `Update1`, `update10`, & `Update100`. You can be more aggressive with skipping than VTOS' recommendations on VTRX's less frequent tasks groups (e.g. `Update100`), since VTOS scheduled it's things for every tick (aka `Update1`).
- Do be careful though, as setting `Update100` to even `10` means that important system checks would only be done every ~17 minutes (`100 * 10 / 60`)!
- General use shouldn't require messing with these, but if you do I recommend testing in a safe scenario first.

## Known issues with workarounds:
- Currently N/A

- - -

## Contributing

- If you've never worked with scripting in Space Engineers before:
    - Everything is written in the pre-existing language `C#`.
    - See the [official wiki](https://spaceengineers.wiki.gg/wiki/Scripting) for guidance/tutorials.

- Deploying the script requires use of [MDK²](https://malforge.github.io/spaceengineers/mdk2/). It is very easy to get started if you install via the Hub executable.

- I develop in [VSCodium](https://vscodium.com/) (open-source-exclusive VSCode), so here are my steps for diving in given that (though they should be broadly applicable):
    - Fork VTRX's GitHub repository & clone that to your local device.
    - Open the folder `Vector-Thrust-Redux` as a workspace in your IDE (e.g. VSCodium).
    - You're ready to go for editing!
- When ready to test your edits in-game, the code needs to be 'built' and exported to the game (`%APPDATA%\SpaceEngineers\IngameScripts\local\VectorThrustRedux`).
    - This is done in VSCodium by pressing `CTRL+SHIFT+B` and then clicking `build` (use .NET, the correct version should've been installed by MDK²).
    - If your build succeeds, you can hop into the game and browse scripts immediately. If it doesn't, you get more helpful errors than Space Engineers would normally provide.
    - MDK²'s documentation has additional guidance if you get stuck.
- If you're happy with your edits:
    - Make a push to your fork from the local clone via [Git](https://git-scm.com/install/) or similar manager (I prefer [GitKraken](https://www.gitkraken.com/download), which has a nice extension for IDEs).
    - Make a pull-request (PR) to my repository using your fork. See GitHub's guidance [here](https://docs.github.com/en/pull-requests/how-tos/create-pull-requests/creating-a-pull-request-from-a-fork). Please include a description of your changes, and briefly justify why you feel they are needed.