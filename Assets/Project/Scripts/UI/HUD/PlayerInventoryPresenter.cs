using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventoryPresenter : MonoBehaviour {

	private UIChannel _viewChannel;
	private TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {

		while (World.current.getPrimaryPlayer() == null) {
			yield return null;
		}

		_viewChannel = World.current.getPrimaryPlayer().inputSystem.uiChannel;
		// _viewChannel.onComponentSelected += toggle;
		_viewChannel.onPlayerInventoryChanged += updateContent;
		World.current.getPrimaryPlayer().initViews();
	}

	private void OnDestroy() {
		_viewChannel.onPlayerInventoryChanged -= updateContent;
	}

	private void updateContent(PlayersResources playersResources) {

		text.text = $"{playersResources.population}/{playersResources.popcap} - Population\n" +
		            $"{playersResources.food} - Food\n" +
		            $"{playersResources.wood} - Wood\n" +
		            $"{playersResources.gold} - Gold\n" +
		            $"{playersResources.iron} - Iron\n" +
		            $"{playersResources.provisions} - Provisions\n" +
		            $"{playersResources.ammunition} - Ammo";


	}

}
