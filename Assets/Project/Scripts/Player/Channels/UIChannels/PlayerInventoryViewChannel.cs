using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventoryViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<Dictionary<ResourceType, int>> onContentsChanged;
	public void updateInventoryView(Dictionary<ResourceType, int> inventory) {
		onContentsChanged?.Invoke(inventory);
	}

}
