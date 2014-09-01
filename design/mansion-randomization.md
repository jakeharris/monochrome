Mansion shuffle
===============
*Everyone likes a little shufflin' in their lives*

*WIP*

We want a structure that says, whenever the player closes a door, if that seals all entrances to the room, and there are no enemies in the room, shuffle all of the rooms in the mansion. We also want the mansion's initial layout to be random. 

Our objective is to punish the player for creating certainty. They may be infinitely safe in their closed room -- but their temporary invulnerability comes at the cost of a dramatic chaos.

Foreseeable problems include room collision, unit loss, and patrol route disruption.

Shuffling works as follows:
1. If the number of rooms fed to the algorithm is greater than the number required, pick out the required number of rooms. We are as of yet uncertain if particular rooms will always be required.
2. Place the room with the most doorways/entrances at 0,0. Its rotation isn't relevant, though changing it (or not) might provide interesting atmosphere depending on how dramatic our lighting is.
3. Place the room with the next most doorways or link-able entrances connected to one of the entrances to the root room, with its doorways or entrances *likely* pointed *away from* the other doorways on the root room.
4. Repeat step 3 until all rooms are placed.

Process to fix errors to come.

## Linguistic anaylsis:
Room
Entrance (Interface? _Interstice_)
Root

## Classes
### Room
- array of interstices
- number of interstices (technically calculable on the fly, but reducing iteration is nice since it shouldn't ever change for a room)
- walls/ground
- game objects
- is root?

### Interstice
- position relative to a room
- width (and maybe height?)

### Randomizer
- array of rooms
- root room

## Process
*Steps with 0 in their number are steps that only happen once the game has begun, not during the initial map generation.*

### Step 0: The Interstice
Replace all exits to the room the user is in with a random instance of The Interstice until we can finish the shuffle, or until the user walks to an exit of the Interstice, whichever happens first.

### Step 1: Topography
1. Determine root room.
a. Loop through rooms, asking each room how many interstices each has.
b. If we have no root yet, or if the current room has more interstices than the current root, replace the current root with the current room.
2. Thing.

### Step 2: Geography
### Step 3: Resolution
