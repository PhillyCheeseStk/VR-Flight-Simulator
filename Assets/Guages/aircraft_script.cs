using UnityEngine;
using System.Collections;

public class aircraft_script : MonoBehaviour {

	public float velocity;
	public float anglePitch;
	public float angleYaw;
	public float angleRoll;
	public float altitude;
	public float vel;
	public float gyro;
	public float gyroS; 
	public float altS;
	public float threshold = 0f;
	public int firstTime = 0;
	public Vector2 yaw; 

	public Vector3 pos; //current position


	// Use this for initialization
	void Start () {
	}
		

	// Update is called once per frame
	void FixedUpdate () {

		//yaw = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
		angleYaw = yaw.x;


		//OlD
		GameObject g = GameObject.FindGameObjectWithTag ("airplane");
		udpstuff udp = g.GetComponent<udpstuff> ();

		if (udp.heading != 0 && firstTime == 0) {
			gyroS = udp.heading; // Get initial heading in X-Plane
			altS = udp.altitude;
			firstTime++;
		}

		velocity = udp.speed;
		anglePitch = udp.pitch;
		angleRoll = udp.roll;
		gyro = udp.heading;
		altitude = udp.altitude;



		Vector3 pil	= transform.eulerAngles;
	    pil.z = anglePitch;
		pil.x = angleRoll;
		pil.y = gyro - gyroS- 90f;
		transform.rotation = Quaternion.Euler(pil);

//		GameObject terrain = GameObject.FindGameObjectWithTag ("terrain");
//		Vector3 terrainPos = terrain.transform.position;
//		terrainPos -= 0.4f*(transform.right * Time.deltaTime * velocity);
//		terrainPos.y = -(0.4f)*(udp.altitude - altS);
//		terrain.transform.position = terrainPos;

		GameObject terrain = GameObject.FindGameObjectWithTag ("terrain");
		Vector3 terrainPos = terrain.transform.position;
		terrainPos -= 6.67f*(transform.right * Time.deltaTime * velocity);
		terrainPos.y = -(6.67f)*(udp.altitude - altS);
		terrain.transform.position = terrainPos;


	}


}