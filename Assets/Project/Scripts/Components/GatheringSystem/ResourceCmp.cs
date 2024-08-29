using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceCmp : MonoBehaviour, IResource, IShowable {

	private EntityCmp _entity;

	[SerializeField]
	private GameObject _resourceNodePrefab;
	[SerializeField]
	private float _radiusOfInteraction;
	[SerializeField]
	private int _amount;
	[SerializeField]
	private ResourceType _type;

	private void Awake() {
		_entity = GetComponent<EntityCmp>();
	}

	/**
	 * Remove a set amount of resource from this source and return this amount with the type of resource
	 */
	public ResourceData getResource() {

		var amountToRemove = 1;

		if (amountToRemove > _amount) {
			amountToRemove = _amount;
		}
		_amount -= amountToRemove;

		var resourcesToReturn = new ResourceData(_type, amountToRemove);

		if (_amount <= 0) {
			gameObject.SetActive(false);
			_entity.state = EntityState.Dying;
		}

		foreach (var player in _entity.getPlayersWhoSelected()) {
			player.resourceViewChannel.updateResourceView(new ResourceData(_type, _amount));
		}
		return resourcesToReturn;
	}

	public Vector3 getWorldPosition() {
		return _resourceNodePrefab.transform.position;
	}

	public float getRadius() {
		return _radiusOfInteraction;
	}


	public bool isDestroyed() {
		return _entity.state is EntityState.Dying;
	}

	public ResourceType getResourceType() {
		return _type;
	}

	public void show(Player player) {
		player.resourceViewChannel.toggleElement(true);
		player.resourceViewChannel.updateResourceView(new ResourceData(_type, _amount));
	}

	public void hide(Player player) {
		player.resourceViewChannel.toggleElement(false);
	}
}
