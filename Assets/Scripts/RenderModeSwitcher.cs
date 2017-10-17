using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RenderModeSwitcher : MonoBehaviour {


	// The major two render modes involve following steps:
	// - Mask renders a front and a full color back, which then get
	//   placed into properly, this works best on simple render scenes
	// - Replace renders a front alpha mask and a full back, where the 
	//   full back gets mixed with the camera alpha + front alpha.
	//   this produces a worse image reproduction, but usually works with all
	//   kinds of complicated render situations
	// Note: Deferred sets all cameras into Deferred mode

	public enum RenderMode {
		MaskDeferredMode, MaskForwardMode,
		ReplaceDeferredMode, ReplaceMode
	}
	[SerializeField,HideInInspector]
	private RenderMode _renderMode;

	public RenderMode CurrentRenderMode {
		get {
			return _renderMode;
		}
		set {
			_renderMode = value;
			ChangeMode();
		}
	}

	public Material maskMaterial;
	public Material replaceMaterial;

	public Material activeMaterial;
	public Camera frontCamera;
	public RenderBufferSwapper renderBufferSwapper;


	void Start () {
		if(!maskMaterial || !replaceMaterial) {
			Debug.LogError("Tri Step Material or Replace Material was not set");
		}
		if(!frontCamera) {
			var cams = GameObject.FindGameObjectsWithTag("Front Camera");
			if(cams.Length != 1) {
				Debug.LogError("No front camera set and none is found.");
			}
			frontCamera = cams[0].GetComponent<Camera>();
		}
		if(!frontCamera) {
			Debug.LogError(
				"GameObject tagged with 'Front Camera' " +
				"does not contain a camera."
			);
		}

		if(renderBufferSwapper == null) {
			Debug.LogError("Renderbuffer Swapper cannot be null.");
		}

		ChangeMode();
	}
	
	void ChangeMode() {
		Debug.LogWarning("Render Mode change triggered - this might break some instances.");
		switch(_renderMode) {
			case RenderMode.MaskForwardMode:
			case RenderMode.MaskDeferredMode:
				activeMaterial = maskMaterial;
				activeMaterial.CopyPropertiesFromMaterial(replaceMaterial);
				break;
			case RenderMode.ReplaceMode:
			case RenderMode.ReplaceDeferredMode:
				activeMaterial = replaceMaterial;
				activeMaterial.CopyPropertiesFromMaterial(maskMaterial);
				break;
		}

		switch(_renderMode) {
			case RenderMode.MaskDeferredMode:
			case RenderMode.ReplaceDeferredMode:
				frontCamera.renderingPath = RenderingPath.DeferredShading;
				break;
			case RenderMode.MaskForwardMode:
			case RenderMode.ReplaceMode:
				frontCamera.renderingPath = RenderingPath.Forward;
				break;
		}

		renderBufferSwapper.targetMaterial = activeMaterial;
		renderBufferSwapper.ResetWebcam();
	}
}
