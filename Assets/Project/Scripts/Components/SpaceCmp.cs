using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceCmp : MonoBehaviour {

	private EntityCmp _entity;

	[SerializeField]
	private Vector3Int _sizeInBlocks = Vector3Int.one;

	[SerializeField]
	private Transform _positionReference;

	private Vector3Int _absPos;

	private void Awake() {
		_absPos = Vector3Int.FloorToInt(_positionReference.position);
		_entity = GetComponent<EntityCmp>();

		for (var x = 0; x < _sizeInBlocks.x; ++x) {
			for (var z = 0; z < _sizeInBlocks.z; ++z) {
				for (var y = 0; y < _sizeInBlocks.y; ++y) {
					var newPos = _absPos + new Vector3Int(x, y, z);
					World.current.placeBlock(newPos, new Structure());
				}
			}
		}

	}

	private void OnDisable() {
		for (var x = 0; x < _sizeInBlocks.x; ++x) {
			for (var z = 0; z < _sizeInBlocks.z; ++z) {
				for (var y = 0; y < _sizeInBlocks.y; ++y) {
					var newPos = _absPos + new Vector3Int(x, y, z);
					World.current.destroyBlock(newPos);
				}
			}
		}
	}
}
