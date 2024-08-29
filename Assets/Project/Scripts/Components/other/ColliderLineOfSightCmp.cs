using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class ColliderLineOfSightCmp : LineOfSightCmp {

	private readonly List<IDamageable> _targets = new ();

	private SphereCollider _sphereCollider;

	protected new void Awake() {
		base.Awake();
		_sphereCollider = GetComponent<SphereCollider>();
		_sphereCollider.radius = _losRange/2f;
		_sphereCollider.isTrigger = true;
		_sphereCollider.enabled = true;
	}

	private void OnTriggerEnter(Collider other) {

		// Checks if entity in sight is enemy entity and if it is, adds it to the list
		if (other.TryGetComponent<IDamageable>(out var damageable)
		    && damageable.getOwner() != null // entity belongs to some player
		    && !ReferenceEquals(_entity.owner, damageable.getOwner())
		   ) {
			_targets.Add(damageable);
		}
	}

	private void OnTriggerExit(Collider other) {
		// Checks if entity that got out of sight is enemy entity and if it is, removes it from the list
		if (other.TryGetComponent<IDamageable>(out var damageable)
		    && damageable.getOwner() != null // entity belongs to some player
		    && !ReferenceEquals(_entity.owner, damageable.getOwner())
		   ) {
			_targets.Remove(damageable);
		}
	}

	// Gets next target from the list of detected enemies.
	public override IDamageable findTarget() {

		while (_targets.Count != 0) {
			if (_targets[0] == null || _targets[0].isDestroyed()) {
				_targets.RemoveAt(0);
				continue;
			}
			return _targets[0];
		}

		return default;

	}
}
