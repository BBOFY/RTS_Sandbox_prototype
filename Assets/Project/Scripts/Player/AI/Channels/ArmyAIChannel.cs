using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArmyAIChannel {

	public UnityAction onDefendRequest;
	public void defend() {
		onDefendRequest?.Invoke();
	}

	public UnityAction<Vector3> onAttackRequest;
	public void attackAt(Vector3 targetRallyPoint) {
		onAttackRequest?.Invoke(targetRallyPoint);
	}

	public UnityAction<Vector3> onRallyRequest;
	public void rallyAt(Vector3 rallyPoint) {
		onRallyRequest?.Invoke(rallyPoint);
	}

}
