using System;
using System.Collections;
using UnityEngine;

public class GatheringCmp : MonoBehaviour, ISubscribable, IBeingSupplied {

	private enum GathererStates {
		Idle, MovingToResource, Gathering
	}

	private EntityCmp _entity;

	private UnitOrdersChannel _unitOrdersChannel;

	private NavigationCmp _navigationCmp;
	private DroppingOffCmp _droppingOffCmp;
	private GatheringInventoryCmp _gatheringInventory;

	private IMovable _moveCmp;
	private IResource _resourceNode;

	private Vector3 _lastResourcePos;

	private GathererStates _state;

	private AudioSource _audioSource;

	[SerializeField]
	private float _timeToGatherInSeconds = 2f;

	[SerializeField]
	private float _gatheringTimePenalty;

	private int _currentTicks;

	private void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
		_moveCmp = GetComponentInParent<IMovable>();
		_navigationCmp = GetComponent<NavigationCmp>();
		_droppingOffCmp = GetComponent<DroppingOffCmp>();
		_gatheringInventory = GetComponent<GatheringInventoryCmp>();
		_audioSource = GetComponent<AudioSource>();
		_lastResourcePos = transform.position;

		if (_gatheringTimePenalty < 0.125f) {
			_gatheringTimePenalty = 0.125f;
		}
	}

	private void Start() {
		_unitOrdersChannel = _entity.owner.inputSystem.unitOrdersChannel;
	}

	public void goToResourceNode(IResource resourceNode = default, ResourceType resourceType = default) {

		// Resource node set by player
		if (resourceNode != default) {
			_resourceNode = resourceNode;
		}
		else if (_gatheringInventory.resourceType == ResourceType.Nothing) {
			changeStateTo(GathererStates.Idle);
			_entity.state = EntityState.Idle;
			return;
		}
		// Resource node is set by other components or already set by previous call
		else if (_resourceNode == default || _resourceNode.isDestroyed()) {
			ResourceType resourceTypeToFind;
			resourceTypeToFind = resourceType == default ? _gatheringInventory.resourceType : resourceType;

			_resourceNode = _navigationCmp.findResourceNodePos(_lastResourcePos, resourceTypeToFind);
		}

		// Navigation component did not found the closest resource node
		// OR The path to resource node does not exists
		if (_resourceNode == default || _resourceNode.isDestroyed()) {
			changeStateTo(GathererStates.Idle);
			_entity.state = EntityState.Idle;
		}
		// Resource node was found or set by player
		else {
			_lastResourcePos = _resourceNode.getWorldPosition();
			_gatheringInventory.informAboutChange();
			_moveCmp.move(_resourceNode.getWorldPosition(), _resourceNode.getRadius());
			changeStateTo(GathererStates.MovingToResource);
			_entity.state = EntityState.Gathering;
			StartCoroutine(gatheringProcess());
		}

	}

	public void subscribe() {
		_unitOrdersChannel.onResourceAssign += goToResourceNode;
	}

	public void unsubscribe() {
		_unitOrdersChannel.onResourceAssign -= goToResourceNode;
		// _tickChannel.onEveryTick -= debugFoo;
	}

	private IEnumerator gatheringProcess() {
		while (true) {
			// Entity got other orders and ceased process of gathering
			if (_entity.state != EntityState.Gathering) {
				changeStateTo(GathererStates.Idle);
				_gatheringInventory.resetType();
				yield break;
			}

			switch (_state) {

				case GathererStates.Idle:
					yield break;

				case GathererStates.MovingToResource:

					// If node stopped existing while entity tries to reach it
					if (_resourceNode == default || _resourceNode.isDestroyed()) {
						// Try to find other node
						goToResourceNode();

						// If there is no node in the proximity of the unit.
						// unit tries to drop off resources gathered
						if (_resourceNode == default) {
							changeStateTo(GathererStates.Idle);
							_droppingOffCmp.goToStorage();
							break;
						}
					}

					// If entity is close to resource node
					if (isNextToResource()) {
						_moveCmp.stop();
						_entity.state = EntityState.Gathering;
						changeStateTo(GathererStates.Gathering);
					}

					break;

				case GathererStates.Gathering:

					// If capacity is reached, go to the nearest storage
					if (_gatheringInventory.isFull()) {
						changeStateTo(GathererStates.Idle);
						_droppingOffCmp.goToStorage();
						break;
					}

					// If resource node stops existing, in another update loop new resource node is found
					if (_resourceNode == default || _resourceNode.isDestroyed()) {
						changeStateTo(GathererStates.MovingToResource);
						break;
					}

					_gatheringInventory.addToInventory(_resourceNode.getResource());
					_audioSource.Play();
					yield return new WaitForSeconds(_timeToGatherInSeconds);
					break;
			}

			yield return null;
		}

	}

	private bool isNextToResource() {

		if (_resourceNode == default || _resourceNode.isDestroyed()) return false;

		return Vector2.Distance(
			new Vector2(_resourceNode.getWorldPosition().x, _resourceNode.getWorldPosition().z),
			new Vector2(transform.position.x, transform.position.z)
			) <= _resourceNode.getRadius();
	}

	private void changeStateTo(GathererStates state) {
		_state = state;
	}

	public void penalize() {
		_timeToGatherInSeconds *= _gatheringTimePenalty;
	}

	public void restore() {
		_timeToGatherInSeconds /= _gatheringTimePenalty;
	}
}
