using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPlayer : Player {

	public ComputerPlayer(
		PlayerColor playerColor = default,
		string playerName = "Computer Player"
	) : base(
		playerColor, playerName) {

		inputSystem = new ComputerInputSystem(this);
	}

	public override void initPlayerSystems(Dictionary<ResourceType, int> inv = null) {

		Dictionary<ResourceType, int> computerPlayerInventory = new () {
			{ResourceType.Food, Int32.MaxValue},
			{ResourceType.Wood, Int32.MaxValue},
			{ResourceType.Gold, Int32.MaxValue},
			{ResourceType.Iron, Int32.MaxValue},
			{ResourceType.Provision, Int32.MaxValue},
			{ResourceType.Ammunition, Int32.MaxValue}
		};

		base.initPlayerSystems(computerPlayerInventory);

	}


}
