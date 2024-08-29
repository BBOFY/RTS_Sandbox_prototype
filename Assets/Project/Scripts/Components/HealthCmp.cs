using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HealthCmp : MonoBehaviour, IDamageable, IShowable {

	private EntityCmp _entity;
	private readonly List<HealthViewChannel> _viewChannels = new ();

	private int _currentHealth;

	private ArmorCmp _armor;

	private bool _isOnStructure = true;

	private Coroutine _isUnderAttackCoroutine;

	[SerializeField]
	private int _maxHealth;

	private void Awake() {
		_currentHealth = _maxHealth;
		_entity = GetComponent<EntityCmp>();
		_armor = GetComponent<ArmorCmp>();


		if (TryGetComponent<MoveCmp>(out _)) {
			_isOnStructure = false;
		}
	}

	// recieves damage via this method
	public void doDamage(DamageData damageData) {

		_currentHealth -= Mathf.Abs(calculateDamage(damageData));

		startUnderAttackTimer();

		if (_currentHealth <= 0) {
			_entity.die();
		}

		foreach (var channel in _viewChannels) {
			channel.updateHealthView(_currentHealth, _maxHealth);
		}
	}

	// Start timer, if unit is under attack. And alerts the player.
	// This is only used by base building and this functionality should be moved elsewhere.
	private void startUnderAttackTimer() {

		if (_entity.entityType != EntityType.BaseBuilding) {
			return;
		}

		// Invoke event for the first time
		if (_isUnderAttackCoroutine == null) {
			_entity.owner.healthViewChannel.isUnderAttack();
		}

		if (_isUnderAttackCoroutine != null) {
			StopCoroutine(_isUnderAttackCoroutine);
		}

		_isUnderAttackCoroutine = StartCoroutine(underAttackTimer());

	}

	private IEnumerator underAttackTimer() {
		yield return new WaitForSeconds(10f);
		_entity.owner.healthViewChannel.notUnderAttack();
	}

	public Vector3 getPosition() {

		if (!_isOnStructure) {
			return transform.position;
		}

		// Adding half vector to center the position and lift it up a bit
		return transform.position + new Vector3(0.5f, 0.5f, 0.5f);
	}

	public bool isDestroyed() {
		return _entity.state == EntityState.Dying;
	}

	public Player getOwner() {
		return _entity.owner;
	}

	public PlayerColor getOwnerColor() {
		return _entity.color;
	}

	private int calculateDamage(DamageData damageData) {
		var damageAmount = _armor == default ? damageData._amount : _armor.getReducedDamage(damageData);
		return damageAmount;

	}

	public void show(Player player) {
		if (_entity.state == EntityState.Dying) {
			return;
		}
		_viewChannels.Add(player.healthViewChannel);
		player.healthViewChannel.toggleElement(true);
		player.healthViewChannel.updateHealthView(_currentHealth, _maxHealth);
	}

	public void hide(Player player) {
		_viewChannels.Remove(player.healthViewChannel);
		player.healthViewChannel.toggleElement(false);
	}
}
