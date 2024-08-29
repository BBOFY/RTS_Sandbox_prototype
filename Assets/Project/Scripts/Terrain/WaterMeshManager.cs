using UnityEngine;

public class WaterMeshManager : MeshManager {

	public WaterMeshManager(GameObject parent)
		: base(parent,
			// Return true only if neighbouring block is not water,
			// is not waterlogged structure, is air or is structure.
			(toRender, neighbour) => {
				if (!toRender.blockSO.isTransparent)
					return false;

				if (neighbour is Void || neighbour.blockId == BlockId.AIR)
					return true;

				if (neighbour.blockSO.isTransparent)
					return false;

				return true;

			}
		) {

		_meshContainerGameObject.name = "WaterMeshContainer";
		// sets layer Terrain_Water
		_meshContainerGameObject.layer = 9;
		_meshRenderer.material = Resources.Load<Material>("Materials/Block Sprites Transparent");
		// excludes from collision detection everything except game objects with "Entity" layer
		_meshCollider.includeLayers = 1 << 12;
		_meshCollider.excludeLayers = Physics.AllLayers & ~(1 << 12);

		// This line of code adds NavMesh for water. Since there are no naval units yet, it is unused
		// _navMeshManager = new WaterNavMeshManager(_meshContainerGameObject);

	}

}
