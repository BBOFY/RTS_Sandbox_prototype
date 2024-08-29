using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceProductionOrder", menuName = "Scriptable objects/Production/ResourceProductionOrder")]
public class ResourceProductionOrderSO : ScriptableObject {
	public string productName;

	[Tooltip("Seconds to produce desired amount of resource")]
	public float productionTime = 1f;

	[Header("Cost")]
	[SerializeField]
	private int food = 0;
	[SerializeField]
	private int wood = 0;
	[SerializeField]
	private int gold = 0;
	[SerializeField]
	private int iron = 0;

	[Space]
	[Header("Product")]
	[SerializeField]
	private ResourceType _resourceType;
	[SerializeField]
	private int _amountToProduce;

	public ResourceData resourceToProduce => new (_resourceType, _amountToProduce);

	public Dictionary<ResourceType, int> cost() {

		Dictionary<ResourceType, int> toReturn = new () {
			{ResourceType.Food, food},
			{ResourceType.Wood, wood},
			{ResourceType.Gold, gold},
			{ResourceType.Iron, iron}
		};

		return toReturn;
	}

}
