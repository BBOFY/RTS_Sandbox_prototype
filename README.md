# RTS Sandbox prototype
This project was an attempt for combining RTS mechanics with ability to modify terrain and a tiny logistics system. Projects was created in about half a year as a personal and thesis project in one, with no prior experience with Unity engine nor with C#.


## Controlls

H -- help screen with controls

W, A, S, D -- camera movement
R, F -- zoom in/out
Q, E -- rotate camera

TAB -- change terrain tool
C -- toggle resupplyer (for warehouse and cart)
X -- toggle resupplyee

Production:
1 -- switch recipes
2 -- begin production
3 -- cancel current production

LMB:
-- de/select object (no tool)
-- place solid block or water (respective tool selected)
-- remove solid block (respective tool selected)

RMB -- order unit to move to location or attack

Alt+F4 -- quit game

## Mechanics


### Gathering resources

### Building units

### Resupplying
Each unit needs to be constatntly supplied with provisions or ammunition to function properly. When low on provisions, unit moves slowly. Ranged units without ammunition cannot attack.

### Modifying terrain
Terrain can be modified by removing, or placing, blocks onto, or from, the unobstracted terrain. Water can be also placed, which can slow down units or damage some structures.


## Units


### Citizen

### Gun soldier

### Pike soldier

### Saber soldier

### Resupply cart



## Used code

[Mesh combiner](https://github.com/pharan/Unity-MeshSaver/blob/master/MeshSaver/Editor/MeshSaverEditor.cs)\
[Mesh combiner example](https://gist.github.com/yigiteren/551f693e62b5f39baaba7536fa2c4680)\
[Agent control](https://answers.unity.com/questions/1650130/change-agenttype-at-runtime.html)\
[Agent link mover](https://github.com/Unity-Technologies/NavMeshComponents/blob/master/Assets/Examples/Scripts/AgentLinkMover.cs)
[Interface searching](https://forum.unity.com/threads/how-to-get-all-components-on-an-object-that-implement-an-interface.101028/)

