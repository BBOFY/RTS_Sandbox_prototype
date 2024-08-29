using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class ProductionQueue {

	private readonly List<UnitProductionOrderSO> _queue = new ();

	public void push(UnitProductionOrderSO unitProductionOrder) {
		_queue.Add(unitProductionOrder);
	}

	public UnitProductionOrderSO top() {
		try {
			var toReturn = _queue[0];
			return toReturn;
		}
		catch (ArgumentOutOfRangeException) {
			return default;
		}
	}

	public UnitProductionOrderSO pop() {
		try {
			var toReturn = _queue[0];
			_queue.RemoveAt(0);
			return toReturn;
		}
		catch (ArgumentOutOfRangeException) {
			return default;
		}
	}

	public bool isEmpty() {
		return _queue.Count == 0;
	}

	public int count() {
		return _queue.Count;
	}

}
