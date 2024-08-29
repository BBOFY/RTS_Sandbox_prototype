using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class UIPresenter : MonoBehaviour {

	protected TextMeshProUGUI text;

	private void Awake() {
		text = GetComponent<TextMeshProUGUI>();
	}

	private IEnumerator Start() {

		while (!World.current.isInitialized) {
			yield return null;
		}

		init();

	}

	protected abstract void init();

}
