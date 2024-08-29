using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<int, int> onHealthChanged;
	public void updateHealthView(int currentHealth, int maxHealth) {
		onHealthChanged?.Invoke(currentHealth, maxHealth);
	}

	public UnityAction onIsUnderAttack;
	public void isUnderAttack() {
		onIsUnderAttack?.Invoke();
	}

	public UnityAction onNotUnderAttack;
	public void notUnderAttack() {
		onNotUnderAttack?.Invoke();
	}
}
