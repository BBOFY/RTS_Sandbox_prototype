using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class ResupplyeeCmp : MonoBehaviour, ISubscribable {

	private AmmunitionInventoryCmp _ammunitionInventory;

	private readonly Dictionary<ResourceType, InventoryCmp> _inventories = new ();

	private ResupplierCmp _resupplierCmp;
	private EntityCmp _entity;

	private UnitOrdersChannel _unitOrdersChannel;

	[SerializeField]
	private bool _isActive;

	public bool isActive => _isActive;

	private void Awake() {
		_resupplierCmp = GetComponentInChildren<ResupplierCmp>();
		_entity = GetComponent<EntityCmp>();
	}

	private void Start() {
		List<InventoryCmp> inventoriesToProcess = new ();
		inventoriesToProcess.AddRange(_entity.GetComponentsInChildren<AmmunitionInventoryCmp>());
		inventoriesToProcess.AddRange(_entity.GetComponentsInChildren<ProvisionsInventoryCmp>());
		foreach (var inv in inventoriesToProcess) {
			_inventories.Add(inv.resourceType, inv);
		}
		_unitOrdersChannel = GetComponentInChildren<EntityCmp>().owner.inputSystem.unitOrdersChannel;
	}

	/// <summary>
	///	Add inserted amount to munition storage
	/// </summary>
	/// <param name="amountToResupply">amount of munition to resupply</param>
	/// <returns>
	/// -1 if there is still space for munition to resupply,
	/// otherwise returns the remainder of munition, that was over the munition storage capacity.
	/// If 0 is returned, the munition storage is exactly full.
	/// </returns>
	public ResourceData resupply(ResourceData resourceToResupply) {

		if (!_inventories.ContainsKey(resourceToResupply.type)
		    || _inventories[resourceToResupply.type].isFull()) {
			return resourceToResupply;
		}

		return _inventories[resourceToResupply.type].addToInv(resourceToResupply.amount);

	}

	// Toggle this component.
	public void toggle() {
		_isActive = !_isActive;
		// If toggle is true, deactivate the resupplier component if present.
		if (_isActive && _resupplierCmp && _resupplierCmp.isActive) {
			_resupplierCmp.toggle();
		}
		_entity.owner.resupplyeeViewChannel.toggleResupplyeeView(_isActive);
	}

	public void subscribe() {
		_unitOrdersChannel.onToggleResupplyee += toggle;
		_entity.owner.resupplyeeViewChannel.toggleElement(true);
		_entity.owner.resupplyeeViewChannel.toggleResupplyeeView(_isActive);
	}

	public void unsubscribe() {
		_unitOrdersChannel.onToggleResupplyee -= toggle;
		_entity.owner.resupplyeeViewChannel.toggleElement(false);
	}
}
