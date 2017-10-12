using UnityEngine;
using System.Collections;

public class tcMark_script : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	public static float rot;
	public static float maxRot = 90f;
	public static float rotFactor;

	//angleRoll
	// Update is called once per frame
	void Update () {
		//speed += Time.deltaTime;
		//if the first mark is not supposed to be 30deg then remove transformation factor
		//as in rotFactor=rot

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		rot = aircraft.angleRoll;

		rotFactor = -rot * (20f / 30) - rot;

		Vector3 temp = transform.rotation.eulerAngles;
		temp.z = -rotFactor;
		transform.rotation = Quaternion.Euler(temp);
	}
}