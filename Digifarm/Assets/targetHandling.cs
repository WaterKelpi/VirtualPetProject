using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetHandling : MonoBehaviour {
	public int health;
	public int effortAmt;

	void Update(){
		if(health <= 0){
			GameObject.Find("FarmManager").GetComponent<fmScript>().curMon.TrainStat(5,effortAmt);
			Destroy(this.gameObject);
		}
	}
}
