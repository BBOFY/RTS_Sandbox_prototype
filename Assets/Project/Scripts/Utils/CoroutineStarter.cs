using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// This class allows other non-MonoBehaviour classes to use coroutines.
/// </summary>
public class CoroutineStarter : MonoBehaviour {
	public static CoroutineStarter current;

	private void Awake() {
		current = this;
	}

	public Coroutine startCoroutine(IEnumerator process) {
		return StartCoroutine(process);
	}

	public void stopCoroutine(Coroutine coroutine) {
		StopCoroutine(coroutine);
	}
}
