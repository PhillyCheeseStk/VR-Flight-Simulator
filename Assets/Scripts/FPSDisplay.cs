using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSDisplay : MonoBehaviour {


	float deltaTime = 0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI(){
	
		int w = Screen.width, h = Screen.height;
		GUIStyle style = new GUIStyle ();

		Rect rect = new Rect (0, 0, w, h * 2 / 180);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0f, 0f, 0.5f, 1f);
		float msec = deltaTime * 1000.0f;
		float fps = 1f / deltaTime;
		string text = string.Format ("{0:0.0} ms ({1:0.} fps)", msec, fps);
		GUI.Label (rect, text, style);
	}

}



