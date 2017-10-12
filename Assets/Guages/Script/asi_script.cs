using UnityEngine;
using System.Collections;

public class asi_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
		

	public static float speed;
	public static float maxSpeed = 240f;
	public static float speedFactor;

	
	// Update is called once per frame
	void Update () {

		//apply the aircraft script to the plane object and reference it with the tag
		//right now its pointed at the asi needle image
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		speed = aircraft.velocity;

		//speed += Time.deltaTime;

		speedFactor = speed * (101.5f / 80);
//		if (speed <= 130) {
//			speedFactor = speed * (210.226f / 130);
//		} else if (speed <= 160) {
//			speedFactor = 130f * (210.226f / 130) + speed * (264.139f / 160);
//		} else {
//			speedFactor = 130f * (210.226f / 130) + 160f * (264.139f / 160) + speed * (312.442f / 200);
//		}

		Vector3 temp = transform.rotation.eulerAngles;
		temp.z = -speedFactor;
		transform.rotation = Quaternion.Euler(temp);
	}
}
