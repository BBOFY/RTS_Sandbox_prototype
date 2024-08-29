using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProductionAIChannel {

	public UnityAction<int> onCitizenProductionRequest;
	public void produceCitizen(int amount) {
		onCitizenProductionRequest?.Invoke(amount);
	}

	public UnityAction<int> onSoldierProductionRequest;
	public void produceSoldier(int amount) {
		onSoldierProductionRequest?.Invoke(amount);
	}

	public UnityAction<int> onResupplierProductionRequest;
	public void produceResupplier(int amount) {
		onResupplierProductionRequest?.Invoke(amount);
	}

	public UnityAction<bool> onProductionHalted;
	public void haltProduction(bool toggle) {
		onProductionHalted?.Invoke(toggle);
	}

}
