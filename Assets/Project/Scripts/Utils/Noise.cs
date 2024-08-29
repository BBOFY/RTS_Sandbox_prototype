using UnityEngine;

public static class Noise {
	public static float get2DNoise(Vector2 pos, float offsetX, float offsetY, float scale) {
		return Mathf.PerlinNoise(
			(pos.x + 0.1f) / ChunkData.chunkWidth * scale + offsetX,
			(pos.y + 0.1f) / ChunkData.chunkWidth * scale + offsetY
		);
	}

	// public static float get2DNoise(Vector2 pos, float offset, float scale) {
	//
	// 	if (pos.x == ChunkData.chunkWidth / 2 && pos.y == ChunkData.chunkWidth / 2)
	// 		return 1;
	// 	return 0;
	// }

	public static bool get3DNoise(Vector3 pos, float offsetX, float offsetY, float offsetZ, float scale, float treshHold) {

		float x = (pos.x + offsetX + 0.1f) * scale;
		float y = (pos.y + offsetY + 0.1f) * scale;
		float z = (pos.z + offsetZ + 0.1f) * scale;

		float xy = Mathf.PerlinNoise(x, y);
		float xz = Mathf.PerlinNoise(x, z);
		float yz = Mathf.PerlinNoise(y, z);
		float yx = Mathf.PerlinNoise(y, x);
		float zx = Mathf.PerlinNoise(z, x);
		float zy = Mathf.PerlinNoise(z, y);

		return (xy + xz + yz + yx + zx + zy) / 6f > treshHold;

	}
}
