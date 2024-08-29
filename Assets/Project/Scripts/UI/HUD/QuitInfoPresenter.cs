using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuitInfoPresenter : MonoBehaviour {

	private UIChannel _viewChannel;
	private TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {
		while (World.current.getPrimaryPlayer() is null) {
			yield return null;
		}
		_viewChannel = World.current.getPrimaryPlayer().inputSystem.uiChannel;
		_viewChannel.onGameEnd += endGameInfo;
	}

	private void OnDestroy() {
		_viewChannel.onGameEnd -= endGameInfo;
	}

	private void endGameInfo(Player player) {
		text.text = $"{player.playerColor.ToString()} player lost\n";
		text.text += "Press any key to Quit";
	}
}
