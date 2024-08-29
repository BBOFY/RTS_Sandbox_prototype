using UnityEngine;

public abstract class BaseWaterInteractorCmp : MonoBehaviour {

	private bool _inWater;
	// used for controlling, if entity is still in water
	private bool _stillInWater;
	private bool _exitToggled = true;

	// Used by child classes
	protected EntityCmp _entity;
	private Collider _previousWaterCollider;

	protected virtual void Awake() {
		_entity = GetComponentInParent<EntityCmp>();
	}

	private void FixedUpdate() {
		_stillInWater = false;
	}

	private void LateUpdate() {
		if (!_stillInWater) {
			_inWater = false;

			if (!_exitToggled) {
				_exitToggled = true;
				onWaterExit();
			}
		}
	}

	private void OnTriggerEnter(Collider other) {
		// check, if collided GO layer is not Terrain_Water
		if (other.gameObject.layer != 9) {
			return;
		}

		if (!_inWater) {
			onWaterEnter();
			_exitToggled = false;
		}

		if (!_stillInWater) {
			_inWater = true;
			_stillInWater = true;
		}
		_previousWaterCollider = other;
	}

	private void OnTriggerExit(Collider other) {
		// check, if collided GO layer is not Terrain_Water
		if (other.gameObject.layer != 9) {
			return;
		}

		if (!ReferenceEquals(other, _previousWaterCollider)) {
			return;
		}
		onWaterExit();
		_exitToggled = true;
		_inWater = false;
		_stillInWater = false;
		_previousWaterCollider = null;
	}

	private void OnTriggerStay(Collider other) {
		if (_stillInWater || other.gameObject.layer != 9) {
			return;
		}
		_stillInWater = true;
	}

	protected abstract void onWaterEnter();

	protected abstract void onWaterExit();

}
