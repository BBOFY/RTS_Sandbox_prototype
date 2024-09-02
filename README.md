# RTS Sandbox prototype
This project was an attempt for combining RTS mechanics with ability to modify terrain and a tiny logistics system. Projects was created in about half a year as a personal and thesis project in one, with no prior experience with Unity engine nor with C#.

Built game executable `rts_game.exe` is located in `Build` folder. It can be run in Windows 10/11 64-bit operating system.


## Controlls

H -- help screen with controls

W, A, S, D -- camera movement\
R, F -- zoom in/out\
Q, E -- rotate camera

TAB -- change terrain tool\
C -- toggle resupplyer (for warehouse and cart)\
X -- toggle resupplyee

Production:
1 -- switch recipes\
2 -- begin production\
3 -- cancel current production

LMB:\
-- de/select object (no tool)\
-- place solid block or water (respective tool selected)\
-- remove solid block (respective tool selected)

RMB -- order unit to move to location or attack

Alt+F4 -- quit game

## Mechanics

Apart from classic RTS mechanics like gathering resources via citizens and creating units in building, there is also need to resupply the units and ability to modify world terrain.

### Resupplying
Each unit needs to be constatntly supplied with provisions or ammunition to function properly. When low on provisions, unit moves slowly. Ranged units without ammunition cannot attack.
You can toggle on/off unit being resupplied with X.
Resupplying other units can be toggled on/off with C

### Modifying terrain
Terrain can be modified by removing, or placing, blocks onto, or from, the unobstracted terrain. Water can be also placed, which can slow down units or damage some structures.


## Units
All units are produced in only available building (warehouse), that each player starts with. Warehouse is also place, where citizens drop off gathered resources. They can also attack enemies nearby. Units can be resupplied by the warehouse.

### Citizen
Can harvest resources. It can also attack enemies if ordered manually.

### Gun soldier
Capable of attacking at a small distance. Has damage bonus against pike soldiers.

### Pike soldier
Capable of close range attack. Has damage bonus against saber soldiers.

### Saber soldier
Capable of melee attack. Has damage bonus against gun soldiers.

### Resupply cart
Cannot attack. Used for resupplying units far from the warehouse.


## Used code
Links where I got some of the code from. 

[Mesh combiner](https://github.com/pharan/Unity-MeshSaver/blob/master/MeshSaver/Editor/MeshSaverEditor.cs)\
[Mesh combiner example](https://gist.github.com/yigiteren/551f693e62b5f39baaba7536fa2c4680)\
[Agent control](https://answers.unity.com/questions/1650130/change-agenttype-at-runtime.html)\
[Agent link mover](https://github.com/Unity-Technologies/NavMeshComponents/blob/master/Assets/Examples/Scripts/AgentLinkMover.cs)
[Interface searching](https://forum.unity.com/threads/how-to-get-all-components-on-an-object-that-implement-an-interface.101028/)

