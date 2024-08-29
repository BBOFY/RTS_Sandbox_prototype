using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

/// <summary>
/// Currently this class hold human player controls for giving orders to entities or for manipulating the terrain.
/// More sophisticated system integrating camera controls and allowing customization should be created in the future.
/// </summary>
public sealed class HumanPlayerInputSystem : InputSystem {

	private GameObject _highlightingCubeValid;
	private GameObject _highlightingCubeInvalid;

	private Vector3Int _newPos;

	private RaycastHit _raycastHit;

	private SelectionManager _selectionManager;

	public HumanPlayerInputSystem(Player owner): base(owner) {

		_highlightingCubeValid = Object.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightBlockValid"));
		_highlightingCubeInvalid = Object.Instantiate(Resources.Load<GameObject>("Prefabs/HighlightBlockInvalid"));
		_highlightingCubeValid.SetActive(false);
		_highlightingCubeInvalid.SetActive(false);
	}

	public override void init() {
		_selectionManager = new SelectionManager(_owner);
	}

	protected override IEnumerator startToManageInputs() {

		while (true) {
			yield return null;

			handleHighlightCube();

			handleToolSwitch();

			handleCameraControls();

			handleUnitOrders();

			handleToolUse();

			handleRMB();

			if (Input.GetKeyDown(KeyCode.H)) {
				uiChannel.toggleDebugScreen();
			}
		}
	}

	/**
	 * If toBreak is true, then the position of block, which raycast is aiming at is returned.
	 * Otherwise the position, where new block should be placed, is returned.
	 */
	private Vector3Int getBlockPosition(RaycastHit raycastHit) {
		int x = Mathf.FloorToInt(raycastHit.point.x);
		int y = Mathf.FloorToInt(raycastHit.point.y);
		int z = Mathf.FloorToInt(raycastHit.point.z);

		Vector3Int raycastFaceHitNormal = Vector3Int.FloorToInt(raycastHit.normal);

		if (_tool == ToolType.Break) raycastFaceHitNormal *= -1;

		Vector3Int normalCorrection = Vector3Int.zero;
		if (raycastFaceHitNormal.x < 0 ||
		    raycastFaceHitNormal.y < 0 ||
		    raycastFaceHitNormal.z < 0)
			normalCorrection = raycastFaceHitNormal;

		return new Vector3Int(x, y, z) + normalCorrection;
	}

	private void handleCameraControls() {
		if (Input.GetKey(KeyCode.W)) {
			cameraChannel.cameraMoveKeyPressed(Vector3.forward);
		}
		if (Input.GetKey(KeyCode.S)) {
			cameraChannel.cameraMoveKeyPressed(Vector3.back);
		}
		if (Input.GetKey(KeyCode.A)) {
			cameraChannel.cameraMoveKeyPressed(Vector3.left);
		}
		if (Input.GetKey(KeyCode.D)) {
			cameraChannel.cameraMoveKeyPressed(Vector3.right);
		}

		if (Input.GetKey(KeyCode.Q)) {
			cameraChannel.cameraRotateKeyPressed(Vector3.left);
		}
		if (Input.GetKey(KeyCode.E)) {
			cameraChannel.cameraRotateKeyPressed(Vector3.right);
		}

		if (Input.GetKey(KeyCode.F)) {
			cameraChannel.cameraZoomKeyPressed(Vector3.down);
		}
		if (Input.GetKey(KeyCode.R)) {
			cameraChannel.cameraZoomKeyPressed(Vector3.up);
		}
	}

	private void handleToolSwitch() {

		if (Input.GetKeyDown(KeyCode.Tab)) {
			++_tool;
			if (_tool >= (ToolType)Enum.GetValues(typeof(ToolType)).Length) {
				_tool = default;
			}

			uiChannel.changeToolType(_tool);
		}
	}

	private void handleUnitOrders() {

		// Found a rare and hard to replicate problem with this piece of code.
		// Therefore it is disabled for now
		/*if (Input.GetKeyDown(KeyCode.T)) {
			unitOrdersChannel.requestStop();
		}*/

		if (Input.GetKeyDown(KeyCode.C)) {
			unitOrdersChannel.toggleResupplier();
		}

		if (Input.GetKeyDown(KeyCode.X)) {
			unitOrdersChannel.toggleResupplyee();
		}

		if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) {
			unitProductionOrdersChannel.switchToNextOrder();
		}
		if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) {
			unitProductionOrdersChannel.addOrderToProduction();
		}
		if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) {
			unitProductionOrdersChannel.stopProduction();
		}
	}

	private void handleToolUse() {

		var checkedBlock = World.current.getBlockAt(_newPos);

		switch (_tool) {

			case ToolType.None:
				if (Input.GetMouseButtonDown(0)) {
					if (_raycastHit.collider != default &&
					    _raycastHit.transform.gameObject.TryGetComponent<ISelectable>(out var selectedEntity)) {
						_selectionManager.selectEntity(selectedEntity);
					}
					else {
						_selectionManager.deselectEntity();
					}
				}
				break;

			case ToolType.PlaceDirt:
				if (checkedBlock is WaterLoggedStructure or Structure) {
					break;
				}

				if (checkedBlock is not (Air or Water)) {
					break;
				}

				var underCheckedBlock = World.current.getBlockAt(_newPos + Vector3Int.down);
				if (underCheckedBlock is Structure or WaterLoggedStructure) {
					break;
				}

				_highlightingCubeInvalid.SetActive(false);
				_highlightingCubeValid.SetActive(true);
				if (Input.GetMouseButtonDown(0)) {
					blockPlacingChannel.placeBlock(_newPos, BlockId.DIRT);
				}

				break;

			case ToolType.PlaceWater:
				if (checkedBlock is WaterLoggedStructure or Structure or WaterSource) {
					break;
				}
				if (checkedBlock is not (Air or Water)) {
					break;
				}
				underCheckedBlock = World.current.getBlockAt(_newPos + Vector3Int.down);
				if (underCheckedBlock is Structure or WaterLoggedStructure) {
					break;
				}
				_highlightingCubeInvalid.SetActive(false);
				_highlightingCubeValid.SetActive(true);
				if (Input.GetMouseButtonDown(0)) {
					blockPlacingChannel.placeBlock(_newPos, BlockId.WATER_SOURCE);
				}
				break;

			case ToolType.Break:
				var breakingBlock = World.current.getBlockAt(_newPos);
				if (breakingBlock is Bedrock or Structure or WaterLoggedStructure) {
					break;
				}

				var overCheckedBlock = World.current.getBlockAt(_newPos + Vector3Int.up);
				if (overCheckedBlock is Structure or WaterLoggedStructure) {
					break;
				}
				_highlightingCubeInvalid.SetActive(false);
				_highlightingCubeValid.SetActive(true);
				if (Input.GetMouseButtonDown(0)) {
					blockPlacingChannel.destroyBlock(_newPos);
				}

				break;
		}
	}

	private void handleHighlightCube() {
		if (_tool is ToolType.None) {
			setRaycast(out _raycastHit);
		}
		else {
			setRaycast(out _raycastHit, 1 << 8);
		}
		_newPos = getBlockPosition(_raycastHit);

		if (_tool is ToolType.PlaceDirt or ToolType.PlaceWater or ToolType.Break) {
			_highlightingCubeValid.transform.position = _newPos;
			_highlightingCubeInvalid.transform.position = _newPos;
			_highlightingCubeInvalid.SetActive(true);
		}
		else {
			_highlightingCubeValid.SetActive(false);
			_highlightingCubeInvalid.SetActive(false);
		}
	}

	private void handleRMB() {
		if (Input.GetMouseButtonDown(1)) {
			if (_raycastHit.transform is null) {
				return;
			}

			if (_raycastHit.transform.gameObject.TryGetComponent<IDamageable>(out var damageable)) {

				if (damageable.getOwner()?.playerColor == _selectionManager.selectedObject?.getOwnerColor()) {
					unitOrdersChannel.requestMove(damageable.getPosition());
				}
				else {
					unitOrdersChannel.requestAttack(damageable);
				}

				return;
			}
			if (_raycastHit.transform.gameObject.TryGetComponent<IDropOffPoint>(out var storageNode)) {
				unitOrdersChannel.assignStorage(storageNode);
				return;
			}
			if (_raycastHit.transform.gameObject.TryGetComponent<IResource>(out var resourceNode)) {
				unitOrdersChannel.assignResource(resourceNode, resourceNode.getResourceType());
				return;
			}

			unitOrdersChannel.requestMove(_raycastHit.point);

		}
	}

	private void setRaycast(out RaycastHit raycastHit, int layerMask = Physics.IgnoreRaycastLayer | 1 << 8 | 1 << 12 )  {
		Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
		Physics.Raycast(ray, out raycastHit, float.MaxValue, layerMask);
	}

}
