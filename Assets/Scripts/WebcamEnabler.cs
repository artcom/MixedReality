using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WebcamEnabler : MonoBehaviour {
	private static string[] _camNames;
	public static string[] CamNames {
		get {
			if(_camNames == null) {
				ReloadCameras();
			}
			return _camNames;
		}
	}
	public static void ReloadCameras() {
		List<string> container = new List<string>();
		WebCamTexture.devices.ToList().ForEach(x => {container.Add(x.name);});
		_camNames = container.ToArray();
	}

	[SerializeField]
	private int _camIndex;
	public int CamIndex {
		get {
			return _camIndex;
		}
		set {
			if(_camIndex != value) {
				_camIndex = value;
				deviceName = CamNames[value];
				ResetCamera();
			}
		}
	}
	public Material material;
	public string shaderPropertyName = "_MainTex";
	public string deviceName;

	public Vector2 imageSize;
	public int frameRate;

	public WebCamTexture webcamTexture;

	// Use this for initialization
	void Start () {
		if(imageSize.x == 0 && imageSize.y == 0) {
			imageSize = new Vector2(1280, 720);
		}
		ResetCamera();
	}

	void OnDisable() {
		if(webcamTexture != null && webcamTexture.isPlaying) {
			webcamTexture.Stop();
		}
	}

	void ResetCamera() {
		if(webcamTexture != null && webcamTexture.isPlaying) {
			webcamTexture.Stop();
		}
		if(!Application.isPlaying) {
			return;
		}
		Debug.Log("Restarting Camera: " + deviceName);
		webcamTexture = new WebCamTexture(deviceName, (int) imageSize.x, (int) imageSize.y, frameRate);
		webcamTexture.name = "Webcam Texture (generated)";
		webcamTexture.Play();
		material.SetTexture(shaderPropertyName, webcamTexture);
	}

	public void StopCamera() {
		if(webcamTexture != null && webcamTexture.isPlaying) {
			webcamTexture.Stop();
		}
	}
}