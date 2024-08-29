using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : Block {

	public Void() : base(Vector3Int.FloorToInt(Vector3.positiveInfinity), BlockId.VOID) {
	}
}
