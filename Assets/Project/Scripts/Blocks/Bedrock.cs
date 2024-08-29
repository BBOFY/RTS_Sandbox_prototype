using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bedrock : Block {
	public Bedrock(Vector3Int absPosition)
		: base(absPosition, BlockId.BEDROCK) { }
}
