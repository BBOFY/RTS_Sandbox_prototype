using UnityEngine;

/**
 * Position of chunk within chunk map
 */
public readonly struct ChunkCoord {

	public int x { get; }
	public int z { get; }

	public ChunkCoord(int x = 0, int z = 0) {
		this.x = x;
		this.z = z;
	}

	/**
	 * Creates chunk coordinates of the chunk from the absolute position of the block
	 */
	public ChunkCoord(Vector3Int pos) {
		x = pos.x / ChunkData.chunkWidth;
		z = pos.z / ChunkData.chunkWidth;
	}

	public bool equals(object o) {
		if (o is not ChunkCoord other)
			return false;

		return x == other.x && z == other.z;
	}
}
