using UnityEngine;
using System.Collections;

public class adi_bank_script : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	//angleRoll
	public static float turn;
	public static float maxAngle = 360;


	// Update is called once per frame
	void Update () {
	//	x = x + Time.deltaTime;

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		turn = aircraft.angleRoll;

		Vector3 temp = transform.rotation.eulerAngles;
		//temp.z = 2*turn;
		temp.z = 0;
		transform.rotation = Quaternion.Euler(temp);
	}
}