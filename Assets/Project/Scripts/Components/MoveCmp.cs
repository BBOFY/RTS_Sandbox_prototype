using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class MoveCmp : MonoBehaviour, IMovable, ISubscribable, IBeingSupplied {

	private enum MovementState {
		Idle, Moving
	}

	[SerializeField]
	private float _clearance = 0.25f;

	// private int _ticksPassed;
	private Vector3 oldPos;

	private Vector3 _destination;

	private Coroutine _movingCoroutine;

	private EntityCmp _entity;
	private NavMeshAgent _agent;
	private MovementState _state;

	private ProvisionsInventoryCmp _provisionsInventory;

	[SerializeField]
	private float _entitySpeed = 4;
	[SerializeField]
	private float _entityProvisionsSpeedPenalty = 1;
	[SerializeField]
	private float _entityWaterSpeedPenalty = 1;

	public float entityProvisionsSpeedPenalty => _entityProvisionsSpeedPenalty;
	public float entityWaterSpeedPenalty => _entityWaterSpeedPenalty;

	private UnitOrdersChannel _unitOrdersChannel;

	private void Awake() {
		_agent = GetComponent<NavMeshAgent>();
		_entity = GetComponentInParent<EntityCmp>();
		_provisionsInventory = GetComponent<ProvisionsInventoryCmp>();

		_agent.speed = _entitySpeed;
		_agent.enabled = false;

		if (_entityProvisionsSpeedPenalty < 0.125f) {
			_entityProvisionsSpeedPenalty = 0.125f;
		}
		if (_entityWaterSpeedPenalty < 0.125f) {
			_entityWaterSpeedPenalty = 0.125f;
		}
	}

	private void Start() {
		_unitOrdersChannel = _entity.owner.inputSystem.unitOrdersChannel;
	}

	// Send entity to given destination
	public void move(Vector3 destination, float stoppingDistance = 0) {

		if (destination != _agent.destination /*|| _state != MovementState.Moving*/) {

			_agent.enabled = true;

			// If destination is outside accessible area, try to find closest accessible point from given destination
			if (!NavMesh.SamplePosition(destination, out var hit, 10f, 1 << 0)) {
				_agent.ResetPath();
				return;
			}

			_destination = hit.position;
			_state = MovementState.Moving;
			_entity.state = EntityState.Moving;

			_movingCoroutine = StartCoroutine(movementProcess());

			_agent.stoppingDistance = stoppingDistance;
			_agent.ResetPath();
			_agent.SetDestination(hit.position);
		}
		else {
			stop();
		}
	}

	// Stops the entity
	public void stop() {
		if (_agent.enabled) {
			_agent.ResetPath();
		}
		_state = MovementState.Idle;
		_entity.state = EntityState.Idle;
		_agent.enabled = false;
		if (_movingCoroutine is not null) {
			StopCoroutine(_movingCoroutine);
		}
	}

	public void subscribe() {
		_unitOrdersChannel.onMoveRequested += move;
		_unitOrdersChannel.onStopRequested += stop;
		// _tickChannel.onEveryTick += checkMovement;
	}

	public void unsubscribe() {
		_unitOrdersChannel.onMoveRequested -= move;
		_unitOrdersChannel.onStopRequested -= stop;
	}

	private IEnumerator movementProcess() {

		while (true) {

			yield return null;

			if (_entity.state is EntityState.Dying) {
				yield break;
			}

			if (_state is not MovementState.Moving) {
				yield break;
			}

			oldPos = transform.position;
			var oldPos2D = new Vector2(oldPos.x, oldPos.z);

			yield return new WaitForSeconds(1f);

			if (_entity.state is EntityState.Dying) {
				yield break;
			}

			var currentPos2D = new Vector2(transform.position.x, transform.position.z);
			var distance = Vector2.Distance(currentPos2D, oldPos2D);

			// checks, if entity is moving on the spot
			if (distance <= _clearance) {

				// Check distance to destination and...
				distance = Vector2.Distance(currentPos2D, new Vector2(_destination.x, _destination.z));

				if (!_agent.enabled) {
					continue;
				}

				// ... if it is near it, stop the entity...
				if (distance <= _clearance) {
					stop();
				}
				// ... otherwise try to move the entity again in attempt to unstuck it.
				else {
					_agent.ResetPath();
					_agent.SetDestination(_destination);
				}

			}

		}
	}


	public void penalize() {
		_agent.speed *= _entityProvisionsSpeedPenalty;
	}

	public void restore() {
		_agent.speed /= _entityProvisionsSpeedPenalty;
	}
}
