using UnityEngine;

public interface IDropOffPoint {

	public Player getOwner();

	public void store(ResourceData resourceData);

	public Vector3 getWorldPosition();

	public float getRadius();

	public bool isDestroyed();

}
