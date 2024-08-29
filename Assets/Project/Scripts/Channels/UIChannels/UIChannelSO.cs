using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UIChannel", menuName = "Scriptable objects/Channels/UIChannel")]
public class UIChannelSO : ScriptableObject {

	public UnityAction onDebugScreenToggle;
	public void toggleDebugScreen() {
		onDebugScreenToggle?.Invoke();
	}

	public UnityAction<ToolType> onToolTypeChanged;
	public void changeToolType(ToolType toolType) {
		onToolTypeChanged?.Invoke(toolType);
	}


	// UI Element toggle
	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	// Show name
	public UnityAction<string> onShowName;
	public void showName(string name) {
		onShowName?.Invoke(name);
	}

	public UnityAction<PlayersResources> onPlayerInventoryChanged;
	public void showPlayersResources(PlayersResources playersResources) {
		onPlayerInventoryChanged?.Invoke(playersResources);
	}

}
