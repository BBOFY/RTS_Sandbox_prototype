using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResupplyeeViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<bool> onResupplyeeToggle;
	public void toggleResupplyeeView(bool toggle) {
		onResupplyeeToggle?.Invoke(toggle);
	}
}