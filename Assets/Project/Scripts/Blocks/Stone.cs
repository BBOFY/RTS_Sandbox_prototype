using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Block {
	public Stone(Vector3Int absPosition)
		: base(absPosition, BlockId.STONE) { }
}
