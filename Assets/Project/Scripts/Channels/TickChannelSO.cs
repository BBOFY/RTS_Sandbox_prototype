using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "TickChannel", menuName = "Scriptable objects/Channels/TickChannel")]
public class TickChannelSO : ScriptableObject {

	public UnityAction onEveryTick;
	public void everyTick() {
		onEveryTick?.Invoke();
	}

	public UnityAction onEveryTickChunkUpdate;
	public void everyTickChunkUpdate() {
		onEveryTickChunkUpdate?.Invoke();
	}
}
