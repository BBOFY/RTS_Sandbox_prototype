using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Block", menuName = "Scriptable objects/World/Block")]
public class BlockSO : ScriptableObject {
	[Header("Properties")]
	public string blockName;
	public BlockId blockId;
	public bool isSolid;
	public bool isTransparent;
	public int hardness;

	[Header("Sprites")]

	public Sprite topFace;
	public Sprite bottomFace;
	public Sprite frontFace;
	public Sprite backFace;
	public Sprite rightFace;
	public Sprite leftFace;

	[Header("Scriptable objects")]
	public BlockShapeSO blockShape;

	public List<Vector3> getVerts(Face face, Vector3Int localPos) {
		return BlockShapeSO.getVerts(face, localPos);
	}

	public List<int> getTris(int currentVert) {
		return BlockShapeSO.getTris(currentVert);
	}

	// ReSharper disable Unity.PerformanceAnalysis
	public Vector2[] getUVs(Face face) {
		switch (face) {
			case Face.Top:
				return topFace.uv;
			case Face.Bottom:
				return bottomFace.uv;
			case Face.Front:
				return frontFace.uv;
			case Face.Back:
				return backFace.uv;
			case Face.Right:
				return rightFace.uv;
			case Face.Left:
				return leftFace.uv;
			default:
				Debug.LogError("Error in getTextureId");
				return new Vector2[]{};
		}

	}

	public int getNumberOfVertices() {
		return blockShape.numberOfVertexes;
	}
}
