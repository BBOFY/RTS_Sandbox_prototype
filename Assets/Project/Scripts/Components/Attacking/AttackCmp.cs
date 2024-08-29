using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackCmp : MonoBehaviour, ISubscribable, IBeingSupplied {

	private enum AttackState {
		Idle, MovingToTarget, Targeting, Attacking, Reloading
	}

	private UnitOrdersChannel _unitOrdersChannel;

	[SerializeField]
	private WeaponCmp _weaponCmp;

	private MoveCmp _moveCmp;
	private LineOfSightCmp _lineOfSightCmp;
	private EntityCmp _entity;
	private AmmunitionInventoryCmp _ammunitionInventory;

	private IDamageable _target;
	private Vector3 _oldTargetPos;

	private float _range;
	private float _attackAfterMoveRange;
	private float _reloadTime;

	[SerializeField]
	[Tooltip("How much longer the reload will be")]
	private float _reloadTimePenalty = 1f;

	[SerializeField]
	[Tooltip("How much weaker the damage will be")]
	private float _damagePenalty = 1f;

	private AttackState _state;

	private Coroutine _onAlert;
	private Coroutine _attackingProcess;

	private void Awake() {
		_entity = gameObject.GetComponentInParent<EntityCmp>();
		_lineOfSightCmp = transform.parent.GetComponentInChildren<LineOfSightCmp>();
		_moveCmp = gameObject.GetComponentInParent<MoveCmp>();
		_ammunitionInventory = gameObject.GetComponent<AmmunitionInventoryCmp>();

		if (_weaponCmp.ammoCost > 0 && _ammunitionInventory == default) {
			throw new NullReferenceException("Ammunition inventory not found");
		}

		if (_reloadTimePenalty < 0.125f) {
			_reloadTimePenalty = 0.125f;
		}

		// Has weapon that need physical strength use
		if ((_ammunitionInventory == default || _weaponCmp.ammoCost <= 0)
		    && _damagePenalty < 0.25f) {
			_damagePenalty = 0.25f;
		}
	}

	private void Start() {
		_range = _weaponCmp.range;
		_attackAfterMoveRange = _range * 0.75f;
		_reloadTime = _weaponCmp.reloadTime;

		/*
		 * This would not work with environment-controlled entities
		 */
		if (_entity.owner != default) {
			_unitOrdersChannel = _entity.owner.inputSystem.unitOrdersChannel;
		}

		_onAlert = StartCoroutine(onAlertProcess());
	}

	public void subscribe() {
		_unitOrdersChannel.onAttackRequested += initiateAttack;
	}

	public void unsubscribe() {
		_unitOrdersChannel.onAttackRequested -= initiateAttack;
	}

	public void initiateAttack(IDamageable target = default) {

		// Target is set manually by the player
		if (target != default) {
			_target = target;
		}
		// Target got destroyed
		else if (_target == null || _target.isDestroyed()) {
			_target = _lineOfSightCmp.findTarget();
		}

		// New target not found
		if (_target == null || _target.isDestroyed()) {
			changeStateTo(AttackState.Idle);
			_entity.state = EntityState.Idle;
			StopAllCoroutines();
			_onAlert = StartCoroutine(onAlertProcess());
		}

		// Continue with the attack
		else {
			_oldTargetPos = _target.getPosition();
			_moveCmp?.move(_oldTargetPos, _attackAfterMoveRange);
			_entity.state = EntityState.Attacking;

			changeStateTo(AttackState.MovingToTarget);
			if (_attackingProcess is not null) {
				StopCoroutine(_attackingProcess);
			}
			_attackingProcess = StartCoroutine(attackingProcess());
			if (_moveCmp is not null) {
				StartCoroutine(doRotationAtTargetDirection());
			}
		}
	}

	private void changeStateTo(AttackState state) {
		_state = state;

	}


	private IEnumerator onAlertProcess() {
		while (true) {
			// Attacking unit is idle, its weapon doesnt require any ammo,
			// and there is enemy in its LOS
			if (_entity.state == EntityState.Idle
			    && (_weaponCmp.ammoCost == 0 || !_ammunitionInventory.isEmpty())
			    && _lineOfSightCmp.findTarget() != default) {

				initiateAttack();
				yield break;
			}

			yield return new WaitForSeconds(_weaponCmp.targetingTime);
		}
	}

	private IEnumerator attackingProcess() {

		StopCoroutine(_onAlert);

		while (true) {

			if (_entity.state != EntityState.Attacking) {
				changeStateTo(AttackState.Idle);
				_onAlert = StartCoroutine(onAlertProcess());
				yield break;
			}

			if (_target == null || _target.isDestroyed()) {
				initiateAttack();
				yield break;
			}

			switch (_state) {
				case AttackState.Idle:
					_onAlert = StartCoroutine(onAlertProcess());
					yield break;

				case AttackState.MovingToTarget:

					// when target is in range, start to look at it
					if (targetInRange(_attackAfterMoveRange)) {
						_moveCmp?.stop();
						_entity.state = EntityState.Attacking;
						changeStateTo(AttackState.Targeting);
					}

					// Target position has dramatically changed
					if (_moveCmp is not null && Mathf.Abs(Vector3.Distance(_oldTargetPos, _target.getPosition())) > 0.5f) {
						_oldTargetPos = _target.getPosition();
						_moveCmp.move(_oldTargetPos, _attackAfterMoveRange);
						_entity.state = EntityState.Attacking;
					}

					break;

				case AttackState.Targeting:

					if (!targetInRange(_range)) {
						changeStateTo(AttackState.MovingToTarget);
						break;
					}

					yield return new WaitForSeconds(_weaponCmp.targetingTime);
					// if target is still in range, attack it
					changeStateTo(AttackState.Attacking);
					break;

				case AttackState.Attacking:
					// call attack on component responsible for damaging the target
					// Check, if weapon has enough munition and use it, if it does
					// In case of not enough ammunition
					if (_weaponCmp.ammoCost == 0 || _ammunitionInventory.takeExactFromInv(_weaponCmp.ammoCost)) {
						_weaponCmp.use(_target.getPosition());
						// Debug.Log("pew");
						changeStateTo(AttackState.Reloading);
						break;
					}

					Debug.Log("Not enough ammunition");
					changeStateTo(AttackState.Idle);
					yield break;

				case AttackState.Reloading:
					yield return new WaitForSeconds(_reloadTime);

					// when target is in range, start to look at it
					if (targetInRange(_range)) {
						changeStateTo(AttackState.Targeting);
						break;
					}

					changeStateTo(AttackState.MovingToTarget);
					break;
			}

			yield return null;
		}

	}

	private bool targetInRange(float range) {
		if (_target == null || _target.isDestroyed()) return false;

		return Vector2.Distance(
			new Vector2(_target.getPosition().x, _target.getPosition().z),
			new Vector2(transform.position.x, transform.position.z)
		) <= range;
	}

	/**
	 * From https://answers.unity.com/questions/1663326/smoothly-rotate-object-towards-target-object.html
	 * When attacking, keep looking at the target
	 */
	private IEnumerator doRotationAtTargetDirection() {

		Quaternion targetRotation = Quaternion.identity;
		do {
			if (_target == null || _target.isDestroyed() || _entity.state != EntityState.Attacking || _state == AttackState.Idle) yield break;

			Vector3 targetDirection = (_target.getPosition() - transform.position).normalized;
			targetRotation = Quaternion.LookRotation(targetDirection);
			targetRotation.x = 0;
			targetRotation.z = 0;
			_entity.transform.rotation = targetRotation;
			// _entity.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);

			yield return null;

		} while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f);
	}


	public void penalize() {
		_reloadTime *= _reloadTimePenalty;
	}

	public void restore() {
		_reloadTime /= _reloadTimePenalty;
	}
}
