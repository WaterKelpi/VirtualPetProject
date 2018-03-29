using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dragCamera : MonoBehaviour {
	public Vector3 pos;
	public float speed;
	public fmScript fm;

	void Awake(){
		fm = GameObject.Find("FarmManager").GetComponent<fmScript>();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButton(0)&&!fm.inMenu){
			pos.x -= Input.GetAxis("Mouse X")*speed*Time.deltaTime;
			pos.x = Mathf.Clamp(pos.x,-8.25f,8.25f);
			pos.y -= Input.GetAxis("Mouse Y")*speed*Time.deltaTime;
			pos.y = Mathf.Clamp(pos.y,-6.75f,6.75f);
			pos.z = -10;
		}
		Camera.main.transform.position = pos;
	}
}
