using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MeshManager {

	protected GameObject _meshContainerGameObject;
	protected MeshRenderer _meshRenderer;
	protected MeshFilter _meshFilter;

	protected NavMeshManager _navMeshManager;

	protected MeshCollider _meshCollider;

	protected int _vertexIndex;

	protected World _world;

	protected Func<Block, Block, bool> _isVisible;

	protected readonly List<Vector3> _vertices = new ();
	protected readonly List<int> _triangles = new ();
	protected readonly List<Vector2> _uvs = new ();

	protected MeshManager(GameObject parent, Func<Block, Block, bool> isVisible) {
		_meshContainerGameObject = new GameObject();
		_meshFilter = _meshContainerGameObject.AddComponent<MeshFilter>();
		_meshRenderer = _meshContainerGameObject.AddComponent<MeshRenderer>();
		_meshCollider = _meshContainerGameObject.AddComponent<MeshCollider>();

		_meshContainerGameObject.transform.SetParent(parent.transform);
		_meshContainerGameObject.transform.position = parent.transform.position;

		_world = World.current;

		_isVisible = isVisible;
	}

	public void updateMesh(My3DArray<Block> blockMap) {
		createMesh(blockMap);
		_navMeshManager?.updateNavMesh();
	}

	public void createMesh(My3DArray<Block> blockData) {

		deleteMesh();

		for (int x = 0; x < ChunkData.chunkWidth; ++x) {
			for (int y = 0; y < ChunkData.chunkHeight; ++y) {
				for (int z = 0; z < ChunkData.chunkWidth; ++z) {

					var blockLocalPos = new Vector3Int(x, y, z);
					addDataToMesh(blockLocalPos, blockData);
				}
			}
		}

		createMesh();
	}

	/**
	 * <param name="localPos">local position of the block in a reference to chunk</param>
	 * Add mesh data about block on said position.
	 *
	 * For every triangle of a voxel, check if its neighbour is solid, i.e. if the face can be visible by the player.
	 * If the face can be seen (has not solid neighbour), then the face vertices are added to mesh data of the chunk.
	 *
	 * Currently supported only cuboid blocks with base at the top of another block.
	 */
	private void addDataToMesh(Vector3Int localPos, My3DArray<Block> blockMap) {

		Block currentBlock = blockMap[localPos];

		if (currentBlock.blockId == BlockId.AIR) {
			return;
		}

		foreach (Face face in Enum.GetValues(typeof(Face))) {

			var checkedBlockPos = localPos + ChunkData.getFaceNormal(face);
			Block checkedNeighbourBlock;
			try {
				checkedNeighbourBlock = blockMap[checkedBlockPos];
			}
			// Neighbour block is outside the chunk
			catch (IndexOutOfRangeException) {
				checkedNeighbourBlock = _world.getBlockAt(Vector3Int.FloorToInt(checkedBlockPos + _meshContainerGameObject.transform.position));
			}

			if (_isVisible(currentBlock, checkedNeighbourBlock))
				addFaceToMesh(localPos, face, currentBlock);

		}
	}

	private void addFaceToMesh(Vector3Int currentBlockPos, Face face, Block currBlock) {
		_triangles.AddRange(currBlock.blockSO.getTris(_vertexIndex));

		if (face == Face.Bottom || currBlock is not ILiquid waterBlock) {
			_vertices.AddRange(currBlock.blockSO.getVerts(face, currentBlockPos));
		}
		else {
			List<Vector3> verts = new ();
			foreach (var vert in currBlock.blockSO.getVerts(face, Vector3Int.zero)) {
				var newVert = new Vector3(vert.x, vert.y * waterBlock.getBlockHeight(), vert.z) + currentBlockPos  ;
				verts.Add(newVert);
			}

			_vertices.AddRange(verts);
		}

		_vertexIndex += currBlock.blockSO.getNumberOfVertices();
		_uvs.AddRange(currBlock.blockSO.getUVs(face));
	}

	/**
	 * Store mesh data and UV data of all blocks to chunk
	 */
	private void createMesh() {
		Mesh mesh = new Mesh();
		mesh.vertices = _vertices.ToArray();
		mesh.triangles = _triangles.ToArray();
		mesh.uv = _uvs.ToArray();
		mesh.RecalculateNormals();

		_meshFilter.mesh = mesh;
		_meshCollider.sharedMesh = mesh;
	}

	private void deleteMesh() {
		_vertices.Clear();
		_triangles.Clear();
		_uvs.Clear();
		_vertexIndex = 0;
	}

	public void updateNavMesh() {
		_navMeshManager?.updateNavMesh();
	}

}




