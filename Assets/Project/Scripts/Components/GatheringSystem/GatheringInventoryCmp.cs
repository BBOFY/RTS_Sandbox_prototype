using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatheringInventoryCmp : InventoryCmp {

	/**
	 * Add given amount of resource to this inventory.
	 */
	public void addToInventory(ResourceData resourceData) {

		// adding resource type is same
		if (_resourceType == resourceData.type) {
			_currentlyCarrying += resourceData.amount;
		}

		// resource type differ
		else {
			_resourceType = resourceData.type;
			_currentlyCarrying = resourceData.amount;
		}

		// if amount given is above capacity, the surplus will be lost
		if (_currentlyCarrying >= _capacity) {
			_currentlyCarrying = _capacity;
		}
		informAboutChange();
	}

	public ResourceData getInventoryContent() {
		var toReturn = new ResourceData(_resourceType, _currentlyCarrying);
		_currentlyCarrying = 0;
		informAboutChange();
		return toReturn;
	}

	public void resetType() {
		if (_currentlyCarrying == 0) {
			_resourceType = ResourceType.Nothing;
		}
		informAboutChange();
	}


}
