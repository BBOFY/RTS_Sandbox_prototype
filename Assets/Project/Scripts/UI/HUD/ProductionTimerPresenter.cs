using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductionTimerPresenter : MonoBehaviour {

	private readonly string NOT_ENOUGH_RESOURCES_MSG = "Not enough resources!";
	private Coroutine _notEnoughResourcesCoroutine;


	private ProductionQueueViewChannel _viewChannel;
	private TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {
		while (World.current.getPrimaryPlayer() is null) {
			yield return null;
		}
		_viewChannel = World.current.getPrimaryPlayer().productionQueueViewChannel;
		_viewChannel.onComponentSelected += toggle;
	}

	private void OnDestroy() {
		_viewChannel.onComponentSelected -= toggle;
		_viewChannel.onProductionTimeCahnged -= updateView;
		_viewChannel.onNotEnoughResources -= showNotEnoughResources;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onProductionTimeCahnged += updateView;
			_viewChannel.onNotEnoughResources += showNotEnoughResources;
		}
		else {
			_viewChannel.onProductionTimeCahnged -= updateView;
			_viewChannel.onNotEnoughResources -= showNotEnoughResources;
		}
	}

	private void updateView(int curr, int max) {

		text.text = "";
		if (max <= 0 || curr >= max) {
			return;
		}

		text.text += "|";

		for (int i = 0; i < curr; ++i) {
			text.text += "..";
		}

		for (int i = curr; i < max; ++i) {
			text.text += "  ";
		}

		text.text += "|";
	}

	private void showNotEnoughResources() {
		_notEnoughResourcesCoroutine = StartCoroutine(showNotEnoughResourcesProcess());
	}

	private IEnumerator showNotEnoughResourcesProcess() {

		if (_notEnoughResourcesCoroutine is not null) {
			yield break;
		}

		_viewChannel.onProductionTimeCahnged -= updateView;
		var prevText = text.text;
		text.text = NOT_ENOUGH_RESOURCES_MSG;
		yield return new WaitForSeconds(1f);
		text.text = prevText;
		_viewChannel.onProductionTimeCahnged += updateView;
		_notEnoughResourcesCoroutine = null;
	}
}
