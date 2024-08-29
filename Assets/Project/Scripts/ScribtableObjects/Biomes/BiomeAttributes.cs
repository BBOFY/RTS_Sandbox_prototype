using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "BiomeAttributes", menuName = "Scriptable objects/World/BiomeAttributes")]
public class BiomeAttributes : ScriptableObject {

	public string biomeName;

	/**
	 * Height of solid blocks. This height is guaranteed to be the minimal height of the terrain in set biome.
	 */
	[Header("Percentage of the height not affected by terrain generation")]
	[Range(0.0f, 1.0f)]
	public float solidGroundHeight;

	/**
	 * Height of the terrain (between solidGroundHeight and total maximum height), that can be changed via Perlin noise (or other type of generation)
	 */
	// [Range(0, 100)]
	// public int terrainHeight;

	[Range(0.0f, 1.0f)]
	public float terrainScale;

	[Header("Trees")]
	public GameObject treePrefab;
	[Range(0.0f, 1.0f)]
	public float treeTreshold;
	public float treeOffset;
	public float treeScale;


	public Lode[] lodes;

}

[System.Serializable]
public class Lode {
	public string lodeName;
	public BlockId blockId;
	public float noiseOffsetX;
	public float noiseOffsetY;
	public float noiseOffsetZ;
	public float noiseScale;
	public float threshold;
	public float minHeight;
	public float maxHeight;
}
