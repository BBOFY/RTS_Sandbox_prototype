using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockShape", menuName = "Scriptable objects/World/BlockShape")]
public class BlockShapeSO : ScriptableObject {

	public int numberOfVertexes {
		get {return 4;}
	}

	/**
	 * Array of 8 vertices of a cube.
	 */
	private static readonly Vector3[] voxelVertices = {
		new (0f, 0f, 0f),
		new (1f, 0f, 0f),
		new (1f, 1f, 0f),
		new (0f, 1f, 0f),
		new (0f, 0f, 1f),
		new (1f, 0f, 1f),
		new (1f, 1f, 1f),
		new (0f, 1f, 1f)
	};


	/**
	 * 2D array of vertex references to draw a triangles. First dimension is number of faces,
	 * second dimension is number of vertex references for one face.
	 * The voxel has 6 faces, each having 4 vertex references to draw 2 triangles.
	 * These two triangles share two vertexes.
	 */
	private static readonly int[,] voxelFaces = {
		{7, 6, 3, 2}, // top
		{5, 4, 1, 0}, // bottom
		{3, 2, 0, 1}, // back
		{6, 7, 5, 4}, // front
		{2, 6, 1, 5}, // right
		{7, 3, 4, 0}  // left
	};

	public static List<Vector3> getVerts(Face face, Vector3Int localPos) {
		var faceId = (int)face;
		List<Vector3> toReturn = new () {
			voxelVertices[voxelFaces[faceId, 0]] + localPos,
			voxelVertices[voxelFaces[faceId, 1]] + localPos,
			voxelVertices[voxelFaces[faceId, 2]] + localPos,
			voxelVertices[voxelFaces[faceId, 3]] + localPos
		};
		return toReturn;
	}

	/**
	 * Adding vertex indices for mesh to create triangles
	 */
	public static List<int> getTris(int currentVert) {
		List<int> toReturn = new () {
			currentVert,
			currentVert + 1,
			currentVert + 2,
			currentVert + 2,
			currentVert + 1,
			currentVert + 3
		};
		return toReturn;
	}

}
