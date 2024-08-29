using Unity.AI.Navigation;
using UnityEngine;

public class WaterNavMeshManager : NavMeshManager {

	public WaterNavMeshManager(GameObject container, World world = default)
		: base(container, world) {

		_navMeshSurface.agentTypeID = AgentTypeID.GetAgentTypeIDByName(AgentTypeID.WATER);

		// layer mask to include Terrain_Water layer
		_navMeshSurface.layerMask |= 1 << 9;

		_links.ForEach((link => link.agentTypeID = AgentTypeID.GetAgentTypeIDByName(AgentTypeID.WATER)));
	}
}
