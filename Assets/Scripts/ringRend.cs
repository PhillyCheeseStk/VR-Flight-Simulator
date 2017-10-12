using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ringRend : MonoBehaviour {

	public Renderer rend;
	private bool wait;
	public float startDelay;
	public float interval;

	void Start () {
		rend = gameObject.GetComponent<Renderer> ();
		rend.enabled = false;
		wait = true;

		interval = 1f;
		startDelay = 0.5f;
	}



	// Update is called once per frame
	void Update () {
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		udpstuff udp = g.GetComponent<udpstuff> ();

		if (udp.speed > 9f && wait) {
			InvokeRepeating ("ToggleState", startDelay, interval);
			wait = false;
			}
		if (udp.speed > 80f) {
			CancelInvoke ();
			rend.enabled = false;
		}
		}
		

	void ToggleState(){
		rend.enabled = !rend.enabled;
	}
}
