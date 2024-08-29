using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : Block {

	public Air()
		: base(Vector3Int.FloorToInt(Vector3.positiveInfinity), BlockId.AIR) { }
}
