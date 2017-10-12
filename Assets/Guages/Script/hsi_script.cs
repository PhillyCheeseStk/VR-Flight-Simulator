using UnityEngine;
using System.Collections;

public class hsi_script : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}

	public static float hSit;
	public static float maxH = 36;
	public static float hSFactor;


	// Update is called once per frame
	void Update () {
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		hSit = aircraft.gyro/10;

		hSFactor = hSit * (360 / maxH) + aircraft.angleRoll;
		Vector3 temp = transform.rotation.eulerAngles;
		temp.z = hSFactor;

		transform.rotation = Quaternion.Euler(temp);
	}
}