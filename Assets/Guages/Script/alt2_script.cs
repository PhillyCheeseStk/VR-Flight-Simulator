using UnityEngine;
using System.Collections;

public class alt2_script : MonoBehaviour {


	// Use this for initialization
	void Start () {

	}

	public static float alt;
	public static float maxAlt = 10;
	public static float altFactor;


	// Update is called once per frame
	void Update () {

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		alt = (aircraft.altitude - Mathf.Floor (aircraft.altitude / 1000f)) / 100;

		altFactor = alt*(360/maxAlt);
		Vector3 temp = transform.rotation.eulerAngles;
		temp.z = -altFactor;
		transform.rotation = Quaternion.Euler(temp);
	}
}