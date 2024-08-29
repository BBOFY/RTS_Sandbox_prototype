using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitOrdersChannel {

	// for moving
	public UnityAction<Vector3, float> onMoveRequested;
	public void requestMove(Vector3 dest, float stoppingDistance = 0f) {
		onMoveRequested?.Invoke(dest, stoppingDistance);
	}

	public UnityAction onStopRequested;
	public void requestStop() {
		onStopRequested?.Invoke();
	}

	// for gathering
	public UnityAction<IDropOffPoint> onStorageAssign;
	public void assignStorage(IDropOffPoint dropOffPoint) {
		onStorageAssign?.Invoke(dropOffPoint);
	}

	public UnityAction<IResource, ResourceType> onResourceAssign;
	public void assignResource(IResource resourceNode, ResourceType resourceType) {
		onResourceAssign?.Invoke(resourceNode, resourceType);
	}

	// for attacking
	public UnityAction<IDamageable> onAttackRequested;
	public void requestAttack(IDamageable target = default) {
		onAttackRequested?.Invoke(target);
	}

	public UnityAction onToggleResupplier;
	public void toggleResupplier() {
		onToggleResupplier?.Invoke();
	}

	public UnityAction onToggleResupplyee;
	public void toggleResupplyee() {
		onToggleResupplyee?.Invoke();
	}


	public UnityAction onRallyPointPlaced;
	public void placeRallyPoint() {
		onToggleResupplyee?.Invoke();
	}

}
