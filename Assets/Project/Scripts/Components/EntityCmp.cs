using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[SelectionBase]
public class EntityCmp : MonoBehaviour, ISelectable {
	private EntityState _state;

	[SerializeField]
	private int _populationCost;
	public int populationCost => _populationCost;

	[SerializeField]
	private string _entityName;

	[SerializeField]
	private EntityType _entityType;
	public EntityType entityType => _entityType;

	private readonly List<Player> _playersWhoSelected = new ();

	public EntityState state {
		get {return _state;}
		set {
			_state = value;
		}}

	[SerializeField]
	private Renderer _colorRenderer;

	private PlayerColor _playerColor;

	public PlayerColor color => _playerColor;

	private bool _isSelected;


	[SerializeField]
	private GameObject _selectionRing;

	// Components, that player can interact with
	private List<ISubscribable> _activeComponents;
	// Components, that can show information to player
	private List<IShowable> _showableComponents;

	private Player _owner;
	public Player owner {get => _owner;
		set {
			_owner = value;
			_playerColor = _owner.playerColor;
			if (_colorRenderer is not null) {
				setColor();
			}

			if (_populationCost < 0) {
				_populationCost = 0;
			}
			_owner?.assignToPlayer(gameObject, _entityType);
		}
	}

	private void Awake() {
		if (_selectionRing) {
			_selectionRing.SetActive(false);
		}
	}

	private void Start() {
		_activeComponents = gameObject.GetInterfacesInChildren<ISubscribable>().ToList();
		_showableComponents = gameObject.GetInterfacesInChildren<IShowable>().ToList();
	}

	public void select(Player player) {

		if (state == EntityState.Dying) {
			return;
		}

		if (_playersWhoSelected.Contains(player)) {
			return;
		}

		_playersWhoSelected.Add(player);

		player.inputSystem.uiChannel.toggleElement(true);
		player.inputSystem.uiChannel.showName(_entityName);

		foreach (var showable in _showableComponents) {
			showable.show(player);
		}

		if (!ReferenceEquals(player, _owner)) {
			return;
		}

		// below will be called only if player is owner of the entity with this component

		if (_selectionRing) {
			// if selection ring is attached to entity
			_selectionRing.SetActive(true);
		}
		foreach (var subscribable in _activeComponents) {
			subscribable.subscribe();
		}
	}

	public void deselect(Player player) {

		player.inputSystem.uiChannel.toggleElement(false);

		if (!_playersWhoSelected.Remove(player)) {
			return;
		}

		foreach (var showable in _showableComponents) {
			showable.hide(player);
		}

		if (!ReferenceEquals(player, _owner)) {
			return;
		}

		// below will be called only if player is owner of the entity with this component

		if (_selectionRing) {
			// if selection ring is attached to entity
			_selectionRing.SetActive(false);
		}

		foreach (var subscribable in _activeComponents) {
			subscribable.unsubscribe();
		}
	}

	public PlayerColor getOwnerColor() {
		return _playerColor;
	}

	public void die() {
		state = EntityState.Dying;

		for (var i = _playersWhoSelected.Count - 1; i >= 0; --i) {
			deselect(_playersWhoSelected[i]);
		}

		_owner.unassignFromPlayer(gameObject, _entityType);
		StartCoroutine(dyingProcess());

	}

	// orders entity to die
	private IEnumerator dyingProcess() {

		NavMeshAgent agent;

		if (TryGetComponent<NavMeshAgent>(out agent)) {
			agent.enabled = false;
		}
		yield return StartCoroutine(fadingOut());

		Destroy(gameObject);
	}

	// control the dying "animation"
	private IEnumerator fadingOut() {

		for (int i = 0; i < 10; ++i) {
			transform.position += 0.1f * Vector3.down;
			yield return new WaitForSeconds(0.1f);
		}
		transform.position += 100f * Vector3.down;

	}

	private void setColor() {

		Material customMaterial = _colorRenderer.material;
		switch (_playerColor) {
			case PlayerColor.Red:
				customMaterial.color = Color.red;
				// customMaterial.SetColor("_TeamColor", Color.red);
				break;
			case PlayerColor.Blue:
				customMaterial.color = Color.blue;
				// customMaterial.SetColor("_TeamColor", Color.blue);
				break;
			case PlayerColor.Green:
				customMaterial.color = Color.green;
				// customMaterial.SetColor("_TeamColor", Color.green);
				break;
			case PlayerColor.Yellow:
				customMaterial.color = Color.yellow;
				// customMaterial.SetColor("_TeamColor", Color.yellow);
				break;
			default:
				// customMaterial.SetColor("_TeamColor", Color.gray);
				break;
		}
	}

	public List<Player> getPlayersWhoSelected() {
		return _playersWhoSelected;
	}
}
