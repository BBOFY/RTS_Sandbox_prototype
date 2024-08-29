using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * Hit scan weapon is dependent on having the target in sight. There cannot be any obstacles between attacker and target
 */
public class HitScanWeaponCmp : WeaponCmp {

	[SerializeField]
	private int _damage;

	[SerializeField]
	private DamageType _damageType;

	private LineRenderer _lineRenderer;

	private AudioSource _audioSource;

	private void Awake() {
		_lineRenderer = gameObject.GetComponent<LineRenderer>();
		if (_lineRenderer == default) {
			return;
		}
		_lineRenderer.enabled = false;
		_audioSource = GetComponent<AudioSource>();
	}

	public override void use(Vector3 targetPos) {

		var raycastDirection = targetPos - transform.position;

		if (Physics.Raycast(transform.position, raycastDirection, out var hit, _range, 1 << 12)) {

			if (_lineRenderer != default) {
				_audioSource.Play();
				StartCoroutine(showShot(hit.point));
			}

			if (hit.collider.TryGetComponent<IDamageable>(out var hitObject)) {
				hitObject.doDamage(new DamageData(_damageType, _damage));
			}
		}
	}

	private IEnumerator showShot(Vector3 target) {
		_lineRenderer.enabled = true;
		_lineRenderer.SetPosition(0, transform.position);
		_lineRenderer.SetPosition(1, target);
		yield return new WaitForSeconds(0.001f);
		_lineRenderer.enabled = false;
	}

}
