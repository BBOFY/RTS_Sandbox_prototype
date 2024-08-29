using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : Block {
	public Dirt(Vector3Int absPosition)
		: base(absPosition, BlockId.DIRT) { }
}
