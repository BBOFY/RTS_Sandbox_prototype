using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class represent the block with flowing water, which is passing through structure in the world (tree, building)
/// </summary>
public class WaterLoggedStructure : Water {
	public WaterLoggedStructure(Vector3Int absPosition, int levelToSet) : base(absPosition, levelToSet) {}
}
