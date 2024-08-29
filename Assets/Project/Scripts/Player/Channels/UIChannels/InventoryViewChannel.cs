using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryViewChannel {

	public UnityAction<bool, ResourceType> onComponentSelected;
	public void toggleElement(bool toggle, ResourceType resourceType) {
		onComponentSelected?.Invoke(toggle, resourceType);
	}

	public UnityAction<ResourceData, int> onContentsChanged;
	public void updateInventoryView(ResourceData currentContent, int capacity) {
		onContentsChanged?.Invoke(currentContent, capacity);
	}

}
