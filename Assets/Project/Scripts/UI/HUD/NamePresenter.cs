using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NamePresenter : UIPresenter {

	private UIChannel _uiChannel;

	protected override void init() {
		_uiChannel = World.current.getPrimaryPlayer().inputSystem.uiChannel;
		_uiChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_uiChannel.onComponentSelected -= toggle;
		_uiChannel.onShowName -= showName;
	}


	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_uiChannel.onShowName += showName;
		}
		else {
			_uiChannel.onShowName -= showName;
		}
	}

	private void showName(string name) {
		text.text = name;
	}

}
