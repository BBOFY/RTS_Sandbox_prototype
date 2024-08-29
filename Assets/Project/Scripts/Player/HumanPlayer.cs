using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanPlayer : Player {
	public HumanPlayer(
		PlayerColor playerColor = default,
		string playerName = "Human Player"
	) : base(playerColor, playerName) {

		inputSystem = new HumanPlayerInputSystem(this);

	}
}
