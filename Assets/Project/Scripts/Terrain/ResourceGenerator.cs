
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


public class ResourceGenerator {

	private readonly List<ResourceType> _startingResourceBucket = new () {ResourceType.Food, ResourceType.Food, ResourceType.Gold, ResourceType.Iron};
	private List<ResourceType> _currentResourceBucket = new ();

	private float offsetX;
	private float offsetY;

	public ResourceGenerator() {
		// Here is just some calculation in attempt to make more diversity from the bytes created by hash from seed.
		// This has no mathematical background, it's just first thing that came into my mind.
		offsetX = Mathf.FloorToInt(SeedProcessor.getNextByteValue() * SeedProcessor.getNextByteValue() /
		                           (SeedProcessor.getNextByteValue() + 1f)) / 100f;
		offsetY = Mathf.FloorToInt(SeedProcessor.getNextByteValue() * SeedProcessor.getNextByteValue() /
		                           (SeedProcessor.getNextByteValue() + 1f)) / 100f;
		Random.InitState(SeedProcessor.getNextByteValue());
	}

	/**
	 * Adds resource node prefabs to the game world per chunk.
	 */
	public void generateResources(Chunk chunk) {

		for (int x = 0; x < ChunkData.chunkWidth; ++x) {
			for (int z = 0; z < ChunkData.chunkWidth; ++z) {

				var blockAbsolute2DPos = new Vector2Int(x + chunk.coords.x * ChunkData.chunkWidth, z + chunk.coords.z * ChunkData.chunkWidth);
				var noiseValue = Noise.get2DNoise(blockAbsolute2DPos, offsetX, offsetY, 25f);

				if (noiseValue > 0.8f) {
					var y = chunk.getFirstHeightWithNonAirBlock(new Vector2Int(x, z));

					if (y < 0) {
						continue;
					}

					var resourceLocalPos = new Vector3Int(blockAbsolute2DPos.x, y + 1, blockAbsolute2DPos.y);

					if (_currentResourceBucket.Count == 0) {
						// https://stackoverflow.com/questions/14007405/how-create-a-new-deep-copy-clone-of-a-listt
						_currentResourceBucket = _startingResourceBucket.ConvertAll(resource => resource);
					}

					var typeIdx = Random.Range(0, _currentResourceBucket.Count);
					var type = _currentResourceBucket[typeIdx];
					_currentResourceBucket.RemoveAt(typeIdx);

					string prefabName;

					switch (type) {
						case ResourceType.Food:
							prefabName = "AppleTree";
							break;
						case ResourceType.Iron:
							prefabName = "IronOre";
							break;
						case ResourceType.Gold:
							prefabName = "GoldOre";
							break;
						default:
							throw new ArgumentException("Unknown resource type");
					}

					var resource = Object.Instantiate(Resources.Load($"Prefabs/Structures/{prefabName}"),
						resourceLocalPos,
						Quaternion.identity,
						chunk.chunkObject.transform);

				}
			}
		}
	}


}
