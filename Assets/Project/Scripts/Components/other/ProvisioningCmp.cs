using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProvisioningCmp : MonoBehaviour {

	private EntityCmp _entity;
	private ProvisionsInventoryCmp _provisionsInventory;

	private bool _entityIsPenalized;

	private List<IBeingSupplied> _suppliedComponents = new ();

	[SerializeField]
	private int _usageAmount;

	[Header("Provisions usage rates in seconds")]
	[Tooltip("Show how many seconds for one usage amount of provisions will pass when entity is idle")]
	[SerializeField]
	private float _waitForUseWhenIdle = 1;

	[Tooltip("Show how many seconds for one usage amount of provisions will pass when entity is not idle")]
	[SerializeField]
	private float _waitForUseWhenActive = 1;

	// [SerializeField]
	[Tooltip("Survival time of the entity without any provisions")]
	private float _timeToDie;


	private void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
		_provisionsInventory = GetComponent<ProvisionsInventoryCmp>();

		_suppliedComponents.AddRange(_entity.gameObject.GetInterfacesInChildren<IBeingSupplied>());
		// _suppliedComponents.AddRange(GameObjectExtensions.GetInterfaces<IBeingSupplied>(_entity.gameObject));
	}

	private void Start() {
		StartCoroutine(provisioningProcess());
	}

	private void OnDisable() {
		StopAllCoroutines();
	}


	/// <summary>
	/// Coroutine, that will slowly deplete entity's provisions
	/// </summary>
	/// <returns></returns>
	private IEnumerator provisioningProcess() {

		while (true) {

			yield return null;

			if ((_provisionsInventory.isEmpty() || !_provisionsInventory.takeExactFromInv(_usageAmount))
			    && !_entityIsPenalized) {
				_entityIsPenalized = true;
				_suppliedComponents.ForEach(suppliedComponent => {suppliedComponent.penalize();});
				StartCoroutine(dyingProcess());
			}

			else if (!_provisionsInventory.isEmpty() && _entityIsPenalized) {
				_entityIsPenalized = false;
				_suppliedComponents.ForEach(suppliedComponent => {suppliedComponent.restore();});
				StopCoroutine(dyingProcess());
			}

			if (_entityIsPenalized) {
				continue;
			}

			if (_entity.state == EntityState.Idle) {
				yield return new WaitForSeconds(_waitForUseWhenIdle);
			}
			else {
				yield return new WaitForSeconds(_waitForUseWhenActive);
			}

		}
	}

	// "Counts down" the time the entity will be without the provisions and kills it. Currently this function is disabled.
	private IEnumerator dyingProcess() {
		yield break;
		// yield return new WaitForSeconds(_timeToDie);
		// _entity.die();
	}

}
