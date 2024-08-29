using UnityEngine;

public interface IDamageable {

	public void doDamage(DamageData damageData);

	public Vector3 getPosition();

	public bool isDestroyed();

	public Player getOwner();

	public PlayerColor getOwnerColor();

}

