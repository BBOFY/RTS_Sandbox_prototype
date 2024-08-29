using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResupplierCmp : MonoBehaviour, ISubscribable {

	private readonly ResourceType[] _resourceTypes = {ResourceType.Provision, ResourceType.Ammunition};

	private List<ResupplyeeCmp> _entitiesToResupply;

	private EntityCmp _entity;

	private readonly List<InventoryCmp> _inventories = new ();
	private PlayerInventory _playerInventory;

	private ResupplyeeCmp _resupplyeeCmp;

	private UnitOrdersChannel _unitOrdersChannel;

	[SerializeField]
	private float _resupplySpeed = 0.5f;

	[SerializeField]
	private int _resupplyAmount = 1;

	[SerializeField]
	private bool _isActive = true;

	public bool isActive => _isActive;

	private void Awake() {
		_entitiesToResupply = new List<ResupplyeeCmp>();
		_resupplyeeCmp = GetComponentInParent<ResupplyeeCmp>();
		_entity = GetComponentInParent<EntityCmp>();

		_inventories.AddRange(transform.parent.GetComponentsInChildren<InventoryCmp>());

		if (_isActive && _resupplyeeCmp && _resupplyeeCmp.isActive) {
			_resupplyeeCmp.toggle();
		}
	}

	private void Start() {
		_unitOrdersChannel = _entity.owner.inputSystem.unitOrdersChannel;

		if (_inventories.Count <= 0) {
			_playerInventory = _entity.owner.inventory;
		}

		StartCoroutine(resupplyProcess());
	}

	private IEnumerator resupplyProcess() {

		while (true) {

			while (!isActive || _entitiesToResupply.Count <= 0) {
				yield return null;
			}

			for (var i = _entitiesToResupply.Count - 1; i >= 0; --i) {

				if (_inventories.Count != 0) {
					resupplyFromEntityInventory(i);
				}
				else {
					resupplyFromPlayerInventory(i);
				}
			}

			yield return null;

			// resupplying speed
			yield return new WaitForSeconds(_resupplySpeed);

		}
	}

	// Resupply entities within resupply area with every resource that resupplier entity has inventory for.
	private void resupplyFromEntityInventory(int entityToResupplyIdx) {

		foreach (var inventory in _inventories) {
			if (inventory.isEmpty()) {
				continue;
			}

			if (!_entitiesToResupply[entityToResupplyIdx].isActive) {
				continue;
			}

			var resupplyingResource = inventory.takeFromInv(_resupplyAmount);

			var resupplyResidue =
				_entitiesToResupply[entityToResupplyIdx]
					.resupply(resupplyingResource)
					.amount;

			if (resupplyResidue < 0) {
				continue;
			}

			if (resupplyResidue > 0) {
				inventory.addToInv(resupplyResidue);
			}
		}
	}

	// Resupply entities within resupply area with every resource from player inventory.
	// (Currently used only by base building and this should be placed somewhere else, in separate component)
	private void resupplyFromPlayerInventory(int i) {

		foreach (var resourceType in _resourceTypes) {
			if (_playerInventory.isEmpty(resourceType)) {
				continue;
			}

			if (!_entitiesToResupply[i].isActive) {
				continue;
			}

			var resupplyingResource = _playerInventory.removeExact(new ResourceData(resourceType, _resupplyAmount));

			int resupplyResidue = int.MinValue;

			if (resupplyingResource) {
				resupplyResidue =
					_entitiesToResupply[i]
						.resupply(new ResourceData(resourceType, _resupplyAmount))
						.amount;
			}

			if (resupplyResidue < 0) {
				continue;
			}

			if (resupplyResidue > 0) {
				_playerInventory.add(new ResourceData(resourceType, resupplyResidue));
			}
		}
	}

	// Add friendly entity in proximity, that can be resupplied.
	private void OnTriggerEnter(Collider other) {

		// Add entities, that belong to the same player as resupplier and need to be resupplied
		if (!other.TryGetComponent<ResupplyeeCmp>(out var newResupplyee)
		    || !ReferenceEquals(_entity.owner, other.gameObject.GetComponentInParent<EntityCmp>().owner)
		    ) {
			return;
		}
		_entitiesToResupply.Add(newResupplyee);

	}

	// Remove friendly resupplyable entity, that got out of resupply area.
	private void OnTriggerExit(Collider other) {
		if (!other.TryGetComponent<ResupplyeeCmp>(out var newResupplyee)) {
			return;
		}
		_entitiesToResupply.Remove(newResupplyee);
	}

	// Toggle this component.
	public void toggle() {
		_isActive = !_isActive;

		// If toggle is true, deactivate the resupplyee component if present.
		if (_isActive && _resupplyeeCmp && _resupplyeeCmp.isActive) {
			_resupplyeeCmp.toggle();
		}
		_entity.owner.resupplierViewChannel.toggleResupplierView(_isActive);
	}

	public void subscribe() {
		_unitOrdersChannel.onToggleResupplier += toggle;
		_entity.owner.resupplierViewChannel.toggleElement(true);
		_entity.owner.resupplierViewChannel.toggleResupplierView(_isActive);
	}

	public void unsubscribe() {
		_unitOrdersChannel.onToggleResupplier -= toggle;
		_entity.owner.resupplierViewChannel.toggleElement(false);
	}
}
