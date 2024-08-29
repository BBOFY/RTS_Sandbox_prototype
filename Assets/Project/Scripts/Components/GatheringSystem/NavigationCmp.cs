using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationCmp : MonoBehaviour {

	protected EntityCmp _entity;

	private void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
	}

	/**
	 * Finds the closes resource node in nearby chunks
	 */
	public virtual IResource findResourceNodePos(Vector3 from, ResourceType resourceType) {
		if (resourceType == default) {
			return null;
		}

		// Look for node in chunk currently in.
		var foundResource = World.current.getChunk(from).getClosestResource(from, resourceType);
		if (foundResource != null) {
			return foundResource;
		}

		Vector3[] sides = {
			Vector3.forward, Vector3.back, Vector3.left, Vector3.right
		};

		// Check neighbouring chunks for the desired type of resource
		foreach (var side in sides) {

			var checkedChunk = World.current.getChunk(from + ChunkData.chunkWidth * side);
			if (checkedChunk == null) {
				continue;
			}

			foundResource = checkedChunk.getClosestResource(from, resourceType);
			if (foundResource != null) {
				return foundResource;
			}
		}

		return null;
	}

	/**
	 * Finds the closest drop off point. Since only one exist at the moment, the players base building is returned.
	 */
	public virtual IDropOffPoint findDropOffNodePos(Vector3 from) {
		return _entity.owner.getClosestDropOffPoint(from);
	}

}
