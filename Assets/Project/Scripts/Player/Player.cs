using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

[Serializable]
public class Player {

	public readonly string playerName;
	private PlayerInventory _inventory;
	public PlayerInventory inventory => _inventory;

	public InputSystem inputSystem;
	public readonly PlayerColor playerColor;

	private Vector3 _rallyPoint;
	public Vector3 rallyPoint => _rallyPoint;

	public readonly HealthViewChannel healthViewChannel;
	public readonly InventoryViewChannel inventoryViewChannel;
	public readonly ResourceViewChannel resourceViewChannel;
	public readonly PlayerInventoryViewChannel playerInventoryViewChannel;
	public readonly ProductionQueueViewChannel productionQueueViewChannel;

	public readonly ResupplierViewChannel resupplierViewChannel;
	public readonly ResupplyeeViewChannel resupplyeeViewChannel;

	public readonly List<GameObject> _buildings = new ();
	public readonly List<GameObject> _citizens = new ();
	public readonly List<GameObject> _soldiers = new ();
	public readonly List<GameObject> _resuppliers = new ();
	public readonly List<GameObject> _otherEntities = new ();

	public Player(PlayerColor playerColor = default, string playerName = "Player") {

		this.playerColor = playerColor;
		this.playerName = playerName;

		healthViewChannel = new HealthViewChannel();
		inventoryViewChannel = new InventoryViewChannel();
		resourceViewChannel = new ResourceViewChannel();
		playerInventoryViewChannel = new PlayerInventoryViewChannel();
		productionQueueViewChannel = new ProductionQueueViewChannel();
		resupplierViewChannel = new ResupplierViewChannel();
		resupplyeeViewChannel = new ResupplyeeViewChannel();

	}

	private Vector3 calculateRallyPoint() {
		var baseBuildingPos = _buildings[0].transform.position;
		var directionToCenterVec = (World.current.centerOfWorld - baseBuildingPos).normalized;
		return baseBuildingPos + directionToCenterVec * 2f;
	}

	public virtual void initPlayerSystems(Dictionary<ResourceType, int> inv = null) {
		inputSystem.init();
		_inventory = new PlayerInventory(inputSystem.uiChannel, playerInventoryViewChannel, inv);
	}

	public void startPlayerSystems() {
		inputSystem.start();
	}

	public void loadBuildings(List<GameObject> allBuildings) {
		_rallyPoint = calculateRallyPoint();
	}

	// Find the closest owned drop off point from given position.
	public IDropOffPoint getClosestDropOffPoint(Vector3 absPos) {

		IDropOffPoint closestDropOff = null;

		var minDistance = float.PositiveInfinity;

		for (var i = _buildings.Count - 1; i >= 0; --i) {

			if (_buildings[i] is null) {
				_buildings.RemoveAt(i);
				continue;
			}

			var currDropOff = _buildings[i].GetComponent<IDropOffPoint>();

			if (currDropOff == null || currDropOff.isDestroyed()) {
				_buildings.RemoveAt(i);
				continue;
			}

			var currResourcePos = new Vector2(currDropOff.getWorldPosition().x, currDropOff.getWorldPosition().z);
			var currDistance = Vector2.Distance(new Vector2(absPos.x, absPos.z), currResourcePos);

			if (currDistance < minDistance) {
				minDistance = currDistance;
				closestDropOff = currDropOff;
			}

		}

		return closestDropOff;
	}

	public void initViews() {
		inventory.showChange();
	}

	// Add given entity to player's "inventory" of entities
	public void assignToPlayer(GameObject selectableEntity, EntityType type) {

		_inventory.changePopulationBy(selectableEntity.GetComponent<EntityCmp>().populationCost);

		switch (type) {
			case EntityType.Citizen:
				_citizens.Add(selectableEntity);
				break;
			case EntityType.LightInfantry or EntityType.HeavyInfantry or EntityType.RangedInfantry:
				_soldiers.Add(selectableEntity);
				break;
			case EntityType.Resupply:
				_resuppliers.Add(selectableEntity);
				break;
			case EntityType.BaseBuilding:
				_buildings.Add(selectableEntity);
				break;
			default:
				_otherEntities.Add(selectableEntity);
				break;
		}
	}

	// Remove given entity from player's "inventory" of entities
	public void unassignFromPlayer(GameObject selectableEntity, EntityType type) {
		_inventory.changePopulationBy(- selectableEntity.GetComponent<EntityCmp>().populationCost);

		switch (type) {
			case EntityType.Citizen:
				_citizens.Remove(selectableEntity);
				break;
			case EntityType.LightInfantry or EntityType.HeavyInfantry or EntityType.RangedInfantry:
				_soldiers.Remove(selectableEntity);
				break;
			case EntityType.Resupply:
				_resuppliers.Remove(selectableEntity);
				break;
			case EntityType.BaseBuilding:
				_buildings.Remove(selectableEntity);
				break;
			default:
				_otherEntities.Remove(selectableEntity);
				break;
		}
	}

}
