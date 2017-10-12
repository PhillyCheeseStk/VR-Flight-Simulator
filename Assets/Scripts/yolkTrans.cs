using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class yolkTrans : MonoBehaviour {

	// Use this for initialization

	float initial;
	public float pitch;
	public float roll; 
	public float yaw; 
	Vector2 Ayaw;
	public float elsaHosk;

	public OVRHapticsClip hapticsClip;
	public AudioClip vib;
		
	void Start () {
		initial = transform.localPosition.x;
		vib = GetComponent<AudioSource> ().clip;
	}

//	public void activatePickUpHaptics()
//	{
//		hapticsClip = new OVRHapticsClip(Vib);
//		OVRHaptics.RightChannel.Mix(hapticsClip);
//	}


	// Update is called once per frame
	void FixedUpdate () {
		Ayaw = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
		yaw = Ayaw.x;
		elsaHosk = transform.localEulerAngles.x;
		pitch = -(transform.localPosition.x - initial) / 0.114556f;
		//roll = transform.localEulerAngles.x;
		if (elsaHosk < 90f && elsaHosk > 0f) {
			roll = elsaHosk /90f ;  //90 corresponds to value of -1 or 1 lol		
		}
		if (elsaHosk < 359f && elsaHosk > 270f) {
			roll = (elsaHosk - 360f) / 90f;		
		}

		//Haptic feedback
		if (pitch > 0.95f || pitch < -0.95) {
			hapticsClip = new OVRHapticsClip(vib);
			OVRHaptics.Channels[0].Mix(hapticsClip);
			OVRHaptics.Channels[1].Mix(hapticsClip);
		}

	}
}
