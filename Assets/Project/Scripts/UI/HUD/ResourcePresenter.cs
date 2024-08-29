using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcePresenter : UIPresenter {

	private ResourceViewChannel _viewChannel;

	protected override void init() {

		_viewChannel = World.current.getPrimaryPlayer().resourceViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onResourceAmountChanged -= updateContent;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onResourceAmountChanged += updateContent;
		}
		else {
			_viewChannel.onResourceAmountChanged -= updateContent;
		}
	}

	private void updateContent(ResourceData resourcesCarried) {
		if (resourcesCarried.type == ResourceType.Nothing) {
			text.text = "";
			return;
		}

		text.text = $"{resourcesCarried.type.ToString()}: {resourcesCarried.amount}";

	}
}
