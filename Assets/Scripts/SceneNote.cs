using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneNote : MonoBehaviour {
	[TextArea(25, 25)]
	public string note;
}
