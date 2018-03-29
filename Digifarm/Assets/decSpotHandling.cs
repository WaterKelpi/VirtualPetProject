using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class decSpotHandling : MonoBehaviour {
	public fmScript fm;
	public Sprite avatar;
	public bool isCur;
	public decorationObject curDec;

	void Awake(){
		fm = GameObject.Find("FarmManager").GetComponent<fmScript>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0) && !fm.inMenu){
			RaycastHit2D hit = Physics2D.Raycast(new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,Camera.main.ScreenToWorldPoint(Input.mousePosition).y),Vector2.zero,0);
			SelectDec(hit);
		}
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !fm.inMenu){
			RaycastHit2D hit = Physics2D.Raycast(Input.GetTouch(0).position,Vector2.zero,0);
			SelectDec(hit);
		}
	}
	void SelectDec(RaycastHit2D hit){
		if(hit){
			if(hit.transform.gameObject == this.gameObject){
				fm.inMenu = true;
				fm.curDecSpot = this;
				fm.RefreshDec(fm.decDropdown,fm.deployableDec);
				fm.decSpotPanel.SetActive(true);
				isCur = true;
			}
		}else{
			isCur = true;
		}
	}
}
