using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[Obsolete("This component was used for gathering system tests and experiments")]
public class DumbNavigationCmp : NavigationCmp {

	// [SerializeField]
	private List<GameObject> _resourceNodes;

	// [SerializeField]
	private List<GameObject> _dropOffNodes;

	private void Start() {
		_resourceNodes = GameObject.FindGameObjectsWithTag("Tree").ToList();
		_dropOffNodes = GameObject.FindGameObjectsWithTag("DropOffPoint" ).ToList();
	}

	public override IResource findResourceNodePos(Vector3 from, ResourceType resourceType) {
		while (true) {

			if (_resourceNodes.Count <= 0) {
				return default;
			}

			int i = Random.Range(0, _resourceNodes.Count);

			if (!_resourceNodes[i].activeInHierarchy) {
				return default;
			}

			var resource = _resourceNodes[i].GetComponent<IResource>();

			if (resource.isDestroyed()) {
				continue;
			}

			return resource;
		}
	}

	public override IDropOffPoint findDropOffNodePos(Vector3 from) {
		while (true) {

			if (_dropOffNodes.Count <= 0) {
				return default;
			}

			int i = Random.Range(0, _dropOffNodes.Count);

			if (!_dropOffNodes[i].activeInHierarchy) {
				return default;
			}

			var resource = _dropOffNodes[i].GetComponent<IDropOffPoint>();

			if (resource.isDestroyed()) {
				continue;
			}

			return resource;
		}
	}

}
