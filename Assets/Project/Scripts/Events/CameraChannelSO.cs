using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraChannel", menuName = "Scriptable objects/Channels/CameraChannel")]
public class CameraChannelSO : ScriptableObject {

	public event Action<Vector3> onCameraMoveKeyPressed;
	public void cameraMoveKeyPressed(Vector3 direction) {
		onCameraMoveKeyPressed?.Invoke(direction);
	}

	public event Action<Vector3> onCameraRotateKeyPressed;
	public void cameraRotateKeyPressed(Vector3 direction) {
		onCameraRotateKeyPressed?.Invoke(direction);
	}

	public event Action<Vector3> onCameraZoomKeyPressed;
	public void cameraZoomKeyPressed(Vector3 direction) {
		onCameraZoomKeyPressed?.Invoke(direction);
	}

}
