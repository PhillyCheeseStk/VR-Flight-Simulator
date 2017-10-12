using UnityEngine;
using System.Collections;

public class adi_pitch_script : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

	//anglePitch
	public static float deg;
	public static float maxDeg = 90;
	public static Vector3 origin = new Vector3 (251.4306f, 0.008131385f, 0.9549f);
	public static float pitch;

	// Update is called once per frame
	void Update () {

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		deg = aircraft.anglePitch;

		pitch = origin.y + (-1f * deg * (0.006795864f / 10));

		Vector3 temp = transform.localPosition;
		temp.y = pitch;

		transform.localPosition = temp;
	}
}
