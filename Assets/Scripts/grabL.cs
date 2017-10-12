using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grabL : MonoBehaviour {

	// Use this for initialization
	public float grabRadius;
	public LayerMask grabMask;

	private GameObject grabbedObject;
	private bool grabbing;

	void GrabObject(){
		grabbing = true;

		RaycastHit[] hits;

		hits = Physics.SphereCastAll (transform.position, grabRadius, transform.forward, 0f, grabMask);

		if (hits.Length > 0) {

			int closestHit = 0;

			for (int i = 0; i < hits.Length; i++) {
				if (hits [i].distance < hits [closestHit].distance)
					closestHit = i;
			}

			grabbedObject = hits [closestHit].transform.gameObject;
			grabbedObject.GetComponent<Rigidbody> ().isKinematic = true;
			//grabbedObject.transform.position = transform.position;
			grabbedObject.transform.parent= transform;

			grabbedObject = hits [closestHit].transform.gameObject;
		}

	}

	void DropObject(){
		grabbing = false;

		if (grabbedObject != null) {
			grabbedObject.transform.parent = null;
			grabbedObject.GetComponent<Rigidbody> ().isKinematic = false;

			grabbedObject = null;
		}


	}

	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Oculus_GearVR_LIndexTrigger") >= 0.8)
			GrabObject ();
		if (Input.GetAxis ("Oculus_GearVR_LIndexTrigger") < 0.8)
			DropObject ();

	}
}
