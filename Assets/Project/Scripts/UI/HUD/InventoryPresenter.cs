using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPresenter : UIPresenter {

	private InventoryViewChannel _viewChannel;

	[SerializeField]
	private ResourceType _resourceTracked;

	[SerializeField]
	private bool _isGatheringInventory;

	protected override void init() {

		_viewChannel = World.current.getPrimaryPlayer().inventoryViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onContentsChanged -= updateContent;
	}

	private void toggle(bool toggle, ResourceType resourceType) {

		if (_isGatheringInventory) {
			text.text = "";
		}

		if (!_isGatheringInventory && _resourceTracked != resourceType) {
			return;
		}

		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onContentsChanged += updateContent;
		}
		else {
			_viewChannel.onContentsChanged -= updateContent;
		}
	}

	private void updateContent(ResourceData resourcesCarried, int capacity) {

		if (_isGatheringInventory && resourcesCarried.amount <= 0) {
			text.text = "";
			return;
		}

		if (_isGatheringInventory) {

			if (resourcesCarried.type is ResourceType.Nothing or ResourceType.Provision or ResourceType.Ammunition) {
				return;
			}

			text.text = $"{resourcesCarried.type.ToString()}: {resourcesCarried.amount}/{capacity}";
		}

		else if (_resourceTracked == resourcesCarried.type) {
			text.text = $"{resourcesCarried.type.ToString()}: {resourcesCarried.amount}/{capacity}";
		}


	}

}
