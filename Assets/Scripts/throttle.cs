using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class throttle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		vib = GetComponent<AudioSource> ().clip;
	}
	
	// Update is called once per frame
	public float elsaHosk;
	public float power;
	public OVRHapticsClip hapticsClip;
	public AudioClip vib;

	void OnCollisionEnter(Collision collision) {
		//Haptic feedback
		hapticsClip = new OVRHapticsClip(vib);
		OVRHaptics.Channels[1].Mix(hapticsClip);
	}
		
	void Update () {
		elsaHosk = transform.localEulerAngles.x - 270f; //90 to normalize to 0
		power = elsaHosk/55.263f;
	}


}
