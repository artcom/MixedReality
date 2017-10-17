using UnityEngine;

public class TrackableTransform : MonoBehaviour {
	public string transformName;
	void Start () {
		if(string.IsNullOrEmpty(transformName)) {
			Debug.LogError("Trackable Transform Name cannot be null or empty!");
			return;
		}
		TransformManager.Instance.AddTransform(transformName, this.transform);	
	}
}
