using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Class representing the global inventory of each player
/// </summary>
public class PlayerInventory {

	private readonly UIChannel _uiChannel;

	private int _popCap = 50;
	public int popCap => _popCap;

	private int _population;
	public int population => _population;

	public void changePopulationBy(int amount) {
		_population += amount;
	}

	private readonly Dictionary<ResourceType, int> _inventory;

	public PlayerInventory(UIChannel uiChannel, PlayerInventoryViewChannel viewChannel, Dictionary<ResourceType, int> inventory = null) {
		if (inventory is null) {
			_inventory = new Dictionary<ResourceType, int> {
				{ResourceType.Food, 200},
				{ResourceType.Wood, 100},
				{ResourceType.Gold, 0},
				{ResourceType.Iron, 0},
				{ResourceType.Provision, 300},
				{ResourceType.Ammunition, 150}
			};
		}
		else {
			_inventory = inventory;
		}

		_uiChannel = uiChannel;
		viewChannel.toggleElement(true);
		viewChannel.updateInventoryView(_inventory);
	}

	public void add(ResourceData resourceData) {
		_inventory[resourceData.type] += resourceData.amount;
		showChange();
	}

	public void add(Dictionary<ResourceType, int> resources) {

		// If enough, remove them from the players inventory
		foreach (var resource in resources) {
			_inventory[resource.Key] += resource.Value;
		}

		showChange();
	}

	public bool removeExact(Dictionary<ResourceType, int> resources) {

		// Check, if there are enough resources
		foreach (var resource in resources) {
			if (_inventory[resource.Key] < resource.Value) {
				return false;
			}
		}

		// If enough, remove them from the players inventory
		foreach (var resource in resources) {
			_inventory[resource.Key] -= resource.Value;
		}

		showChange();
		return true;
	}

	/// <summary>
	/// Checks, if there is enough amount of resource of given type in inventory
	/// </summary>
	/// <param name="resourceData">Amount and type of resource to be removed from inv.</param>
	/// <returns>True, desired amount and type of resoruce is present in inventory, otherwise false</returns>
	public bool removeExact(ResourceData resourceData) {

		// Check, if there are enough resources
		if (_inventory[resourceData.type] < resourceData.amount) {
			return false;
		}

		// If enough, remove them from the players inventory
		_inventory[resourceData.type] -= resourceData.amount;

		showChange();
		return true;
	}

	// This method is needed to initialize the AI player inventory, so it can has large amount of resources.
	// This method has to be removed with better AI.
	public bool removeExact(UnitProductionOrderSO unitProductionOrder) {
		if (_population + unitProductionOrder.populationCost > _popCap) {
			return false;
		}
		return removeExact(unitProductionOrder.cost());
	}

	public void showChange() {
		_uiChannel.showPlayersResources(new PlayersResources(
			_population,
			_popCap,
			_inventory[ResourceType.Food],
			_inventory[ResourceType.Wood],
			_inventory[ResourceType.Iron],
			_inventory[ResourceType.Gold],
			_inventory[ResourceType.Provision],
			_inventory[ResourceType.Ammunition])
		);
	}

	public bool isEmpty(ResourceType resourceType) {
		return _inventory[resourceType] <= 0;
	}

	public int showAmount(ResourceType resourceType) {
		return _inventory[resourceType];
	}

	public override string ToString() {
		var text = _inventory.Aggregate("", (current, res) => current + $"{res.Key.ToString()}: {res.Value} | ");

		return text + $"Pop: {_population}/{_popCap}";
	}
}
