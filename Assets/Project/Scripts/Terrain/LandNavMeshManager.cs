using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandNavMeshManager : NavMeshManager {
	public LandNavMeshManager(GameObject container, World world = default) : base(container, world) {
		_navMeshSurface.agentTypeID = AgentTypeID.GetAgentTypeIDByName(AgentTypeID.LAND);

		// layer mask to include Terrain_Land layer
		_navMeshSurface.layerMask |= 1 << 8;

		_links.ForEach((link => link.agentTypeID = AgentTypeID.GetAgentTypeIDByName(AgentTypeID.LAND)));

	}
}
