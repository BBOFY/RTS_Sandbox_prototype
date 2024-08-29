using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToProducePresenter : MonoBehaviour {

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
		_viewChannel.onOrderChanged -= updateView;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onOrderChanged += updateView;
		}
		else {
			_viewChannel.onOrderChanged -= updateView;
		}
	}

	private void updateView(UnitProductionOrderSO currentlyProducing) {
		if (currentlyProducing is null) {
			text.text = "";
			return;
		}

		text.text = $"Currently chosen: {currentlyProducing.productName}\n";
		text.text += $"Food: {currentlyProducing.food}, Wood: {currentlyProducing.wood}, Gold: {currentlyProducing.gold}, Iron: {currentlyProducing.iron}";
	}
}
