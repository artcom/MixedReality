using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFrustumPlacer : MonoBehaviour {

	[Range(1, 100)]
	public float distance;
	private Transform follower;
	public GameObject plane;
	[Tooltip("This is the lightning camera with an attached quad.")]
	private Camera lightCam;

	[Tooltip("Would likely be the HMD - therefore the HMD name would be appropiate")]
	public string followerName;

	public TransformManager ctm;

	void Start () {
		lightCam = GetComponent<Camera>();
		if(!lightCam) {
			Debug.LogError("This script is not attached to a camera.");
			return;
		}
		lightCam.nearClipPlane = 0.999f;
		lightCam.farClipPlane = 101f;

		if(ctm == null) {
			ctm = TransformManager.Instance;
		}
		if(follower == null) {
			ctm.GetTransform(followerName, (transform) => {
				follower = transform;
			});
		}
	}
	
	void Update () {
		var followerDistance = Vector3.Distance(lightCam.transform.position, follower.position);
		distance = Mathf.MoveTowards(distance, followerDistance, 3);
		var height = 2.0 * Mathf.Tan(0.5f * lightCam.fieldOfView * Mathf.Deg2Rad) * distance;
		var width = height * Screen.width / Screen.height;

		plane.transform.localScale = new Vector3((float) width / 10f, 1, (float) height / 10);
		plane.transform.position = lightCam.transform.position + lightCam.transform.forward * distance;
	}
}
