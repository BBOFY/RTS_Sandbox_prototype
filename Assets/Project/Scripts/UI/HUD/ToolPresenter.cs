using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

public class ToolPresenter : UIPresenter {

	private UIChannel _uiChannel;

	protected override void init() {
		_uiChannel = World.current.getPrimaryPlayer().inputSystem.uiChannel;
		_uiChannel.onToolTypeChanged += updateToolView;
		text.text = "Tool: None";
	}

	private void OnDestroy() {
		_uiChannel.onToolTypeChanged -= updateToolView;
	}

	private void updateToolView(ToolType toolType) {
		text.text = $"Tool: {toolType}";
	}
}
