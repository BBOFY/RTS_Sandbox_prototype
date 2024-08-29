using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSystem {

	protected ToolType _tool;

	protected readonly Player _owner;
	protected readonly Camera _camera;

	public readonly UIChannel uiChannel;
	public readonly UnitOrdersChannel unitOrdersChannel;
	public readonly UnitProductionOrdersChannel unitProductionOrdersChannel;
	protected readonly BlockPlacingChannelSO blockPlacingChannel;
	protected readonly CameraChannelSO cameraChannel;

	protected InputSystem(Player owner) {
		_owner = owner;
		_tool = ToolType.None;
		_camera = Camera.main;

		uiChannel = new UIChannel();
		unitOrdersChannel = new UnitOrdersChannel();
		unitProductionOrdersChannel = new UnitProductionOrdersChannel();

		blockPlacingChannel = Resources.Load<BlockPlacingChannelSO>("Channels/BlockPlacingChannel");
		cameraChannel = Resources.Load<CameraChannelSO>("Channels/CameraChannel");

		CoroutineStarter.current.startCoroutine(startToManageInputs());
	}

	/// <summary>
	/// This logic is not included in constructor for the synchronisation reasons
	/// with Awake and Start
	/// methods called by engine.
	/// </summary>
	public abstract void init();

	public virtual void start() {}

	protected abstract IEnumerator startToManageInputs();


}
