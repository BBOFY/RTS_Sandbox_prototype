using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResupplyeePresenter : MonoBehaviour {

	private ResupplyeeViewChannel _viewChannel;
	private TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {

		while (World.current.getPrimaryPlayer() == null) {
			yield return null;
		}

		_viewChannel = World.current.getPrimaryPlayer().resupplyeeViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onResupplyeeToggle -= updateContent;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onResupplyeeToggle += updateContent;
		}
		else {
			_viewChannel.onResupplyeeToggle -= updateContent;
		}
	}

	private void updateContent(bool toggle) {
		text.text = toggle ? "be supplied: On" : "be supplied: Off";
	}
}
