using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppingOffCmp : MonoBehaviour, ISubscribable {

	private enum DroppingOffStates {
		Idle, MovingToStorage, DroppingOff
	}

	private UnitOrdersChannel _unitOrdersChannel;

	private EntityCmp _entity;

	private Vector3 _lastDropOffPointPos;

	private NavigationCmp _navigationCmp;
	private GatheringCmp _gatheringCmp;
	private GatheringInventoryCmp _gatheringInventory;

	private IDropOffPoint _dropOffPoint;
	private IMovable _moveCmp;

	private DroppingOffStates _state;

	public void goToStorage(IDropOffPoint dropOffPoint = default) {

		// drop off point stopped to exist
		if (dropOffPoint != default) {
			_dropOffPoint = dropOffPoint;
		}

		// try to find another
		else if (_dropOffPoint == default || _dropOffPoint.isDestroyed()) {
			_dropOffPoint = _navigationCmp.findDropOffNodePos(_lastDropOffPointPos);
		}

		// another was not found
		if (_dropOffPoint == default || _dropOffPoint.isDestroyed()) {
			changeStateTo(DroppingOffStates.Idle);
			_entity.state = EntityState.Idle;
		}
		// another was found or original drop off point exists
		else {
			_moveCmp.move(_dropOffPoint.getWorldPosition(), _dropOffPoint.getRadius());
			changeStateTo(DroppingOffStates.MovingToStorage);
			_entity.state = EntityState.DroppingOff;
			StartCoroutine(droppingOffProcess());
		}

	}

	private void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
		_moveCmp = GetComponentInParent<MoveCmp>();
		_navigationCmp = GetComponent<NavigationCmp>();
		_gatheringCmp = GetComponent<GatheringCmp>();
		_gatheringInventory = GetComponent<GatheringInventoryCmp>();
		_lastDropOffPointPos = transform.position;
	}

	private void Start() {
		_unitOrdersChannel = _entity.owner.inputSystem.unitOrdersChannel;
	}

	public void subscribe() {
		_unitOrdersChannel.onStorageAssign += goToStorage;
	}

	public void unsubscribe() {
		_unitOrdersChannel.onStorageAssign -= goToStorage;
	}

	private void changeStateTo(DroppingOffStates state) {
		_state = state;
	}

	private IEnumerator droppingOffProcess() {
		while (true) {
			if (_entity.state != EntityState.DroppingOff) {
				changeStateTo(DroppingOffStates.Idle);
				yield break;
			}

			switch (_state) {
				case DroppingOffStates.Idle:
					break;

				case DroppingOffStates.MovingToStorage:

					// If the drop off point is destroyed when the entity is moving towards it...
					if (_dropOffPoint == default || _dropOffPoint.isDestroyed()) {
						// ...try to find another one in the defined proximity.
						goToStorage();

						// If none such exists,...
						if (_dropOffPoint == default) {
							// ...stop the entity entirely
							changeStateTo(DroppingOffStates.Idle);
							_entity.state = EntityState.Idle;
							break;
						}
					}

					if (isNextToDropOffPoint()) {
						_moveCmp.stop();
						_entity.state = EntityState.DroppingOff;
						changeStateTo(DroppingOffStates.DroppingOff);
					}

					break;

				case DroppingOffStates.DroppingOff:
					if (_dropOffPoint == default || _dropOffPoint.isDestroyed()) {
						changeStateTo(DroppingOffStates.MovingToStorage);
						break;
					}

					_dropOffPoint.store(_gatheringInventory.getInventoryContent());
					_gatheringCmp.goToResourceNode();
					break;
			}

			yield return null;
		}

	}

	private bool isNextToDropOffPoint() {
		return Vector2.Distance(
			new Vector2(_dropOffPoint.getWorldPosition().x, _dropOffPoint.getWorldPosition().z),
			new Vector2(transform.position.x, transform.position.z)
		) <= _dropOffPoint.getRadius();
	}

}
