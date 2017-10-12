using UnityEngine;
using System.Collections;

public class tcBall_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame

	//angleYaw
	//based on the tail sway of the plane when attempting a rotation
	//sway outward then ball moves to the right (so pilot steps on right pedal to fix)
	//assume 90 deg sway then ball at the max

	public static float deg;
	public static float maxDeg = 90;
	public static float xFactor;
	public static float yFactor;
	//harcode origin once positioned properly
	public static Vector3 origin = new Vector3 (-276.1302f, 33.89961f, 64.7743f);

	void Update () {

		//the 41 and 4 is based on how many pixels to the edge of the groove

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		deg = aircraft.angleYaw;

		xFactor = origin.x + (deg * 0.02f / 90);
		if (deg >= 0) {
			yFactor = origin.y + (deg * 0.00227f / 90);	
		} else {
			yFactor = origin.y + (-deg * 0.00227f / 90);
		}

		Vector3 temp = transform.localPosition;
		temp.x = xFactor;
		temp.y = yFactor;

		transform.localPosition = temp;


	}
}
