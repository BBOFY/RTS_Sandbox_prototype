using UnityEngine;

public class LandMeshManager : MeshManager {

	public LandMeshManager(GameObject parent)
		: base(parent,
			// Return true only if neighbouring block is water, air or structure.
			(toRender, neighbour) => {
				if (!toRender.blockSO.isSolid)
					return false;

				if (neighbour is Void)
					return true;

				if (neighbour.blockSO.isTransparent)
					return true;

				return false;
			})
	{
		_meshContainerGameObject.name = "LandMeshContainer";
		// sets layer Terrain_Land
		_meshContainerGameObject.layer = 8;
		_meshRenderer.material = Resources.Load<Material>("Materials/Block Sprites");

		_navMeshManager = new LandNavMeshManager(_meshContainerGameObject);
	}

}
