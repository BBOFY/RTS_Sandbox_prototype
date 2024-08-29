using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductionQueuePresenter : MonoBehaviour {

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
		_viewChannel.onContentsChanged -= updateView;
	}

	private void toggle(bool toggle) {
		gameObject.SetActive(toggle);
		if (toggle) {
			_viewChannel.onContentsChanged += updateView;
		}
		else {
			_viewChannel.onContentsChanged -= updateView;
		}
	}

	private void updateView(UnitProductionOrderSO currentlyProducing, ProductionQueue queue) {
		if (currentlyProducing is null) {
			text.text = "";
			return;
		}

		text.text = $"Producing: {currentlyProducing.productName}\n" +
		            $"In queue: {queue.count()}";

	}

}
