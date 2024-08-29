using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Water : Block, ILiquid, IUpdatableBlock {

	private readonly float _timeToSpill = 0.5f;

	protected static readonly int maxLevel = 4;
	protected int level;
	protected bool hasBlockOverIt;


	protected bool _newlyCreated;

	protected float blockHeightStep;

	protected World _world;

	protected Coroutine _spillageCoroutine;

	public float getBlockHeight() {
		if (hasBlockOverIt)
			return 1f;
		if (level == maxLevel)
			return 0.9375f;
		return blockHeightStep * level;

	}

	protected bool _isSubscribed;

	public Water(Vector3Int absPosition, int levelToSet)
		: base(absPosition, BlockId.WATER) {

		_newlyCreated = true;

		blockHeightStep = 1f / maxLevel;

		_world = World.current;

		if (levelToSet <= 0) {
			_world.destroyBlock(_absPosition);
			return;
		}

		level = levelToSet > maxLevel ? maxLevel : levelToSet;

		subscribeToTicks();

	}

	public void update() {
		if (!_isSubscribed) {
			_isSubscribed = true;
			subscribeToTicks();
		}
	}

	protected IEnumerator executeUpdateLogicProcess() {

		yield return new WaitForSeconds(_timeToSpill);

		// if this is the first time of calling this method in block's life, do spill
		if (_newlyCreated) {
			_newlyCreated = false;
			doSpillage();
			yield break;
		}

		// if level was changed, the world is informed about this change
		if (stateChanged()) {
			// This statement is used for the creation of the new water block with the new level
			if (this is WaterLoggedStructure) {
				if (level <= 0) {
					_world.placeBlock(_absPosition, new Structure());
				}
				else {
					_world.placeBlock(_absPosition, new WaterLoggedStructure(_absPosition, level));
				}

			}
			else {
				if (level <= 0) {
					_world.destroyBlock(_absPosition);
				}
				else {
					_world.placeBlock(_absPosition, new Water(_absPosition, level));
				}
			}

			_world.updateNeighbours(_absPosition);

		}

		doSpillage();

		unsubscribeToTicks();
	}


	protected virtual void executeUpdateLogic() {
		CoroutineStarter.current.startCoroutine(executeUpdateLogicProcess());
	}
	protected void subscribeToTicks() {
		executeUpdateLogic();

	}
	protected void unsubscribeToTicks() {
		_isSubscribed = false;
	}

	/// <summary>
	/// Changes level of this block to 1 less, than the level of neighbour water block with the largest level.
	/// </summary>
	/// <returns>
	/// True if level was changed,
	///	otherwise false.
	/// </returns>
	private bool stateChanged() {

		int maxNeighbourLevel = 0;

		foreach (Face face in Enum.GetValues(typeof(Face))) {

			if (face == Face.Bottom) continue;

			var checkedBlockPos = _absPosition + ChunkData.getFaceNormal(face);

			var checkedBlock = _world.getBlockAt(checkedBlockPos);

			// This is also checking, if checkedBlock is not null
			if (checkedBlock is Water waterBlock) {

				if (face == Face.Top) {
					maxNeighbourLevel = maxLevel + 1;
					hasBlockOverIt = true;
					break;
				}

				if (waterBlock.level > maxNeighbourLevel) {
					maxNeighbourLevel = waterBlock.level;
				}

			}

			hasBlockOverIt = false;

		} /* foreach through faces */

		if (maxNeighbourLevel != level + 1) {
			level = maxNeighbourLevel - 1;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Checks bottom block and tries to spread to that place
	/// </summary>
	/// <returns>
	/// True, if the water has spread to bottom block, otherwise false
	/// </returns>
	protected bool isFalling() {

		var bottomBlockPos = _absPosition + ChunkData.getFaceNormal(Face.Bottom);

		var bottomBlock = World.current.getBlockAt(bottomBlockPos);

		if (bottomBlock?.blockSO is null || bottomBlock.blockSO.isSolid) {
			return false;
		}

		// bottom block is water source, so no need to update it
		if (bottomBlock is WaterSource) {
			return true;
		}

		// set bottom water block to max level
		if (bottomBlock is Water waterBottomBlock) {
			if (waterBottomBlock.level < maxLevel) {
				waterBottomBlock.update();
			}

			return true;
		}

		// At this point, bottom block must be air or structure, so create water block accordingly
		_world.placeBlock(bottomBlockPos,
			bottomBlock is Structure
				? new WaterLoggedStructure(bottomBlockPos, maxLevel)
				: new Water(bottomBlockPos, maxLevel)
		);

		return true;
	}

	/// <summary>
	/// Starts spillage process if it has not already begun
	/// </summary>
	protected void doSpillage() {
		if (_spillageCoroutine is not null) {
			CoroutineStarter.current.stopCoroutine(_spillageCoroutine);
		}

		_spillageCoroutine = CoroutineStarter.current.startCoroutine(doSpillageProcess());
	}

	protected IEnumerator doSpillageProcess() {

		if (isFalling()) {
			yield break;
		}

		// water lost its range
		if (level <= 1) {
			yield break;
		}

		foreach (var face in new []{Face.Back, Face.Left, Face.Front, Face.Right}) {

			yield return null;

			var checkedBlockPos = _absPosition + ChunkData.getFaceNormal(face);
			var checkedBlock = _world.getBlockAt(checkedBlockPos);

			if (checkedBlock?.blockSO is null || checkedBlock.blockSO.isSolid || checkedBlock is WaterSource) {
				continue;
			}

			// neighbour is water block
			if (checkedBlock is Water waterCheckedBlock) {
				if (waterCheckedBlock.level < level - 1) {
					waterCheckedBlock.update();
				}
				continue;
			}

			// Here, the neighbour must be air or structure, so create new water block accordingly.
			_world.placeBlock(checkedBlockPos,
				checkedBlock is Structure
					? new WaterLoggedStructure(checkedBlockPos, level - 1)
					: new Water(checkedBlockPos, level - 1)
			);

		} /* foreach loop through faces */

	}


}
