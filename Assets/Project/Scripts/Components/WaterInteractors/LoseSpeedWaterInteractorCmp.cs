using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LoseSpeedWaterInteractorCmp : BaseWaterInteractorCmp {

	private NavMeshAgent _agent;
	private MoveCmp _moveCmp;

	protected override void Awake() {
		_agent = GetComponent<NavMeshAgent>();
		_moveCmp = GetComponent<MoveCmp>();
	}

	protected override void onWaterEnter() {
		_agent.speed *= _moveCmp.entityWaterSpeedPenalty;
	}

	protected override void onWaterExit() {
		_agent.speed /= _moveCmp.entityWaterSpeedPenalty;
	}
}
