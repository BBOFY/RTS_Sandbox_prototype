using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageData {

	public DamageType _type;
	public int _amount;

	public DamageData(DamageType type, int amount) {
		_type = type;
		_amount = amount;
	}

}
