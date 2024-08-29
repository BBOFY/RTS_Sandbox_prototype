using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "HealthViewChannel", menuName = "Scriptable objects/Channels/UI/HealthView")]
public class HealthViewChannelSO : ScriptableObject {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<int, int> onHealthChanged;
	public void updateHealthView(int currentHealth, int maxHealth) {
		onHealthChanged?.Invoke(currentHealth, maxHealth);
	}
}
