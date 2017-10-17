using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

	public Camera fullCamera;
	public Camera frontCamera;
	public Camera stencilCamera;
	public Camera lightCamera;
	private Camera[] ManagedCameras {
		get {
			return new Camera[]{fullCamera, frontCamera, stencilCamera, lightCamera};
		}
	}
	public enum LensType {
		RectliniarFilm,
		RectliniarDSLR
	}
	public LensType type = LensType.RectliniarFilm;
	public float focalLength;
	[HideInInspector, SerializeField]
	public float FocalLength {
		get {
			return focalLength;
		}
		set {
			focalLength = value;
			RecalculateFOV();
		}
	}
	public Vector2 SensorSize {
		get {
			return sensorSize;
		}
		set {
			sensorSize = value;
			RecalculateFOV();
		}
	}
	[HideInInspector, SerializeField]
	public Vector2 sensorSize;
	public float fieldOfView;

	public string rootTransformName;
	public string controllerTransformName;
	public string headsetTransformName;

	private Transform rootTransform;
	private Transform controllerTransform;
	private Transform headsetTransform;

	void Start () {
		var tfManager = TransformManager.Instance;
		tfManager.GetTransform(rootTransformName, (t) => { rootTransform = t; });
		tfManager.GetTransform(controllerTransformName, (t) => { controllerTransform = t; });
		tfManager.GetTransform(headsetTransformName, (t) => { headsetTransform = t; });

		RecalculateFOV();
	}

	void Update() {
		rootTransform.position = controllerTransform.position;
		rootTransform.rotation = controllerTransform.rotation;

		frontCamera.farClipPlane = Vector3.Distance(
			rootTransform.position, headsetTransform.position
		);
	}
	
	private void RecalculateFOV() {
		double fovdub = Mathf.Rad2Deg * 2.0 * Mathf.Atan(sensorSize.y  / (2f * focalLength));
		fieldOfView = (float) fovdub;
		foreach(Camera c in ManagedCameras) {
			c.fieldOfView = fieldOfView;
		}
	}
}
