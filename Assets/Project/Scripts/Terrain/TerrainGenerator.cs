using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator {

	private World _world;

	private BiomeAttributes _biome;

	private float offsetX;
	private float offsetY;

	public TerrainGenerator(World world, BiomeAttributes biome) {
		_world = world;
		_biome = biome;
	}

	// Here is just some calculation in attempt to make more diversity from the bytes created by hash from seed.
	// This has no mathematical background, it's just first thing that came into my mind.
	public void init() {
		offsetX = Mathf.FloorToInt(SeedProcessor.getNextByteValue() * SeedProcessor.getNextByteValue() /
		                           (SeedProcessor.getNextByteValue() + 1f)) / 100f;
		offsetY = Mathf.FloorToInt(SeedProcessor.getNextByteValue() * SeedProcessor.getNextByteValue() /
		                           (SeedProcessor.getNextByteValue() + 1f)) / 100f;
	}

	public My2DArray<Chunk> createChunks(int width, int length) {

		var newChunks = new My2DArray<Chunk>(width, length);

		for (var x = 0; x < width; ++x) {
			for (var z = 0; z < length; ++z) {

				var newChunkCoords = new ChunkCoord(x, z);
				var newBlockMap = fillBlockMap(newChunkCoords);

				newChunks[x, z] = new Chunk(newChunkCoords, newBlockMap, _world);
			}
		}

		for (var x = 0; x < width; ++x) {
			for (var z = 0; z < length; ++z) {

				newChunks[x, z].initMeshes();
			}
		}
		return newChunks;
	}

	// Creates chunk's block map.
	private My3DArray<Block> fillBlockMap(ChunkCoord newChunkCoords) {

		var newBlockMap = new My3DArray<Block>(ChunkData.chunkWidth, ChunkData.chunkHeight, ChunkData.chunkWidth);

		for (var x = 0; x < ChunkData.chunkWidth; ++x) {
			for (var z = 0; z < ChunkData.chunkWidth; ++z) {
				var perlinNoiseValue = Noise.get2DNoise(new Vector2Int(x + newChunkCoords.x * ChunkData.chunkWidth, z + newChunkCoords.z * ChunkData.chunkWidth), offsetX, offsetY, 0.5f);



				for (var y = 0; y < ChunkData.chunkHeight; ++y) {
					var newBlockAbsolutePosition = new Vector3Int(x * newChunkCoords.x, y, z * newChunkCoords.z);
					newBlockMap[x, y, z] = BlockTypes.getBlock(
						newBlockAbsolutePosition,
						defineBlockAt(newBlockAbsolutePosition, perlinNoiseValue)
					);
				}
			}
		}

		return newBlockMap;
	}

	/**
	 * Get block ID from the given absolute position. It depends on type of generation.
	 */
	private BlockId defineBlockAt(Vector3Int pos, float perlinTerrainNoise) {

		var y = pos.y;
		/*
		 * Foundation part
		 * This part is independent from terrain generation
		 */

		if (!World.isBlockInsideWorld(pos)) {
			return BlockId.AIR;
		}
		if (y == 0) {
			return BlockId.BEDROCK;
		}

		/*
		 * Primary part
		 * Basic terrain generation
		 */

		int notAffectedHeight = Mathf.FloorToInt(ChunkData.chunkHeight * _biome.solidGroundHeight);
		int affectedHeight = ChunkData.chunkHeight - notAffectedHeight;

		BlockId currentBlockId = BlockId.AIR;

		var generatedHeight = Mathf.FloorToInt(affectedHeight * perlinTerrainNoise);
		generatedHeight += notAffectedHeight ;

		if (y == generatedHeight)
			currentBlockId = BlockId.GRASS_BLOCK;
		else if (y < generatedHeight && y > generatedHeight - 3)
			currentBlockId = BlockId.DIRT;
		else if (y <= generatedHeight - 3)
			currentBlockId = BlockId.STONE;


		/*
		 * Secondary part
		 * Generating nodes of given blocks, or caves if desirable
		 */
		if (currentBlockId == BlockId.STONE) {
			foreach (var lode in _biome.lodes) {
				if (y >= lode.minHeight && y <= lode.maxHeight && Noise.get3DNoise(pos, lode.noiseOffsetX, lode.noiseOffsetY, lode.noiseOffsetZ, lode.noiseScale, lode.threshold)) {
					currentBlockId = lode.blockId;
				}
			}
		}

		return currentBlockId;
	}
}
