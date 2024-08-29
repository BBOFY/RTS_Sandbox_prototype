using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FactoryCmp : MonoBehaviour {

	private EntityCmp _entity;

	[FormerlySerializedAs("_resourceReceipts")]
	[SerializeField]
	private List<ResourceProductionOrderSO> _resourceRecipe = new ();

	[SerializeField]
	private int startProductionAmount = 200;
	[SerializeField]
	private int stopProductionAmount = 400;

	private void Awake() {
		_entity = GetComponent<EntityCmp>();
	}

	private void Start() {

		foreach (var recipe in _resourceRecipe) {
			StartCoroutine(factoryProductionProcess(recipe));
		}
	}

	/** Periodically produce resource defined in recipe, or wait until valid amount
	 *	of resources will be present for production to be able to continue.
	 */
	private IEnumerator factoryProductionProcess(ResourceProductionOrderSO recipe) {

		var inventory = _entity.owner.inventory;

		var needToProduce = false;

		while (true) {

			yield return new WaitForSeconds(5f);

			if (!needToProduce && inventory.showAmount(recipe.resourceToProduce.type) <= startProductionAmount) {
				needToProduce = true;
			}
			else if (inventory.showAmount(recipe.resourceToProduce.type) >= stopProductionAmount) {
				needToProduce = false;
				continue;
			}

			if (!needToProduce) {
				continue;
			}

			while (!inventory.removeExact(recipe.cost())) {
				yield return null;
			}

			yield return new WaitForSeconds(recipe.productionTime);

			inventory.add(recipe.resourceToProduce);

			yield return null;
		}

	}
}
