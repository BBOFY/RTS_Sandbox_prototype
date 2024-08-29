// Logic inspired by https://www.youtube.com/watch?v=rnqF6S7PfFA

using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour {

	[SerializeField]
	private Transform cameraTransform;

	[SerializeField]
	private CameraChannelSO _cameraChannel;

	public float movementSpeed;
	public float movementTime;
	public float rotationAmount;
	public float zoomAmount;
	public float startingZoom;

	[Header("Limits")]
	public float minZoom;
	public float maxZoom;

	private float borderMinX;
	private float borderMaxX;
	private float borderMinZ;
	private float borderMaxZ;

	private Vector3 zoomMat;
	private Vector3 newPosition;
	private Quaternion newRotation;
	private float newZoom;

	private void Awake() {
		_cameraChannel.onCameraMoveKeyPressed += handleMoveInput;
		_cameraChannel.onCameraRotateKeyPressed += handleRotateInput;
		_cameraChannel.onCameraZoomKeyPressed += handleZoomInput;
	}

	private IEnumerator Start() {

		while (!World.current.isInitialized) {
			yield return null;
		}

		transform.position = World.current.getPrimaryPlayer().rallyPoint;

		newPosition = transform.position;
		newRotation = transform.rotation;
		zoomMat = new Vector3(0, 1, -1);
		newZoom = startingZoom;

		borderMinX = borderMinZ = -100;
		borderMaxX = borderMaxZ = ChunkData.chunkWidth * ChunkData.worldSizeInChunks + 100;
	}

	private void OnDestroy() {
		_cameraChannel.onCameraMoveKeyPressed -= handleMoveInput;
		_cameraChannel.onCameraRotateKeyPressed -= handleRotateInput;
		_cameraChannel.onCameraZoomKeyPressed -= handleZoomInput;
	}

	private void LateUpdate() {
		updateCameraTransform();
	}

	private void handleMoveInput(Vector3 direction) {
		if (direction == Vector3.forward) {
			newPosition += transform.forward * movementSpeed;
		}
		if (direction == Vector3.back) {
			newPosition -= transform.forward * movementSpeed;
		}
		if (direction == Vector3.right) {
			newPosition += transform.right * movementSpeed;
		}
		if (direction == Vector3.left) {
			newPosition -= transform.right * movementSpeed;
		}

		if (newPosition.x < borderMinX) {
			newPosition.x = borderMinX;
		}
		if (newPosition.x > borderMaxX) {
			newPosition.x = borderMaxX;
		}
		if (newPosition.z < borderMinZ) {
			newPosition.z = borderMinZ;
		}
		if (newPosition.z > borderMaxZ) {
			newPosition.z = borderMaxZ;
		}
	}

	private void handleRotateInput(Vector3 direction) {
		if (direction == Vector3.left) {
			newRotation *= Quaternion.Euler(Vector3.up * rotationAmount);
		}
		if (direction == Vector3.right) {
			newRotation *= Quaternion.Euler(Vector3.up * -rotationAmount);
		}
	}

	void handleZoomInput(Vector3 direction) {
		if (direction == Vector3.down) {
			newZoom += zoomAmount;
		}
		if (direction == Vector3.up) {
			newZoom -= zoomAmount;
		}

		if (newZoom < minZoom) {
			newZoom = minZoom;
		}
		if (newZoom > maxZoom) {
			newZoom = maxZoom;
		}

	}

	void updateCameraTransform() {

		transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
		cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom * zoomMat, Time.deltaTime * movementTime);
	}
}
