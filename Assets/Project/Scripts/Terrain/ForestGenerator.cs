using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ForestGenerator {

	private World _world;

	private BiomeAttributes _biome;

	public ForestGenerator(World world, BiomeAttributes biome) {
		_world = world;
		_biome = biome;
	}

	/**
	 * Adds tree prefabs to the game world per chunk.
	 */
	public void generateForest(Chunk chunk) {

		for (int x = 0; x < ChunkData.chunkWidth; ++x) {
			for (int z = 0; z < ChunkData.chunkWidth; ++z) {

				var blockAbsolute2DPos = new Vector2Int(x + chunk.coords.x * ChunkData.chunkWidth, z + chunk.coords.z * ChunkData.chunkWidth);
				var noiseValue = Noise.get2DNoise(blockAbsolute2DPos, _biome.treeOffset, _biome.treeOffset, _biome.treeScale);

				if (noiseValue > _biome.treeTreshold) {
					var y = chunk.getFirstHeightWithNonAirBlock(new Vector2Int(x, z));

					if (y < 0) {
						continue;
					}

					var treeLocalPos = new Vector3Int(blockAbsolute2DPos.x, y + 1, blockAbsolute2DPos.y);
					var tree = Object.Instantiate(Resources.Load("Prefabs/Structures/Tree"), treeLocalPos, Quaternion.identity, chunk.chunkObject.transform);
				}
			}
		}
	}

	/**
	 * Returns the height of the first block, that is not air or structure
	 */
	private int getHeightOfColumn(int x, int z, My3DArray<Block> blockMap) {
		for (int y = ChunkData.chunkHeight - 1; y >= 0; --y) {

			if (blockMap[x, y, z] is not Air) {
				return y;
			}

		}

		return 0;
	}
}
