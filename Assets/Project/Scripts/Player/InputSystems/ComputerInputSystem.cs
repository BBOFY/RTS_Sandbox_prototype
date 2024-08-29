using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;


/// <summary>
/// Acts as a brain of the computer player AI
/// </summary>
public class ComputerInputSystem : InputSystem {

	public readonly ProductionAIChannel productionChannel;
	public readonly ArmyAIChannel armyChannel;

	public Vector3 defendingRallyPoint => _defendingRallyPoint;
	public List<Vector3> attackingRallyPoints => _attackingRallyPoints;


	private ProductionAI _productionAI;
	private ArmyAI _armyAI;

	// Rally point near the player's base building, between it and the center of the world
	private Vector3 _defendingRallyPoint;

	// Rally points near the enemy's base building, between them and the center of the world
	private List<Vector3> _attackingRallyPoints;

	public ComputerInputSystem(Player owner) : base(owner) {
		productionChannel = new ProductionAIChannel();
		armyChannel = new ArmyAIChannel();
	}

	private List<Vector3> getAttackingPoints() {
		var enemiesPositions = new List<Vector3>();

		foreach (var player in World.current.players) {
			if (ReferenceEquals(player, _owner)) {
				continue;
			}

			enemiesPositions.Add(player.rallyPoint);
		}

		return enemiesPositions;
	}

	public override void init() {
		_productionAI = new ProductionAI(this, _owner);
		_armyAI = new ArmyAI(this, _owner);

	}

	public override void start() {
		_defendingRallyPoint = _owner.rallyPoint;
		_attackingRallyPoints = getAttackingPoints();

		_productionAI.start();
		_armyAI.start();
	}

	/// <summary>
	/// In the current state, this function is for debugging purposes only.
	/// This function effects nothing in normal build.
	/// </summary>
	protected override IEnumerator startToManageInputs() {

		while (true) {

			if (Input.GetKeyDown(KeyCode.Y)) {
				Debug.Log(_owner.inventory.ToString());
			}

			yield return null;

		}
	}
}
