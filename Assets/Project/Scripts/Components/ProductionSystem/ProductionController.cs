
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class ProductionController : MonoBehaviour, ISubscribable {

	private UnitProductionOrdersChannel _unitProductionOrdersChannel;
	private ProductionQueueViewChannel _viewChannel;

	private EntityCmp _entity;
	private PlayerInventory _playerInventory;

	[SerializeField]
	private List<UnitProductionOrderSO> _productionOrders;

	private UnitProductionOrderSO _currentlyProducedOrder;
	private ProductionQueue _productionQueue;

	private bool _productionCoroutineRunning;
	private bool _unitIsBeingProduced;

	private Coroutine _produceUnitCoroutine;

	private int _currentlyChosenOrderIndex;

	private void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
	}

	private void Start() {
		_playerInventory = _entity.owner.inventory;
		_unitProductionOrdersChannel = _entity.owner.inputSystem.unitProductionOrdersChannel;
		_productionQueue = new ProductionQueue();
		_viewChannel = _entity.owner.productionQueueViewChannel;
	}

	private void OnDisable() {
		StopAllCoroutines();
	}

	public void addToProduction(UnitProductionOrderSO unitProductionOrder) {

		// check for resources
		if (!_playerInventory.removeExact(unitProductionOrder)) {
			// inform about not enough resources here
			_viewChannel.notEnoughResources();
			informAboutChange();
			return;
		}

		// if enough resources, add to production
		_productionQueue.push(unitProductionOrder);

		if (!_productionCoroutineRunning) {
			StartCoroutine(productionProcess());
		}
		informAboutChange();

	}

	private void removeFromProduction() {

		if (_produceUnitCoroutine == null || _currentlyProducedOrder == null) {
			return;
		}

		// return cost to players inventory
		_playerInventory.add(_currentlyProducedOrder.cost());

		_viewChannel.updateProductionTimer(0, 0);
		StopCoroutine(_produceUnitCoroutine);
		_unitIsBeingProduced = false;
		informAboutChange();

	}

	/// <summary>
	/// Coroutine responsible for timing the production of a single unit
	/// </summary>
	/// <returns></returns>
	private IEnumerator produceUnit() {

		_unitIsBeingProduced = true;
		float waitTime = _currentlyProducedOrder.productionTime / 10;

		for (int i = 0; i < 10; ++i) {

			// here send to UI information about the progress bar
			_viewChannel.updateProductionTimer(i, 10);
			yield return new WaitForSeconds(waitTime);

		}
		_viewChannel.updateProductionTimer(0, 0);

		// here check, if the population is valid for created unit
		while (_playerInventory.population + _currentlyProducedOrder.populationCost > _playerInventory.popCap) {
			yield return new WaitForSeconds(waitTime);
		}

		// spawn produced unit near production building at random pos and at valid position.
		var spawnAngle = Random.Range(0, 360);
		var spawnPos = Quaternion.Euler(0, spawnAngle, 0) * new Vector3(1, 0, 1);


		if (Physics.Raycast(spawnPos, Vector3.down, out var raycastHit, 3f, 1 << 8)) {
			spawnPos += new Vector3(0, raycastHit.point.y, 0);
		}

		if (NavMesh.SamplePosition(spawnPos, out var hit, 2f, 1 << 0)) {
			spawnPos = hit.position;
		}

		var gObj = Instantiate(_currentlyProducedOrder.prefabToProduce, transform.position + spawnPos, Quaternion.identity);
		var entityController = gObj.GetComponent<EntityCmp>();
		entityController.owner = _entity.owner;
		_unitIsBeingProduced = false;
	}

	private IEnumerator productionProcess() {

		_productionCoroutineRunning = true;

		_currentlyProducedOrder = _productionQueue.pop();

		// while there are any orders for unit production
		while (_currentlyProducedOrder != default) {

			informAboutChange();

			// wait for current order to complete
			_produceUnitCoroutine = StartCoroutine(produceUnit());

			while (_unitIsBeingProduced) {
				yield return null;
			}

			// add another order for production
			_currentlyProducedOrder = _productionQueue.pop();
			yield return null;
		}
		_viewChannel.updateProductionTimer(0, 0);
		informAboutChange();
		_productionCoroutineRunning = false;
	}

	private void addOrderToQueue() {
		addToProduction(_productionOrders[_currentlyChosenOrderIndex]);
	}

	private void iterateOrders() {
		++_currentlyChosenOrderIndex;
		if (_currentlyChosenOrderIndex >= _productionOrders.Count) {
			_currentlyChosenOrderIndex = 0;
		}

		// Inform about the changed value
		_viewChannel.updateToProduceView(_productionOrders[_currentlyChosenOrderIndex]);
	}


	public void subscribe() {
		_unitProductionOrdersChannel.onProductionStopRequest += removeFromProduction;
		_unitProductionOrdersChannel.onOrderSwitch += iterateOrders;
		_unitProductionOrdersChannel.onAddOrderToProduction += addOrderToQueue;

		_viewChannel.toggleElement(true);
		informAboutChange();
		_viewChannel.updateToProduceView(_productionOrders[_currentlyChosenOrderIndex]);
	}

	public void unsubscribe() {
		_unitProductionOrdersChannel.onProductionStopRequest -= removeFromProduction;
		_unitProductionOrdersChannel.onOrderSwitch -= iterateOrders;
		_unitProductionOrdersChannel.onAddOrderToProduction -= addOrderToQueue;

		_viewChannel.toggleElement(false);
	}

	private void informAboutChange() {
		_viewChannel.updateProductionQueueView(_currentlyProducedOrder, _productionQueue);
	}
}
