using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InventoryViewChannel", menuName = "Scriptable objects/Channels/UI/InventoryView")]
public class InventoryViewChannelSO : ScriptableObject {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<ResourceData, int> onContentsChanged;
	public void updateInventoryView(ResourceData currentContent, int capacity) {
		onContentsChanged?.Invoke(currentContent, capacity);
	}

}
