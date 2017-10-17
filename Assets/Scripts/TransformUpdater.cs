using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformUpdater : MonoBehaviour {
	public string transformName;
	public Vector3 fallbackPosition;
	public Quaternion fallbackRotation;
	public Vector3 fallbackScale;
	private Transform transf;

	public enum MirrorMode {
		None, Position, PositionRotation, PositionScale, Rotation, RotationScale,
		Scale, PositionRotationScale
	};
	public MirrorMode mirrorMode;

	public bool onLocalPosition;
	public bool onLocalRotation;

	void Start () {
		TransformManager.Instance.GetTransform(transformName, (transform) => {
			transf = transform; 
		});
	}
	
	void Update () {
		if(transf == null) {
			transform.position = fallbackPosition;
		}

		Vector3 pos, scale;
		Quaternion rot;

		switch(mirrorMode) {
			case MirrorMode.Position:
			case MirrorMode.PositionScale:
			case MirrorMode.PositionRotation:
			case MirrorMode.PositionRotationScale:
				pos = transf ? transf.position : fallbackPosition;
				break;
			default:
				pos = onLocalPosition ? transform.localPosition : transform.position;
				break;
		}

		switch(mirrorMode) {
			case MirrorMode.RotationScale:
			case MirrorMode.PositionRotation:
			case MirrorMode.Rotation:
			case MirrorMode.PositionRotationScale:
				rot = transf ? transf.rotation : fallbackRotation;
				break;
			default:
				rot = onLocalRotation ? transform.localRotation : transform.rotation;
				break;
		}

		switch(mirrorMode) {
			case MirrorMode.Scale:
			case MirrorMode.RotationScale:
			case MirrorMode.PositionScale:
			case MirrorMode.PositionRotationScale:
				scale = transf ? transf.localScale : fallbackScale;
				break;
			default:
				scale = transform.localScale;
				break;
		}
		
		if(onLocalPosition) {
			transform.localPosition = pos;
		} else {
			transform.position = pos;
		}

		if(onLocalRotation) {
			transform.localRotation = rot;
		} else {
			transform.rotation = rot;
		}

		transform.localScale = scale;		
	}
}
