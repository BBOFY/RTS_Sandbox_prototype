using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ResourceViewChannel", menuName = "Scriptable objects/Channels/UI/ResourceView")]
public class ResourceViewChannelSO : ScriptableObject {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<int, int> onResourceAmountChanged;
	public void updateResourceView(int currentAmount, int capacity) {
		onResourceAmountChanged?.Invoke(currentAmount, capacity);
	}
}