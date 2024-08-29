using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "BlockPlacingChannel", menuName = "Scriptable objects/Channels/BlockPlacingChannel")]
public class BlockPlacingChannelSO : ScriptableObject {

	public UnityAction<Vector3Int, BlockId> onBlockIdPlaced;
	public void placeBlock(Vector3Int posToPlace, BlockId blockId) {
		onBlockIdPlaced?.Invoke(posToPlace, blockId);
	}

	public UnityAction<Vector3Int, Block> onBlockPlaced;
	public void placeBlock(Vector3Int posToPlace, Block block) {
		onBlockPlaced?.Invoke(posToPlace, block);
	}

	public UnityAction<Vector3Int> onBlockDestroyed;
	public void destroyBlock(Vector3Int target) {
		onBlockDestroyed?.Invoke(target);
	}

}
