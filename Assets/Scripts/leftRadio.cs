using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leftRadio : MonoBehaviour {

	public Text txt;
	public float station;
//	public GameObject buttonLeft;
//	public GameObject knobLeft;
//	public Vector3 initialRot;
//	public float debug = 0;
	// Use this for initialization
	void Start () {
		txt = gameObject.GetComponent<Text>();
		station = 120.12f;
		//knobLeft = GameObject.Find ("test");
		//initialRot = knobLeft.transform.localEulerAngles;

	}
	
	// Update is called once per frame
	void Update () {
//		debug = knobLeft.transform.localEulerAngles.z;
//		float angle = (debug - initialRot.z);
//		//debug = angle;
//		float temp = station + (angle/10f);
//		initialRot.z = angle;
//		station = temp;
		//station = (Mathf.Floor (temp * 10)) / 10f;

//		if(Input.GetKey(KeyCode.UpArrow)){
//			float temp = station + 0.01f;
//			station = Mathf.Floor (temp * 10) / 10f;
//		}

		txt.text = "" + System.Math.Round(station,2) +" Hz";

	}
}