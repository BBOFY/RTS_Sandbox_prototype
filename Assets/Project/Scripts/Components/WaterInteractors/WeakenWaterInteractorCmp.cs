using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class WeakenWaterInteractorCmp : BaseWaterInteractorCmp {

	private ArmorCmp _armorCmp;

	[SerializeField]
	private float weakenModifier = 50f;

	private float invertedWeakenModifier;

	protected override void Awake() {
		_armorCmp = GetComponent<ArmorCmp>();
		invertedWeakenModifier = 1 / 50f;
	}

	protected override void onWaterEnter() {
		_armorCmp.modifyProtection(weakenModifier);
	}

	protected override void onWaterExit() {
		_armorCmp.modifyProtection(invertedWeakenModifier);
	}
}
