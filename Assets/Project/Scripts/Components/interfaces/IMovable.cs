using UnityEngine;

public interface IMovable {

	public void move(Vector3 destination, float stoppingDistance = 0);

	// public bool moveIfPathExists(Vector3 destination, float stoppingDistance = 0);

	public void stop();
}
