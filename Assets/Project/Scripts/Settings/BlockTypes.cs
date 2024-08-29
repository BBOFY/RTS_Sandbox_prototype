using UnityEngine;

/// <summary>
/// This class defines all available block types via Unity editor.
/// </summary>
public class BlockTypes : MonoBehaviour {

	public static BlockTypes current;

	[SerializeField]
	private BlockSO AirSO;
	[SerializeField]
	private BlockSO BedrockSO;
	[SerializeField]
	private BlockSO StoneSO;
	[SerializeField]
	private BlockSO DirtSO;
	[SerializeField]
	private BlockSO GrassBlockSO;
	[SerializeField]
	private BlockSO WaterSO;

	public BlockSO airSO => AirSO;
	public BlockSO bedrockSO => BedrockSO;
	public BlockSO stoneSO => StoneSO;
	public BlockSO dirtSO => DirtSO;
	public BlockSO grassBlockSO => GrassBlockSO;
	public BlockSO waterSO => WaterSO;

	private void Awake() {
		current = this;
	}

	// Return block type via desired block id
	public BlockSO getBlockType(BlockId blockId) {
		return blockId switch {
			BlockId.AIR => AirSO,
			BlockId.BEDROCK => BedrockSO,
			BlockId.STONE => StoneSO,
			BlockId.DIRT => DirtSO,
			BlockId.GRASS_BLOCK => GrassBlockSO,
			BlockId.WATER => WaterSO,
			BlockId.STRUCTURE => AirSO,
			_ => AirSO
		};
	}

	// Create and return new block with desired type and location.
	public static Block getBlock(Vector3Int absPos, BlockId blockId) {
		return blockId switch {
			BlockId.AIR => new Air(),
			BlockId.BEDROCK => new Bedrock(absPos),
			BlockId.STONE => new Stone(absPos),
			BlockId.DIRT => new Dirt(absPos),
			BlockId.GRASS_BLOCK => new GrassBlock(absPos),
			BlockId.WATER => new Water(absPos, 8),
			BlockId.WATER_SOURCE => new WaterSource(absPos),
			_ => null
		};
	}

}
