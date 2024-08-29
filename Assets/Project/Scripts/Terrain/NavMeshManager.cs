using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public abstract class NavMeshManager {

	protected World _world;

	protected readonly NavMeshSurface _navMeshSurface;
	protected readonly List<NavMeshLink> _links = new ();

	protected NavMeshManager(GameObject container, World world = default) {

		_world = world == default ? world : World.current;

		_navMeshSurface = container.AddComponent<NavMeshSurface>();

		/*
		 *  With buildHeightMesh enabled, the nav mesh links can be placed less precisely to work.
		 *	However, there can be problem with unit movement.
		 */
		// _navMeshSurface.buildHeightMesh = true;

		_navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
		_navMeshSurface.collectObjects = CollectObjects.Volume;
		_navMeshSurface.size = new Vector3(ChunkData.chunkWidth /*- 0.001f*/, ChunkData.chunkHeight + 1.05f, ChunkData.chunkWidth /*- 0.001f*/);
		_navMeshSurface.center = new Vector3(ChunkData.chunkWidth/2f, ChunkData.chunkHeight/2f, ChunkData.chunkWidth/2f);

		/*
		 * Uncomment part of the code to make obstacles from the structures.
		 */
		_navMeshSurface.layerMask = 1 << 0 /*| 1 << 12*/;

		createLinks(container);
	}

	public void updateNavMesh() {
		_navMeshSurface.BuildNavMesh();
	}

	private void createLinks(GameObject container) {

		for (int y = 1; y <= ChunkData.chunkHeight; ++y) {

			float lengthOfLink = 0.025f;

			for (int x = 0; x < ChunkData.chunkWidth; ++x) {

				float centerOfLink = x + 0.5f;
				// float centerOfLink = ChunkData.chunkWidth / 2f;

				var newNavMeshLinkNorth = container.AddComponent<NavMeshLink>();
				addAttributesToLinks(
					ref newNavMeshLinkNorth,
					new Vector3(centerOfLink, y, ChunkData.chunkWidth - lengthOfLink),
					new Vector3(centerOfLink, y, ChunkData.chunkWidth + lengthOfLink)
					);
				_links.Add(newNavMeshLinkNorth);

				var newNavMeshLinkEast = container.AddComponent<NavMeshLink>();
				addAttributesToLinks(
					ref newNavMeshLinkEast,
					new Vector3(ChunkData.chunkWidth - lengthOfLink, y, centerOfLink),
					new Vector3(ChunkData.chunkWidth + lengthOfLink, y, centerOfLink)
				);
				_links.Add(newNavMeshLinkEast);

			}

		}

	}

	private void addAttributesToLinks(ref NavMeshLink link, Vector3 start, Vector3 end, float width = 1f) {
		link.width = width;
		link.bidirectional = true;
		link.autoUpdate = false;
		link.startPoint = start;
		link.endPoint = end;
		link.costModifier = 1;
	}




}
