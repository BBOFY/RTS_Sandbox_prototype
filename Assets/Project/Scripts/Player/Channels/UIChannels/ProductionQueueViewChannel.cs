using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProductionQueueViewChannel {

	public UnityAction<bool> onComponentSelected;
	public void toggleElement(bool toggle) {
		onComponentSelected?.Invoke(toggle);
	}

	public UnityAction<UnitProductionOrderSO, ProductionQueue> onContentsChanged;
	public void updateProductionQueueView(UnitProductionOrderSO currentlyProducing, ProductionQueue queue) {
		onContentsChanged?.Invoke(currentlyProducing, queue);
	}

	public UnityAction<UnitProductionOrderSO> onOrderChanged;
	public void updateToProduceView(UnitProductionOrderSO currentlyProducing) {
		onOrderChanged?.Invoke(currentlyProducing);
	}

	public UnityAction<int, int> onProductionTimeCahnged;
	public void updateProductionTimer(int curr, int finish) {
		onProductionTimeCahnged?.Invoke(curr, finish);
	}

	public UnityAction onNotEnoughResources;
	public void notEnoughResources() {
    	onNotEnoughResources?.Invoke();
    }

}
