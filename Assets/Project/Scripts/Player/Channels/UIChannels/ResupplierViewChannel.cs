using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResupplierViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<bool> onResupplierToggle;
	public void toggleResupplierView(bool toggle) {
		onResupplierToggle?.Invoke(toggle);
	}
}