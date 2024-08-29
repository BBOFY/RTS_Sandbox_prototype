using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;

public class ArmyAI {

	public enum AttackingAIState {
		Idle, Attacking, Defending, WaitingReinforcement
	}

	private readonly ComputerInputSystem _brain;
	private readonly Player _owner;

	private readonly List<GameObject> _army = new ();
	private readonly List<GameObject> _soldiers;

	private readonly UnitOrdersChannel _unitOrdersChannel;
	private readonly HealthViewChannel _healthViewChannel;

	private readonly ArmyAIChannel _armyAIChannel;

	private readonly ProductionAIChannel _productionAIChannel;

	private AttackingAIState _state;
	private Coroutine _attackingAICoroutine;

	public ArmyAI(ComputerInputSystem mainAI, Player owner) {
		_brain = mainAI;
		_armyAIChannel = mainAI.armyChannel;

		_owner = owner;
		_soldiers = owner._soldiers;

		_unitOrdersChannel = owner.inputSystem.unitOrdersChannel;
		_healthViewChannel = owner.healthViewChannel;
		_productionAIChannel = mainAI.productionChannel;

		_armyAIChannel.onAttackRequest += startAttack;
		_armyAIChannel.onDefendRequest += startDefending;
		_healthViewChannel.onIsUnderAttack += startDefending;
		_healthViewChannel.onNotUnderAttack += idle;

	}

	~ArmyAI() {
		_armyAIChannel.onAttackRequest -= startAttack;
		_armyAIChannel.onDefendRequest -= startDefending;
		_healthViewChannel.onIsUnderAttack -= startDefending;
		_healthViewChannel.onNotUnderAttack -= idle;
	}

	private void idle() {
		_state = AttackingAIState.WaitingReinforcement;
	}

	// Sends units in army near the enemy's base building
	private void startAttack(Vector3 target) {
		_state = AttackingAIState.Attacking;
		moveArmy(target);
	}

	// Sends units in army near own base building
	private void startDefending() {
		_state = AttackingAIState.Defending;
		moveArmy(_brain.defendingRallyPoint);
	}

	private void reinforce() {
		startDefending();
		_state = AttackingAIState.WaitingReinforcement;
	}

	public void start() {
		if (_attackingAICoroutine is null) {
			_state = AttackingAIState.WaitingReinforcement;
			_attackingAICoroutine = CoroutineStarter.current.startCoroutine(attackingAIProcess());
		}
	}

	private IEnumerator attackingAIProcess() {

		int waveStrength = 1;
		float percentageToSave = 0.125f;

		while (true) {

			yield return null;

			// Periodically check, if soldiers in army are still alive and remove references to dead ones
			for (int i = _army.Count - 1; i >= 0; --i) {
				if (_army[i] is null || !_army[i].TryGetComponent<IDamageable>(out var soldier) || soldier.isDestroyed()) {
					_army.RemoveAt(i);
				}
			}

			switch (_state) {

				case AttackingAIState.Idle:

					// wait for reserves and army to fill
					break;


				// Sends army to attack other players base building
				case AttackingAIState.Attacking:

					if (_army.Count < waveStrength * percentageToSave) {
						_productionAIChannel.haltProduction(false);
						reinforce();
					}

					break;


				// send army back and wait for resources
				case AttackingAIState.WaitingReinforcement:
					if (_soldiers.Count > waveStrength) {

						Debug.Log("Adding soldiers to army");
						Debug.Log($"Wave strength = {waveStrength}");
						// Fill army with produced soldier. Army will have a "waveStrength" amount of soldier in it
						for (int i = _soldiers.Count - 1; i >= 0; --i) {
							_army.Add(_soldiers[i]);
							_soldiers.RemoveAt(i);
							if (_army.Count >= waveStrength) {
								break;
							}
						}

						// Wait after forming and army and then attack
						yield return new WaitForSeconds(90f);
						startAttack(_brain.attackingRallyPoints[0]);

						if (waveStrength >= _owner.inventory.popCap) {
							Debug.Log("popcap size");
							waveStrength = _owner.inventory.popCap;
						}
						else {
							Debug.Log("plus rand size");
							waveStrength += Random.Range(0, 3);
						}
					}

					break;

				case AttackingAIState.Defending:
					// here just wait for the event, that invokes idle() method
					break;
			}

		}
	}

	/**
	 * Moves army to desired position
	 */
	private void moveArmy(Vector3 pos) {

		var distortedPos = pos + new Vector3(Random.Range(-0.5f, 0.5f), pos.y, Random.Range(-0.5f, 0.5f));

		foreach (var soldier in _army) {
			soldier.GetComponent<EntityCmp>().select(_owner);
		}
		_unitOrdersChannel.requestMove(distortedPos);
		foreach (var soldier in _army) {
			soldier.GetComponent<EntityCmp>().deselect(_owner);
		}
	}

}
