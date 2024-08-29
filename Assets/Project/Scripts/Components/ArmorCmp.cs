using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorCmp : MonoBehaviour {

	[Serializable]
	private struct ArmorProtection {
		public ArmorProtection(DamageType damageType, float damageReduction) {
			this.damageType = damageType;
			this.damageReduction = damageReduction;
		}

		public DamageType damageType;
		[Tooltip("Percentage of damage reduction written as float")]
		public float damageReduction;
	}

	[SerializeField]
	private List<ArmorProtection> _protections;

	// returns new value of damage taken
	public int getReducedDamage(DamageData damageData) {

		foreach (var protection in _protections) {

			if (protection.damageType == damageData._type) {
				return Mathf.FloorToInt(damageData._amount * protection.damageReduction);
			}

		}

		return damageData._amount;
	}

	// changes every present armor modification by multiplying them with given multiplier
	public void modifyProtection(float modifier) {

		for (int i = 0; i < _protections.Count; ++i) {
			ArmorProtection newProtection = new (_protections[i].damageType, _protections[i].damageReduction * modifier);
			_protections[i] = newProtection;
		}

	}

}
