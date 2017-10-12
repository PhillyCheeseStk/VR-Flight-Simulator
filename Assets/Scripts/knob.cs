using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class knob : MonoBehaviour {


	// Use this for initialization
	void Start () {
	}

	public float BE;
	public bool CE;
	public int count;
	public float initialRot;
	public float lastRot;
	public float debug;

	// Update is called once per frame
	void Update () {
		BE = OVRInput.Get (OVRInput.Axis1D.SecondaryIndexTrigger);
		CE = OVRInput.Get (OVRInput.Touch.SecondaryThumbRest);
		CapsuleCollider col = this.GetComponent<CapsuleCollider> ();

		if (BE < 0.5f || !CE) {
			count = 0;
		}
			
		if (BE > 0.5f && CE) {
			GameObject hand = GameObject.FindGameObjectWithTag ("rightHand");
			GameObject radioLeft = GameObject.Find ("radioLeft");
			leftRadio leftRad = radioLeft.GetComponent<leftRadio>();


			Vector3 temp;
			if (count == 0) {
				initialRot = hand.transform.localEulerAngles.z;
				lastRot = 0;
			}
			count++;
			temp = transform.localEulerAngles;
			float phil = (hand.transform.localEulerAngles.z);
			if (phil > 180) {
				phil = -(phil - 360);
			} else {
				phil = -phil;
			}
			temp.z = -phil-initialRot;

			transform.localRotation = Quaternion.Euler(temp);
			debug = temp.z;
			//leftRad.station = Mathf.Floor((-temp.z * 10f))/10f;
			float ans = (float) System.Math.Round(((-temp.z/100f) - lastRot),2);
			leftRad.station += ans;
			lastRot = ans;
		}
	}


}
