using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<ResourceData> onResourceAmountChanged;
	public void updateResourceView(ResourceData currentAmount) {
		onResourceAmountChanged?.Invoke(currentAmount);
	}
}