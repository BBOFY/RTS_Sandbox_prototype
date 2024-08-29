using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponCmp : MonoBehaviour {

	[SerializeField]
	protected float _range;
	[SerializeField]
	protected float _reloadTime = 1f;
	[SerializeField]
	protected float _targetingTime = 1f;

	[SerializeField]
	protected int _ammoCost;

	protected Transform _weaponOrigin;

	/**
	 * Use weapon to attack target on given position
	 */
	public abstract void use(Vector3 target);

	// public virtual void init(Transform weaponOrigin) {
	// 	// Debug.Log(_weaponOrigin.position);
	// 	// _weaponOrigin = weaponOrigin;
	// }

	public float range => _range;
	public float reloadTime => _reloadTime;
	public float targetingTime => _targetingTime;
	public int ammoCost => _ammoCost;
}
