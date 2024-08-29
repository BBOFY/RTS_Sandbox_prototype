using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public class ProductionAI {

	public enum ProductionAIState {
		Idle, CitizensProd, ArmyProd, ResupplyProd
	}

	private static readonly EntityType[] soldierTypes =
	{
		EntityType.RangedInfantry,
		EntityType.LightInfantry,
		EntityType.LightInfantry,
		EntityType.HeavyInfantry

	};

	private readonly Player _owner;
	private readonly List<EntityCmp> _productionBuildings = new ();
	private readonly List<GameObject> _soldiers;

	private ProductionAIState _state;
	private Coroutine _productionAICoroutine;

	private readonly UnitProductionOrdersChannel _productionOrdersChannel;
	private readonly ProductionQueueViewChannel _productionQueueViewChannel;

	private readonly ProductionAIChannel _productionAIChannel;

	private EntityType _selectedTypeForProduction = EntityType.Citizen;

	private bool _notEnoughResources;

	private bool _productionHalted;

	public ProductionAI(ComputerInputSystem mainAI, Player owner) {
		_productionAIChannel = mainAI.productionChannel;

		_owner = owner;
		_soldiers = owner._soldiers;
		_productionOrdersChannel = owner.inputSystem.unitProductionOrdersChannel;
		_productionQueueViewChannel = owner.productionQueueViewChannel;

		_productionQueueViewChannel.onNotEnoughResources += setNotEnoughResources;
		_productionAIChannel.onProductionHalted += haltProduction;
	}

	~ProductionAI() {
		_productionQueueViewChannel.onNotEnoughResources -= setNotEnoughResources;
		_productionAIChannel.onProductionHalted -= haltProduction;
	}

	private void setNotEnoughResources() {
		_notEnoughResources = true;
	}

	private void haltProduction(bool toggle) {
		_productionHalted = toggle;
	}

	public void start() {
		if (_productionAICoroutine is null) {
			foreach (var building in _owner._buildings) {
				var productionBuilding = building.GetComponentInChildren<EntityCmp>();

				if (productionBuilding != null) {
					_productionBuildings.Add(productionBuilding);
				}
			}

			_productionAICoroutine = CoroutineStarter.current.startCoroutine(productionAIProcess());
		}
	}

	private IEnumerator productionAIProcess() {
		// Wait 60 seconds before starting producing units
		yield return new WaitForSeconds(60f);

		while (true) {

			if (_productionHalted) {
				yield return null;
			}

			// Wait 20 second if there are not enough resources for unit to produce
			if (_notEnoughResources) {
				_notEnoughResources = false;
				yield return new WaitForSeconds(20f);
			}

			// yield return new WaitForSeconds(2f);

			switch (_state) {

				case ProductionAIState.Idle:
					// produce army units only when population capacity has free space
					if (_soldiers.Count < _owner.inventory.popCap) {
						_state = ProductionAIState.ArmyProd;
					}

					break;

				case ProductionAIState.CitizensProd:
					// currently unused
					// produce citizen units until amount reached
					_state = ProductionAIState.Idle;
					break;

				case ProductionAIState.ArmyProd:

					var randomSoldierType = soldierTypes[Random.Range(0, soldierTypes.Length)];
					yield return produceOrder(1, randomSoldierType);

					// produce army units in random until amount
					_state = ProductionAIState.Idle;
					break;

				case ProductionAIState.ResupplyProd:

					// produce resupply units until amount reached
					// currently unused
					_state = ProductionAIState.Idle;
					break;

			}

		}
	}

	/**
	 * Coroutine responsible for ordering to produce desired amount of desired type of units.
	 */
	private IEnumerator produceOrder(int n, EntityType type) {

		for (var i = 0; i < n; ++i) {
			_productionBuildings[0]?.select(_owner);
			setEntityToProduce(type);
			_owner.inputSystem.unitProductionOrdersChannel.addOrderToProduction();
			_productionBuildings[0]?.deselect(_owner);
			yield return null;
		}

	}

	/**
	 * Sets current order for production building to desired entity type.
	 */
	private void setEntityToProduce(EntityType type) {

		if (type is EntityType.Nothing or EntityType.BaseBuilding) {
			throw new ArgumentException($"This type \"{type}\" cannot be produced");
		}

		while (_selectedTypeForProduction != type) {

			if (_selectedTypeForProduction == EntityType.Resupply) {
				_selectedTypeForProduction = EntityType.Citizen;
			}
			else {
				++_selectedTypeForProduction;
			}
			_productionOrdersChannel.switchToNextOrder();

		}

	}

    
}