using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component representing the area of alert around an entity
/// </summary>
public abstract class LineOfSightCmp : MonoBehaviour, IBeingSupplied {

	protected EntityCmp _entity;

	[SerializeField]
	protected float _losRange;

	[SerializeField]
	[Tooltip("Percentage of LOS after provision penalization")]
	protected float _losPenalty = 1f;

	protected void Awake() {
		_entity = GetComponentInParent<EntityCmp>();

		if (_losPenalty < 0.25f) {
			_losPenalty = 0.25f;
		}
	}

	public abstract IDamageable findTarget();

	public void penalize() {
		_losRange *= _losPenalty;
	}

	public void restore() {
		_losRange /= _losPenalty;
	}
}
