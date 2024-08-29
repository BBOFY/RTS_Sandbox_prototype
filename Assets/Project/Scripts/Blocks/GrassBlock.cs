using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlock : Block {
	public GrassBlock(Vector3Int absPosition)
		: base(absPosition, BlockId.GRASS_BLOCK) { }
}
