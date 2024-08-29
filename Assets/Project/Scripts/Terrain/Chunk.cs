using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Chunk {

	public readonly ChunkCoord coords;

	private LandMeshManager _landMesh;
	private WaterMeshManager _waterMesh;

	private GameObject _chunkObject;
	public GameObject chunkObject => _chunkObject;

	private readonly World _world;

	/**
	 * 3D array of bytes. If value is present, then the value represents the voxel type inside the chunk.
	 * Only visible faces of voxels are drawn.
	 */
	private readonly My3DArray<Block> _blockMap;

	private readonly List<IResource> _resources = new ();

	/**
	 * Gets absolute position of a chunk
	 */
	public Vector3Int position {get;}

	public Chunk(ChunkCoord coords, My3DArray<Block> blockMap, World world) {
		_world = world;
		this.coords = coords;
		_blockMap = blockMap;

		position = new Vector3Int(coords.x, 0, coords.z) * ChunkData.chunkWidth;

		initGameObject();
	}

	private void initGameObject() {
		// Create chunk as game object and assign required components to it
		_chunkObject = new GameObject {
			name = $"Chunk ({coords.x}, {coords.z})"
		};

		_chunkObject.transform.SetParent(_world.transform);
		_chunkObject.transform.position = position;

	}

	public void loadResources(IEnumerable<GameObject> allResources) {
		foreach (var resource in from resource in allResources let resPos = resource.transform.position
		         where resPos.x >= coords.x * ChunkData.chunkWidth
			         && resPos.x < (coords.x + 1) * ChunkData.chunkWidth
			         && resPos.z >= coords.z * ChunkData.chunkWidth
			         && resPos.z < (coords.z + 1) * ChunkData.chunkWidth select resource) {
			_resources.Add(resource.GetComponentInChildren<IResource>());
		}
	}

	public IResource getClosestResource(Vector3 absPos, ResourceType resourceType) {

		IResource closestResource = null;

		var minDistance = float.PositiveInfinity;

		for (var i = _resources.Count - 1; i >= 0; --i) {

			var currResource = _resources[i];


			if (currResource == null || currResource.isDestroyed()) {
				_resources.RemoveAt(i);
				continue;
			}

			if (currResource.getResourceType() != resourceType) {
				continue;
			}

			var currResourcePos = new Vector2(currResource.getWorldPosition().x, currResource.getWorldPosition().z);
			var currDistance = Vector2.Distance(new Vector2(absPos.x, absPos.z), currResourcePos);

			if (currDistance < minDistance) {
				minDistance = currDistance;
				closestResource = currResource;
			}

		}

		return closestResource;
	}

	public void initMeshes() {
		_landMesh = new LandMeshManager(_chunkObject);
		_waterMesh = new WaterMeshManager(_chunkObject);
	}

	public Block getBlockFromAbsolutePos(Vector3Int pos) {
		return _blockMap[pos.x - position.x, pos.y, pos.z - position.z];
	}

	/**
	 * Checks, if voxel on said coordinates is inside this chunk.
	 * Coordinates are local in reference to chunk
	 */
	public static bool isVoxelInsideChunk(Vector3Int localPos) {
		return ! (localPos.x < 0 || localPos.x > ChunkData.chunkWidth - 1 ||
		          localPos.y < 0 || localPos.y > ChunkData.chunkHeight - 1 ||
		          localPos.z < 0 || localPos.z > ChunkData.chunkWidth - 1);
	}

	public void updateMesh() {
		_landMesh.updateMesh(_blockMap);
		_waterMesh.updateMesh(_blockMap);
	}

	public void updateNavMesh() {
		_landMesh.updateNavMesh();
		_waterMesh.updateNavMesh();
	}

	public void editChunk(Vector3Int pos, Block newBlock) {
		// Extracting position from absolute position to local position in reference to chunk
		pos.x -= position.x;
		pos.z -= position.z;

		var oldBlock = _blockMap[pos.x, pos.y, pos.z];
		_blockMap[pos.x, pos.y, pos.z] = newBlock;

		if (oldBlock.blockSO.isSolid || newBlock.blockSO.isSolid) {
			_landMesh.updateMesh(_blockMap);
		}

		if (oldBlock.blockSO.isTransparent || newBlock.blockSO.isTransparent) {
			_waterMesh.updateMesh(_blockMap);
			updateNavMesh();
		}

		updateNeighbourChunks(new Vector3Int(pos.x, pos.y, pos.z), newBlock, oldBlock);
		// toUpdate(toUpdateLand, toUpdateWater);
	}

	/**
	 * Updates meshes of neighbouring chunks if necessary
	 */
	private void updateNeighbourChunks(Vector3Int currentBlock, Block newBlock, Block oldBlock) {

		foreach (Face face in Enum.GetValues(typeof(Face))) {
			var neighbourBlockPos = currentBlock + ChunkData.getFaceNormal(face);

			if (isVoxelInsideChunk(neighbourBlockPos)) continue;

			var neighbourBlock = _world.getBlockAt(neighbourBlockPos + position);

			if (neighbourBlock == null) continue;

			var neighbourChunk = _world.getChunk(neighbourBlockPos + position);

			if (neighbourBlock.blockId == BlockId.AIR) {
				continue;
			}

			if (neighbourBlock.blockSO.isSolid) {

				if (newBlock.blockSO.isTransparent && oldBlock.blockSO.isTransparent) {
					continue;
				}
				neighbourChunk._landMesh.updateMesh(neighbourChunk._blockMap);
			}

			else if (neighbourBlock.blockSO.isTransparent) {

				if (newBlock.blockSO.isTransparent || oldBlock.blockSO.isTransparent) {
					continue;
				}
				neighbourChunk._waterMesh.updateMesh(neighbourChunk._blockMap);
			}

		}
	}

	/**
	 * Returns the height of the first block, that is not air, on given position
	 */
	public int getFirstHeightWithNonAirBlock(Vector2Int pos, bool ignoreStructures = false) {

		for (int y = ChunkData.chunkHeight - 1; y >= 0; --y) {

			var checkedBlock = _blockMap[pos.x, y, pos.y];

			if (checkedBlock is Structure) {
				if (ignoreStructures) {
					return y - 1;
				}
				return -1;
			}

			if (checkedBlock is not Air and not Structure) {
				return y;
			}

		}

		return 0;
	}


}
