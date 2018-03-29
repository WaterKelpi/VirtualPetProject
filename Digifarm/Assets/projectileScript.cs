using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody2D>().velocity = transform.up*10;
	}

	void OnCollisionEnter2D(Collision2D col){
		if(col.gameObject.tag == "Target"){
			col.gameObject.GetComponent<targetHandling>().health -= GameObject.Find("FarmManager").GetComponent<fmScript>().curMon.stats[5];
		}
		Destroy(this.gameObject);
	}
}
