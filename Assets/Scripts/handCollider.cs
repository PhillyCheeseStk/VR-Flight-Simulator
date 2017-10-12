using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class handCollider : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<SphereCollider> ().enabled = false;
	}


	public bool AE;
	public float BE;
	public bool CE;
	// Update is called once per frame
	void Update () {
		AE = OVRInput.Get (OVRInput.NearTouch.SecondaryIndexTrigger);
		BE = OVRInput.Get (OVRInput.Axis1D.SecondaryIndexTrigger);
		CE = OVRInput.Get (OVRInput.Touch.SecondaryThumbRest);

		if (CE && (AE == false || BE>0.5f)) {
			gameObject.GetComponent<SphereCollider> ().enabled = true;
		} else {
			gameObject.GetComponent<SphereCollider> ().enabled = false;
		}
	}
}
