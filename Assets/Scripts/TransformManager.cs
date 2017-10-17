using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TransformManager : MonoBehaviour {

	public delegate void TransformUpdate(Transform newTransform);

	private static TransformManager instance;
	public static TransformManager Instance {
		get {
			return GetInstance();
		}
	}

	private Dictionary<string, TransformStorage> tfStorage;

	void Awake () {
		if(instance != null) {
			Debug.LogWarning("This instance will delete itself.");
			Component.Destroy(this);
			return;
		}

		instance = this;
		tfStorage = new Dictionary<string, TransformStorage>();
	}

	/// <summary>
	/// Gets a named transform and calls the delegate. You can retrieve
	/// it via the callback or just by the return value.
	/// The delegate gets called when the storage gets updated with that name.
	/// </summary>
	public Transform GetTransform(string name, TransformUpdate tuDelegate) {
		var transStorage = tfStorage[name];
		if(transStorage == null) {
			tfStorage[name] = new TransformStorage{delegates = tuDelegate};
			return null;
		}
		transStorage.delegates += tuDelegate;
		tuDelegate(transStorage.transform);
		return transStorage.transform;
	} 

	public void AddTransform(string name, Transform transform, bool force=false) {
		if(!tfStorage.ContainsKey(name)) {
			tfStorage[name] = new TransformStorage{transform = transform};
			return;
		}

		if(force) {
			var transStorage = tfStorage[name];
			transStorage.transform = transform;
			if(transStorage.delegates != null) {
				transStorage.delegates(transform);
			}
			Debug.LogWarning(
				"A key with name [" + name + "] was already added," + 
				" but it was force-overwritten."
			);
			return;
		}
		Debug.LogWarning(
			"A key named [" + name + "] already existed and" + 
			" was not force-overwritten."
		);
	}

	private static TransformManager GetInstance() {
		if(instance != null) {
			return instance;
		}

		var go = new GameObject("Controller Transform Manager (generated)");
		var mngr = go.AddComponent<TransformManager>();
		return mngr;
	}

	private class TransformStorage {
		public Transform transform;
		public TransformUpdate delegates;
	}
}
