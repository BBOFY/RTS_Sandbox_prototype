using UnityEngine;

/**
 * Class for storing voxel data. Will be replaced by proper initialization system
 * with ability to configure it in the game.
 */
public static class ChunkData {

	public static readonly int chunkWidth = 8;
	public static readonly int chunkHeight = 12;
	public static readonly int worldSizeInChunks = 8;

	public static int worldSizeInVoxels => chunkWidth * worldSizeInChunks;

	/**
	 * Normal vectors for each side of the voxel.
	 * Order of faces must correspond to ordering in voxelTriangles value
	 */
	public static Vector3Int getFaceNormal(Face face) {
		return face switch {
			Face.Top => Vector3Int.up,
			Face.Bottom => Vector3Int.down,
			Face.Front => Vector3Int.forward,
			Face.Back => Vector3Int.back,
			Face.Right => Vector3Int.right,
			Face.Left => Vector3Int.left,
			_ => Vector3Int.zero
		};
	}

}
