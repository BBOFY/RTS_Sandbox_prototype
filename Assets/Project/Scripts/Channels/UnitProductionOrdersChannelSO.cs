using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "UnitProductionOrdersChannel", menuName = "Scriptable objects/Channels/UnitProductionOrdersChannel")]
public class UnitProductionOrdersChannelSO : ScriptableObject {

	public UnityAction onProductionStopRequest;
	public void stopProduction() {
		onProductionStopRequest?.Invoke();
	}

	public UnityAction onAddOrderToProduction;
	public void addOrderToProduction() {
		onAddOrderToProduction?.Invoke();
	}

	public UnityAction onOrderSwitch;
	public void switchToNextOrder() {
		onOrderSwitch?.Invoke();
	}

	public UnityAction onCitizenProduceRequest;
	public void produceCitizen() {
		onCitizenProduceRequest?.Invoke();
	}

	public UnityAction onSupplyTruckProduceRequest;
	public void produceSupplyTruck() {
		onSupplyTruckProduceRequest?.Invoke();
	}

	public UnityAction onPikeManProduceRequest;
	public void producePikeMan() {
		onPikeManProduceRequest?.Invoke();
	}

	public UnityAction onMusketeerProduceRequest;
	public void produceMusketeer() {
		onMusketeerProduceRequest?.Invoke();
	}

	public UnityAction onKnightProduceRequest;
	public void produceKnightupplyTruck() {
		onKnightProduceRequest?.Invoke();
	}

}
