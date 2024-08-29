using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResupplierPresenter : MonoBehaviour {

	private ResupplierViewChannel _viewChannel;
	private TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {

		while (World.current.getPrimaryPlayer() == null) {
			yield return null;
		}

		_viewChannel = World.current.getPrimaryPlayer().resupplierViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onResupplierToggle -= updateContent;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onResupplierToggle += updateContent;
		}
		else {
			_viewChannel.onResupplierToggle -= updateContent;
		}
	}

	private void updateContent(bool toggle) {
		text.text = toggle ? "resupplying: On" : "resupplying: Off";
	}
}
