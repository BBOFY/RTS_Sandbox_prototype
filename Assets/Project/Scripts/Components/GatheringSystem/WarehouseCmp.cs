using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseCmp : MonoBehaviour, IDropOffPoint {

	[SerializeField]
	private float _objectRadius;

	private float _height;

	private EntityCmp _entity;

	private void Awake() {
		_entity = GetComponent<EntityCmp>();

	}

	/**
	 * Store given resources to inventory
	 */
	public void store(ResourceData resourceData) {
		if (resourceData.type == ResourceType.Nothing) {
			return;
		}

		_entity.owner.inventory.add(resourceData);

	}

	public Vector3 getWorldPosition() {
		return transform.position;
	}

	public float getRadius() {
		return _objectRadius;
	}

	public bool isDestroyed() {
		return !gameObject.activeInHierarchy;
	}

	public Player getOwner() {
		return _entity.owner;
	}
}
