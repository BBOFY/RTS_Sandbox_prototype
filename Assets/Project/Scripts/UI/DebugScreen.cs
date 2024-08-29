using System.Collections;
using TMPro;
using UnityEngine;

public class DebugScreen : MonoBehaviour {
	public UIChannel uiChannel;

	private TextMeshProUGUI text;

	private int frameRate;
	private float timer;

	private ToolType _selectedTool = ToolType.None;

	private void Awake() {

		text = GetComponent<TextMeshProUGUI>();
		timer = 0;
		frameRate = 0;
	}

	private IEnumerator Start() {
		while (!World.current.isInitialized) {
			yield return null;
		}
		transform.parent.gameObject.SetActive(false);
		uiChannel = World.current.getPrimaryPlayer().inputSystem.uiChannel;

		uiChannel.onDebugScreenToggle += toggleScreen;
		uiChannel.onToolTypeChanged += changeTool;
	}

	private void OnDestroy() {
		uiChannel.onDebugScreenToggle -= toggleScreen;
		uiChannel.onToolTypeChanged -= changeTool;
	}

	private void toggleScreen() {
		transform.parent.gameObject.SetActive(!transform.parent.gameObject.activeSelf);
	}

	private void changeTool(ToolType toolType) {
		_selectedTool = toolType;
	}

	private void Update() {

		if (timer > 0.25f) {
			frameRate = (int)(1 / Time.deltaTime);
			timer = 0;
		}

		timer += Time.deltaTime;

		string newText = "Game name\n";

		newText += $"FPS: {frameRate}\n";
		newText += "=======\n";
		newText += $"Tool: {_selectedTool.ToString()}";
		text.text = newText;

	}
}
