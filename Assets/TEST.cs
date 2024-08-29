using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class TEST : MonoBehaviour {

	[SerializeField]
	private NavMeshSurface _navMeshSurface;

	private void Update() {

		if (Input.GetKeyDown(KeyCode.M)) {
		}

		if (Input.GetKeyDown(KeyCode.B)) {
			_navMeshSurface.BuildNavMesh();
		}

	}

}
