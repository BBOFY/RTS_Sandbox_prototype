using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "UnitProductionOrder", menuName = "Scriptable objects/Production/UnitProductionOrder")]
public class UnitProductionOrderSO : ScriptableObject {
	public string productName;

	[Tooltip("Seconds to produce one unit")]
	public float productionTime = 1f;

	[FormerlySerializedAs("population")]
	[Header("Cost")]
	public int populationCost = 0;
	[Space]
	public int food = 0;
	public int wood = 0;
	public int gold = 0;
	public int iron = 0;

	[Space]
	public GameObject prefabToProduce;

	public Dictionary<ResourceType, int> cost() {

		Dictionary<ResourceType, int> toReturn = new ();

		toReturn.Add(ResourceType.Food, food);
		toReturn.Add(ResourceType.Wood, wood);
		toReturn.Add(ResourceType.Gold, gold);
		toReturn.Add(ResourceType.Iron, iron);

		return toReturn;
	}

}
