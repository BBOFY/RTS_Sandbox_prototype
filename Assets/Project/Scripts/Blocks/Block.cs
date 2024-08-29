using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block {

	public readonly BlockSO blockSO;
	public readonly BlockId blockId;
	protected readonly Vector3Int _absPosition;

	protected Block(Vector3Int absPosition, BlockId blockId) {
		blockSO = World.blockTypes.getBlockType(blockId);
		this.blockId = blockId;
		_absPosition = absPosition;
	}

}
