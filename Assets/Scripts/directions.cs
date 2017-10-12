using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class directions : MonoBehaviour {

	public Text direction;
	private bool rudder;
	private bool wait;
	private bool pullUp;
	private bool tooMuch;
	private bool last;
	public Vector2 yaw;

	// Use this for initialization
	void Start () {
		direction = gameObject.GetComponent<Text> ();
		rudder = true;
		wait = true;
		pullUp = true;
		tooMuch = true;
		last = true;
	}

	// Update is called once per frame
	void Update () {
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		udpstuff udp = g.GetComponent<udpstuff> ();
		yaw = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);

		if (udp.speed > 5f && rudder) {
			direction.text = "Use the left joystick to stay centered";
			rudder = false;
		}
			
		if(!rudder && yaw.x>0.1f && wait){
			direction.text = "Wait till the speed hits 80 knots and then pull up";
			wait = false;
		}
		if (udp.speed > 80f && pullUp) {
			direction.text = "Pull Up SLOWLY";
			pullUp = false;
		}
		if (udp.pitch > 20f && tooMuch) {
			direction.text = "Pulling up too Fast. Push Down to 5 degrees";
			tooMuch = false;
		}
		if (udp.altitude > 1000f && last) {
			direction.text = "Fly around. Try and go through the hoops";
			last = false;
		}
		if (!last && udp.altitude > 1050f) {
			gameObject.SetActive (false);
		}
	}
}
