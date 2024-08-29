using UnityEngine;

public interface IResource {

	public ResourceData getResource();

	public Vector3 getWorldPosition();

	public float getRadius();

	public bool isDestroyed();

	public ResourceType getResourceType();
}
