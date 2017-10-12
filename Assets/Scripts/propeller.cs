using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propeller : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		udpstuff udp = g.GetComponent<udpstuff> ();

		Quaternion temp = transform.localRotation;
		temp.x = udp.speed;
		transform.rotation = temp;
	}
}
