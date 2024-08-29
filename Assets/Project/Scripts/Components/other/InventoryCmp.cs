using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryCmp : MonoBehaviour, ISubscribable {

	protected bool _isSelected;

	[SerializeField]
	protected int _capacity = 50;
	[SerializeField]
	protected int _currentlyCarrying;

	protected ResourceType _resourceType;

	protected InventoryViewChannel _viewChannel;

	public int capacity {
		get => _capacity;
	}

	public int currentlyCarrying {
		get => _currentlyCarrying;
	}

	public ResourceType resourceType {
		get => _resourceType;
	}

	public bool isFull() {
		return _currentlyCarrying >= _capacity;
	}

	public bool isEmpty() {
		return _currentlyCarrying <= 0;
	}

	protected void Awake() {
		if (_currentlyCarrying > _capacity) {
			_currentlyCarrying = _capacity;
		}
	}

	protected void Start() {
		_viewChannel = GetComponentInParent<EntityCmp>().owner.inventoryViewChannel;
	}

	public ResourceData addToInv(int amount) {
		if (amount < 0) throw new ArgumentException("Amount cannot be a negative value");

		var toCarry = _currentlyCarrying;
		toCarry += amount;

		var remainder = toCarry - capacity;

		if (remainder <= 0) {
			_currentlyCarrying = toCarry;
		}
		else {
			_currentlyCarrying = toCarry - remainder;
		}

		informAboutChange();
		return new ResourceData(resourceType, remainder);
	}

	// Takes desired amount or the rest of the inventory (whichever is less) of resource from the inventory
	public ResourceData takeFromInv(int desiredAmount) {

		if (desiredAmount < 0) throw new ArgumentException("Amount cannot be a negative value");


		if (desiredAmount >= _currentlyCarrying) {
			_currentlyCarrying = 0;
			informAboutChange();
			return new ResourceData(resourceType, _currentlyCarrying);
		}

		_currentlyCarrying -= desiredAmount;
		informAboutChange();
		return new ResourceData(resourceType, desiredAmount);
	}

	/**
	 * Tries to add exact amount to inventory. If there is enough space, returns true and add amount to inv.
	 * Otherwise returns false and nothing happens.
	 */
	public bool addExactToInv(int amount) {
		if (amount < 0) throw new ArgumentException("Amount cannot be negative value");

		if (capacity < _currentlyCarrying + amount) {
			informAboutChange();
			return false;
		}

		_currentlyCarrying += amount;
		informAboutChange();
		return true;
	}

	/**
	 * Tries to remove exact amount from inventory. If there is enough amount in inv., returns true and remove amount from inv.
	 * Otherwise returns false and nothing happens.
	 */
	public bool takeExactFromInv(int amount) {
		if (amount < 0) throw new ArgumentException("Amount cannot be negative value");

		if (currentlyCarrying < amount) {
			informAboutChange();
			return false;
		}

		_currentlyCarrying -= amount;
		informAboutChange();
		return true;
	}

	public void informAboutChange() {
		if (_capacity > 0 && _isSelected) {
			_viewChannel.updateInventoryView(new ResourceData(resourceType, _currentlyCarrying), _capacity);
		}
	}

	public void subscribe() {
		_isSelected = true;
		_viewChannel.toggleElement(true, _resourceType);
		informAboutChange();
	}

	public void unsubscribe() {
		_viewChannel.toggleElement(false, _resourceType);
		_isSelected = false;
	}
}
