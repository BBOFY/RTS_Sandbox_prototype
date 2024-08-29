using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HealthPresenter : UIPresenter {

	private HealthViewChannel _viewChannel;

	protected override void init() {
		_viewChannel = World.current.getPrimaryPlayer().healthViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onHealthChanged -= updateHealth;
	 }

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onHealthChanged += updateHealth;
		}
		else {
			_viewChannel.onHealthChanged -= updateHealth;
		}
	}

	private void updateHealth(int currentHealth, int maxHealth) {
		text.text = $"Health: {currentHealth}/{maxHealth}";
	}
}
