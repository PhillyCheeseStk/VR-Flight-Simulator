using UnityEngine;
using System.Collections;

public class vsi_script : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}

	public static float vSpeed; 
	public static float maxV = 20f;
	public static float speedFactor;


	// Update is called once per frame
	void Update () {
		//speed += Time.deltaTime;

		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		aircraft_script aircraft = g.GetComponent<aircraft_script> ();
		vSpeed = (101.288f * aircraft.velocity * Mathf.Sin (aircraft.anglePitch * (Mathf.PI / 180))) / 100;
		speedFactor = vSpeed * (172f / 20);

		Vector3 temp = transform.rotation.eulerAngles;
		temp.z = -speedFactor;
		transform.rotation = Quaternion.Euler(temp);
	}
}
