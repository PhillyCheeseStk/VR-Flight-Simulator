using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour {

	public Vector3 initialPosition;

	void Start(){
		initialPosition = transform.localPosition;
	}

	void OnTriggerEnter(Collider other) {
		if (transform.position == initialPosition) {
			Vector3 temp = transform.localPosition;
			temp.x += 0.0023f;
			transform.position = temp;
		} else {
			transform.localPosition = initialPosition;
		}
	}

}
